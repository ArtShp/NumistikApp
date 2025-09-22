using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace App.ViewModels;

[QueryProperty(nameof(CollectionId), "collectionId")]
public partial class CreateCollectionItemViewModel : ObservableObject
{
    private readonly ICollectionItemService _itemService;
    private readonly ILookupService _lookupService;

    private Guid _collectionGuid;
    private bool _isSubmitting;

    private FileResult? _obverseFile;
    private FileResult? _reverseFile;

    public ObservableCollection<LookupItem> Types { get; init; } = [];
    public ObservableCollection<LookupItem> Countries { get; init; } = [];
    public ObservableCollection<LookupItem> Statuses { get; init; } = [];
    public ObservableCollection<LookupItem> Qualities { get; init; } = [];
    public ObservableCollection<LookupItem> SpecialStatuses { get; init; } = [];

    // Selected items
    private LookupItem? selectedType;
    public LookupItem? SelectedType
    {
        get => selectedType;
        set => SetProperty(ref selectedType, value);
    }

    private LookupItem? selectedCountry;
    public LookupItem? SelectedCountry
    {
        get => selectedCountry;
        set => SetProperty(ref selectedCountry, value);
    }

    private LookupItem? selectedStatus;
    public LookupItem? SelectedStatus
    {
        get => selectedStatus;
        set => SetProperty(ref selectedStatus, value);
    }

    private LookupItem? selectedSpecialStatus;
    public LookupItem? SelectedSpecialStatus
    {
        get => selectedSpecialStatus;
        set => SetProperty(ref selectedSpecialStatus, value);
    }

    private LookupItem? selectedQuality;
    public LookupItem? SelectedQuality
    {
        get => selectedQuality;
        set => SetProperty(ref selectedQuality, value);
    }

    // Form fields
    private string value = string.Empty;
    public string Value
    {
        get => value;
        set => SetProperty(ref this.value, value);
    }

    private string currency = string.Empty;
    public string Currency
    {
        get => currency;
        set => SetProperty(ref currency, value);
    }

    private string? additionalInfo;
    public string? AdditionalInfo
    {
        get => additionalInfo;
        set => SetProperty(ref additionalInfo, value);
    }

    private string? serialNumber;
    public string? SerialNumber
    {
        get => serialNumber;
        set => SetProperty(ref serialNumber, value);
    }

    private string? description;
    public string? Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    private string? obversePreviewPath;
    public string? ObversePreviewPath
    {
        get => obversePreviewPath;
        set => SetProperty(ref obversePreviewPath, value);
    }

    private string? reversePreviewPath;
    public string? ReversePreviewPath
    {
        get => reversePreviewPath;
        set => SetProperty(ref reversePreviewPath, value);
    }

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

    public bool IsSubmitting
    {
        get => _isSubmitting;
        set => SetProperty(ref _isSubmitting, value);
    }

    public ICommand PickObverseCommand { get; init; }
    public ICommand PickReverseCommand { get; init; }
    public IAsyncRelayCommand SubmitCommand { get; init; }
    public IRelayCommand CancelCommand { get; init; }
    public IAsyncRelayCommand GoBackCommand { get; init; }

    public CreateCollectionItemViewModel(ICollectionItemService itemService, ILookupService lookupService)
    {
        _itemService = itemService;
        _lookupService = lookupService;

        PickObverseCommand = new AsyncRelayCommand(PickObverseAsync);
        PickReverseCommand = new AsyncRelayCommand(PickReverseAsync);
        SubmitCommand = new AsyncRelayCommand(SubmitAsync, () => !IsSubmitting);
        GoBackCommand = new AsyncRelayCommand(GoBackAsync);
        CancelCommand = new AsyncRelayCommand(GoBackAsync);
    }

    public async Task InitializeAsync()
    {
        var typesTask = _lookupService.GetTypesAsync();
        var countriesTask = _lookupService.GetCountriesAsync();
        var statusesTask = _lookupService.GetStatusesAsync();
        var qualitiesTask = _lookupService.GetQualitiesAsync();
        var specialStatusesTask = _lookupService.GetSpecialStatusesAsync();

        // await all together
        await Task.WhenAll(typesTask, countriesTask, statusesTask, qualitiesTask, specialStatusesTask);

        // update UI collections
        Replace(Types, typesTask.Result);
        Replace(Countries, countriesTask.Result);
        Replace(Statuses, statusesTask.Result);
        Replace(Qualities, qualitiesTask.Result);
        Replace(SpecialStatuses, specialStatusesTask.Result);

        // optional pickers get a "None" item
        InsertNoneOption(SpecialStatuses);
        InsertNoneOption(Qualities);

        // Preselect defaults
        SelectedType = Types.FirstOrDefault();
        SelectedCountry = Countries.FirstOrDefault();
        SelectedStatus = Statuses.FirstOrDefault();
        SelectedSpecialStatus = SpecialStatuses.FirstOrDefault();
        SelectedQuality = Qualities.FirstOrDefault();
    }

    private static void Replace(ObservableCollection<LookupItem> target, IReadOnlyList<LookupItem> source)
    {
        target.Clear();
        foreach (var i in source) target.Add(i);
    }

    private static void InsertNoneOption(ObservableCollection<LookupItem> target)
    {
        target.Insert(0, new LookupItem { Id = 0, Name = "— None —" });
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

    private async Task SubmitAsync()
    {
        if (_collectionGuid == Guid.Empty)
        {
            await ShowAlert("Error", "Collection is not specified.");
            return;
        }

        if (SelectedType is null || SelectedCountry is null || SelectedStatus is null
            || string.IsNullOrWhiteSpace(Value) || string.IsNullOrWhiteSpace(Currency))
        {
            await ShowAlert("Validation", "Please select Type, Country, Status and fill required fields.");
            return;
        }

        IsSubmitting = true;
        SubmitCommand.NotifyCanExecuteChanged();
        try
        {
            var request = new CollectionItemCreateRequest
            {
                TypeId = SelectedType.Id,
                CountryId = SelectedCountry.Id,
                CollectionStatusId = SelectedStatus.Id,
                SpecialStatusId = SelectedSpecialStatus is { Id: > 0 } ? SelectedSpecialStatus.Id : null,
                QualityId = SelectedQuality is { Id: > 0 } ? SelectedQuality.Id : null,
                CollectionId = _collectionGuid,
                Value = Value,
                Currency = Currency,
                AdditionalInfo = AdditionalInfo,
                SerialNumber = SerialNumber,
                Description = Description,
                ObverseImage = _obverseFile,
                ReverseImage = _reverseFile
            };

            var id = await _itemService.CreateCollectionItemAsync(request);

            if (id is null)
            {
                await ShowAlert("Error", "Failed to create item.");
                return;
            }

            await GoBackAsync();
        }
        finally
        {
            IsSubmitting = false;
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    private Task GoBackAsync() => Shell.Current.GoToAsync("..");

    private static Task ShowAlert(string title, string message)
        => Shell.Current.DisplayAlert(title, message, "OK");
}
