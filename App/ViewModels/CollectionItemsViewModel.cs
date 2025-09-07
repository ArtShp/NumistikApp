using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

[QueryProperty(nameof(CollectionId), "collectionId")]
public partial class CollectionItemsViewModel : ObservableObject
{
    private readonly ICollectionItemService _itemsService;
    private readonly IImageService _imageService;

    public ObservableCollection<CollectionItemPreview> Items { get; init; } = [];

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

    public CollectionItemsViewModel(ICollectionItemService itemsService, IImageService imageService)
    {
        _itemsService = itemsService;
        _imageService = imageService;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync, () => HasMore && !IsLoading);
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
        if (!HasMore || IsLoading || _collectionGuid == Guid.Empty) return;

        IsLoading = true;
        try
        {
            var page = await _itemsService.GetCollectionItemsAsync(_collectionGuid, _lastSeenId);
            int? lastId = null;

            foreach (var ci in page)
            {
                ci.ObverseImageUrl = await _imageService.GetLocalPathAsync(ci.ObverseImageUrl) ?? null;
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
    }
}
