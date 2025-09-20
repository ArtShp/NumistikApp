using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

[QueryProperty(nameof(Item), "item")]
public partial class UpdateCollectionItemViewModel : ObservableObject
{
    private readonly ICollectionItemService _itemService;
    private readonly ILookupService _lookupService;

    private FileResult? _obverseFile;
    private FileResult? _reverseFile;

    public ObservableCollection<LookupItem> Types { get; init; } = [];
    public ObservableCollection<LookupItem> Countries { get; init; } = [];
    public ObservableCollection<LookupItem> Statuses { get; init; } = [];
    public ObservableCollection<LookupItem> Qualities { get; init; } = [];
    public ObservableCollection<LookupItem> SpecialStatuses { get; init; } = [];

    // Current item
    private CollectionItem? _item;
    public CollectionItem? Item
    {
        get => _item!;
        set
        {
            if (SetProperty(ref _item, value) && value is not null)
            {
                // Pre-fill fields
                Value = value.Value;
                Currency = value.Currency;
                AdditionalInfo = value.AdditionalInfo;
                SerialNumber = value.SerialNumber;
                Description = value.Description;
                ObversePreviewPath = value.ObverseImageUrl;
                ReversePreviewPath = value.ReverseImageUrl;
            }
        }
    }

    // Selected lookup items
    private LookupItem? _selectedType;
    public LookupItem? SelectedType
    {
        get => _selectedType;
        set => SetProperty(ref _selectedType, value);
    }

    private LookupItem? _selectedCountry;
    public LookupItem? SelectedCountry
    {
        get => _selectedCountry;
        set => SetProperty(ref _selectedCountry, value);
    }

