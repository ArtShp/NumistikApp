using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;
using App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using App.Messages;

namespace App.ViewModels;

[QueryProperty(nameof(CollectionId), "collectionId")]
public partial class CollectionItemsViewModel : ObservableObject
{
    private readonly ICollectionItemService _itemsService;
    private readonly IImageService _imageService;

    public ObservableCollection<CollectionItem> Items { get; init; } = [];

    private Guid _collectionGuid;
    private int? _lastSeenId;
    private bool _isLoading;
    private bool _hasMore = true;

    public string? CollectionId
    {
        set
        {
            if (Guid.TryParse(value, out var id))
            {
                _collectionGuid = id;
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool HasMore
    {
        get => _hasMore;
        set => SetProperty(ref _hasMore, value);
    }

    public ICommand RefreshCommand { get; init; }
    public ICommand LoadMoreCommand { get; init; }
    public ICommand AddItemCommand { get; init; }
    public ICommand DeleteItemCommand { get; init; }
    public ICommand OpenItemCommand { get; init; }
    public ICommand UpdateItemCommand { get; init; }

    public CollectionItemsViewModel(ICollectionItemService itemsService, IImageService imageService)
    {
        _itemsService = itemsService;
        _imageService = imageService;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync, () => !IsLoading);
        AddItemCommand = new AsyncRelayCommand(OpenCreateItemAsync);
        DeleteItemCommand = new AsyncRelayCommand<CollectionItem>(DeleteItemAsync);
        OpenItemCommand = new AsyncRelayCommand<CollectionItem>(OpenItemAsync);
        UpdateItemCommand = new AsyncRelayCommand<CollectionItem>(OpenUpdateItemAsync);

        // Listen for item updates coming from the Update page
        WeakReferenceMessenger.Default.Register<CollectionItemUpdatedMessage>(this, (recipient, message) =>
        {
            MainThread.BeginInvokeOnMainThread(async () => await OnItemUpdatedAsync(message.Value));
        });
    }

    public async Task InitializeAsync()
    {
        if (Items.Count == 0)
        {
            await RefreshAsync();
        }
    }

    private async Task RefreshAsync()
    {
        Items.Clear();
        _lastSeenId = null;
        HasMore = true;

        await LoadMoreAsync();
    }

    private async Task LoadMoreAsync()
    {
        if (IsLoading || _collectionGuid == Guid.Empty) return;

        IsLoading = true;
        try
        {
            var page = await _itemsService.GetCollectionItemsAsync(_collectionGuid, _lastSeenId);
            int? lastId = null;

            foreach (var ci in page)
            {
                var obverseImageTask = _imageService.GetLocalPathAsync(ci.ObverseImageUrl);
                var reverseImageTask = _imageService.GetLocalPathAsync(ci.ReverseImageUrl);

                await Task.WhenAll(obverseImageTask, reverseImageTask);

                ci.ObverseImageUrl = obverseImageTask.Result ?? null;
                ci.ReverseImageUrl = reverseImageTask.Result ?? null;

                Items.Add(ci);
                lastId = ci.Id;
            }

            if (!page.Any() || lastId is null)
            {
                HasMore = false;
            }
            else
            {
                _lastSeenId = lastId;
                HasMore = true;
            }
        }
        finally
        {
            IsLoading = false;
            (LoadMoreCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }

        if (!HasMore)
        {
            await Shell.Current.DisplayAlert("Nothing loaded", "No more items to load.", "OK");
        }
    }

    private async Task OpenCreateItemAsync()
    {
        if (_collectionGuid == Guid.Empty) return;

        await Shell.Current.GoToAsync(nameof(CreateCollectionItemPage), new Dictionary<string, object>
        {
            ["collectionId"] = _collectionGuid.ToString()
        });
    }

    private async Task OpenItemAsync(CollectionItem? item)
    {
        if (item is null) return;

        await Shell.Current.GoToAsync(nameof(CollectionItemDetailsPage), new Dictionary<string, object>
        {
            ["item"] = item
        });
    }

    private async Task OpenUpdateItemAsync(CollectionItem? item)
    {
        if (item is null) return;

        await Shell.Current.GoToAsync(nameof(UpdateCollectionItemPage), new Dictionary<string, object>
        {
            ["item"] = item
        });
    }

    private async Task DeleteItemAsync(CollectionItem? item)
    {
        if (item is null) return;

        var confirm = await Shell.Current.DisplayAlert("Delete item",
            "Are you sure you want to delete this item?", "Delete", "Cancel");
        if (!confirm) return;

        bool ok = await _itemsService.DeleteCollectionItemAsync(item.CollectionId, item.Id);
        if (!ok)
        {
            await Shell.Current.DisplayAlert("Error", "Unable to delete item. You might not have permissions.", "OK");
            return;
        }

        Items.Remove(item);
    }

    private async Task OnItemUpdatedAsync(CollectionItem updated)
    {
        updated.ObverseImageUrl = await _imageService.GetLocalPathAsync(updated.ObverseImageUrl);
        updated.ReverseImageUrl = await _imageService.GetLocalPathAsync(updated.ReverseImageUrl);

        // Replace the item in the collection to trigger UI update
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Id == updated.Id)
            {
                Items[i] = updated;
                break;
            }
        }
    }
}
