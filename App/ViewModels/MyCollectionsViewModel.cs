using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Collection;

namespace App.ViewModels;

public partial class MyCollectionsViewModel : ObservableObject
{
    private readonly ICollectionService _collectionService;

    public ObservableCollection<MyCollectionDto> Collections { get; init; } = [];

    private Guid? _lastSeenId;
    private string? _lastSeenName;
    private bool _isLoading;
    private bool _hasMore = true;
    private MyCollectionDto? _selected;

    public CreateCollectionForm Form { get; init; } = new();

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
    public ICommand ToggleCreateCommand { get; init; }
    public ICommand CreateCommand { get; init; }
    public ICommand CancelCreateCommand { get; init; }

    public MyCollectionsViewModel(ICollectionService collectionService)
    {
        _collectionService = collectionService;

        LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync, () => HasMore && !IsLoading);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        OpenSelectedCommand = new AsyncRelayCommand(OpenSelectedAsync);
        OpenCommand = new AsyncRelayCommand<MyCollectionDto?>(OpenAsync);
        ToggleCreateCommand = new RelayCommand(() => Form.IsVisible = !Form.IsVisible);
        CreateCommand = new AsyncRelayCommand(CreateAsync, CanCreate);
        CancelCreateCommand = new RelayCommand(CancelCreate);

        Form.PropertyChanged += OnFormPropertyChanged;
    }

    private void OnFormPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(CreateCollectionForm.Name) or nameof(CreateCollectionForm.IsCreating))
        {
            (CreateCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    public async Task InitializeAsync()
    {
        if (Collections.Count == 0)
        {
            await RefreshAsync();
        }
    }

    private async Task RefreshAsync()
    {
        Collections.Clear();

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
                Collections.Add(item);
                last = item;
            }

            if (!page.Any() || last is null)
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

    private bool CanCreate()
    {
        return !Form.IsCreating && !string.IsNullOrWhiteSpace(Form.Name);
    }

    private async Task CreateAsync()
    {
        if (!CanCreate()) return;

        Form.IsCreating = true;
        try
        {
            var id = await _collectionService.CreateCollectionAsync(new CollectionCreationDto.Request
            {
                Name = Form.Name.Trim(),
                Description = Form.Description?.Trim()
            });

            if (id is not null)
            {
                CancelCreate();
                await RefreshAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to create collection.", "OK");
            }
        }
        finally
        {
            Form.IsCreating = false;
            (CreateCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    private void CancelCreate()
    {
        Form.Reset();
        Form.IsVisible = false;
    }
}