    private LookupItem? _selectedStatus;
    public LookupItem? SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }

    private LookupItem? _selectedSpecialStatus;
    public LookupItem? SelectedSpecialStatus
    {
        get => _selectedSpecialStatus;
        set => SetProperty(ref _selectedSpecialStatus, value);
    }

    private LookupItem? _selectedQuality;
    public LookupItem? SelectedQuality
    {
        get => _selectedQuality;
        set => SetProperty(ref _selectedQuality, value);
    }

    // Form fields
    private string _value = string.Empty;
    public string Value
    {
        get => _value;
        set => SetProperty(ref this._value, value);
    }

    private string _currency = string.Empty;
    public string Currency
    {
        get => _currency;
        set => SetProperty(ref _currency, value);
    }

    private string? _additionalInfo;
    public string? AdditionalInfo
    {
        get => _additionalInfo;
        set => SetProperty(ref _additionalInfo, value);
    }

    private string? _serialNumber;
    public string? SerialNumber
    {
        get => _serialNumber;
        set => SetProperty(ref _serialNumber, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string? _obversePreviewPath;
    public string? ObversePreviewPath
    {
        get => _obversePreviewPath;
        set => SetProperty(ref _obversePreviewPath, value);
    }

    private string? _reversePreviewPath;
    public string? ReversePreviewPath
    {
        get => _reversePreviewPath;
        set => SetProperty(ref _reversePreviewPath, value);
    }

    private bool _isSubmitting;
    public bool IsSubmitting
    {
        get => _isSubmitting;
        set => SetProperty(ref _isSubmitting, value);
    }

    public ICommand PickObverseCommand { get; init; }
    public ICommand PickReverseCommand { get; init; }
    public ICommand ClearObverseCommand { get; init; }
    public ICommand ClearReverseCommand { get; init; }
    public ICommand SaveCommand { get; init; }
    public ICommand CancelCommand { get; init; }

    public UpdateCollectionItemViewModel(ICollectionItemService itemService, ILookupService lookupService)
    {
        _itemService = itemService;
        _lookupService = lookupService;

        PickObverseCommand = new AsyncRelayCommand(PickObverseAsync);
        PickReverseCommand = new AsyncRelayCommand(PickReverseAsync);
        ClearObverseCommand = new RelayCommand(ClearObverse);
        ClearReverseCommand = new RelayCommand(ClearReverse);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => !IsSubmitting);
        CancelCommand = new AsyncRelayCommand(CancelAsync);
    }

    public async Task InitializeAsync()
    {
        // Load lookups
        var typesTask = _lookupService.GetTypesAsync();
        var countriesTask = _lookupService.GetCountriesAsync();
        var statusesTask = _lookupService.GetStatusesAsync();
        var qualitiesTask = _lookupService.GetQualitiesAsync();
        var specialStatusesTask = _lookupService.GetSpecialStatusesAsync();

        await Task.WhenAll(typesTask, countriesTask, statusesTask, qualitiesTask, specialStatusesTask);

        Replace(Types, typesTask.Result);
        Replace(Countries, countriesTask.Result);
        Replace(Statuses, statusesTask.Result);
        Replace(Qualities, qualitiesTask.Result);
        Replace(SpecialStatuses, specialStatusesTask.Result);

        // Optional pickers get a "None"
        InsertNoneOption(SpecialStatuses);
        InsertNoneOption(Qualities);

        // Preselect by IDs
        if (Item is not null)
        {
            SelectedType = Types.FirstOrDefault(x => x.Id == Item.TypeId) ?? Types.FirstOrDefault();
            SelectedCountry = Countries.FirstOrDefault(x => x.Id == Item.CountryId) ?? Countries.FirstOrDefault();
            SelectedStatus = Statuses.FirstOrDefault(x => x.Id == Item.StatusId) ?? Statuses.FirstOrDefault();
            SelectedSpecialStatus = Item.SpecialStatusId.HasValue
                ? SpecialStatuses.FirstOrDefault(x => x.Id == Item.SpecialStatusId.Value) ?? SpecialStatuses.FirstOrDefault()
                : SpecialStatuses.FirstOrDefault();
            SelectedQuality = Item.QualityId.HasValue
                ? Qualities.FirstOrDefault(x => x.Id == Item.QualityId.Value) ?? Qualities.FirstOrDefault()
                : Qualities.FirstOrDefault();
        }
        else
        {
            SelectedType ??= Types.FirstOrDefault();
            SelectedCountry ??= Countries.FirstOrDefault();
            SelectedStatus ??= Statuses.FirstOrDefault();
            SelectedSpecialStatus ??= SpecialStatuses.FirstOrDefault();
            SelectedQuality ??= Qualities.FirstOrDefault();
        }
    }

    private static void Replace(ObservableCollection<LookupItem> target, IReadOnlyList<LookupItem> source)
    {
        target.Clear();
        foreach (var i in source) target.Add(i);
    }

    private static void InsertNoneOption(ObservableCollection<LookupItem> target)
    {
        target.Insert(0, new LookupItem { Id = 0, Name = "--- None ---" });
    }

    private async Task PickObverseAsync()
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Select obverse image",
            FileTypes = FilePickerFileType.Images
        });

        if (result is not null)
        {
            _obverseFile = result;
            ObversePreviewPath = result.FullPath;
        }
    }

    private async Task PickReverseAsync()
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Select reverse image",
            FileTypes = FilePickerFileType.Images
        });

        if (result is not null)
        {
            _reverseFile = result;
            ReversePreviewPath = result.FullPath;
        }
    }

    private void ClearObverse()
    {
        _obverseFile = null;
        ObversePreviewPath = null;
    }

    private void ClearReverse()
    {
        _reverseFile = null;
        ReversePreviewPath = null;
    }

    private async Task SaveAsync()
    {
        if (Item is null)
        {
            await ShowAlert("Error", "Item is not specified.");
            return;
        }

        if (SelectedType is null || SelectedCountry is null || SelectedStatus is null
            || string.IsNullOrWhiteSpace(Value) || string.IsNullOrWhiteSpace(Currency))
        {
            await ShowAlert("Validation", "Please select Type, Country, Status and fill required fields.");
            return;
        }

        IsSubmitting = true;
        (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
        try
        {
            var request = new CollectionItemUpdateRequest
            {
                Id = Item.Id,
                CollectionId = Item.CollectionId,
                TypeId = SelectedType.Id,
                CountryId = SelectedCountry.Id,
                CollectionStatusId = SelectedStatus.Id,
                SpecialStatusId = SelectedSpecialStatus is { Id: > 0 } ? SelectedSpecialStatus.Id : null,
                QualityId = SelectedQuality is { Id: > 0 } ? SelectedQuality.Id : null,
                Value = Value,
                Currency = Currency,
                AdditionalInfo = AdditionalInfo,
                SerialNumber = SerialNumber,
                Description = Description,
                ObverseImage = _obverseFile,
                ReverseImage = _reverseFile
            };

            var ok = await _itemService.UpdateCollectionItemAsync(request);

            if (!ok)
            {
                await ShowAlert("Error", "Failed to update item.");
                return;
            }

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsSubmitting = false;
            (SaveCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    private Task CancelAsync() => Shell.Current.GoToAsync("..");

    private static Task ShowAlert(string title, string message)
        => Shell.Current.DisplayAlert(title, message, "OK");
}
