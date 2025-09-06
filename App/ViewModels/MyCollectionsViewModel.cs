using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

public partial class MyCollectionsViewModel : ObservableObject
{
    private readonly ICollectionService _collectionService;

    private readonly ObservableCollection<MyCollectionDto> _collections = [];

    private Guid? _lastSeenId;
    private string? _lastSeenName;
    private bool _isLoading;
    private bool _hasMore = true;
    private MyCollectionDto? _selected;

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

    public MyCollectionDto? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public ICommand LoadMoreCommand { get; init; }
    public ICommand RefreshCommand { get; init; }
    public ICommand OpenSelectedCommand { get; init; }
    public ICommand OpenCommand { get; init; }

    public MyCollectionsViewModel(ICollectionService collectionService)
    {
        _collectionService = collectionService;

        LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync, () => HasMore && !IsLoading);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        OpenSelectedCommand = new AsyncRelayCommand(OpenSelectedAsync);
        OpenCommand = new AsyncRelayCommand<MyCollectionDto?>(OpenAsync);
    }

    public async Task InitializeAsync()
    {
        if (_collections.Count == 0)
        {
            await RefreshAsync();
        }
    }

    private async Task RefreshAsync()
    {
        _collections.Clear();

        _lastSeenId = null;
        _lastSeenName = null;
        HasMore = true;

        await LoadMoreAsync();
    }

    private async Task LoadMoreAsync()
    {
        if (!HasMore || IsLoading) return;

        IsLoading = true;
        try
        {
            var page = await _collectionService.GetMyCollectionsAsync(_lastSeenId, _lastSeenName);
            MyCollectionDto? last = null;

            foreach (var item in page)
            {
                _collections.Add(item);
                last = item;
            }

            if (page.Count == 0 || last is null)
            {
                HasMore = false;
                return;
            }

            _lastSeenId = last.Id;
            _lastSeenName = last.Name;
            HasMore = true;
        }
        finally
        {
            IsLoading = false;
            (LoadMoreCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    private async Task OpenSelectedAsync()
    {
        await OpenAsync(Selected);
    }

    private async Task OpenAsync(MyCollectionDto? item)
    {
        if (item is null) return;

        await Shell.Current.GoToAsync($"CollectionItemsPage?collectionId={item.Id}");
    }
}
