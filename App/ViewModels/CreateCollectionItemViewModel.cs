using System.Windows.Input;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

[QueryProperty(nameof(CollectionId), "collectionId")]
public partial class CreateCollectionItemViewModel : ObservableObject
{
    private readonly ICollectionItemService _service;

    private Guid _collectionGuid;
    private bool _isSubmitting;
    
    private FileResult? _obverseFile;
    private FileResult? _reverseFile;

    private int typeId;
    public int TypeId
    {
        get => typeId;
        set => SetProperty(ref typeId, value);
    }

    private int countryId;
    public int CountryId
    {
        get => countryId;
        set => SetProperty(ref countryId, value);
    }

    private int collectionStatusId;
    public int CollectionStatusId
    {
        get => collectionStatusId;
        set => SetProperty(ref collectionStatusId, value);
    }

    private int? specialStatusId;
    public int? SpecialStatusId
    {
        get => specialStatusId;
        set => SetProperty(ref specialStatusId, value);
    }

    private int? qualityId;
    public int? QualityId
    {
        get => qualityId;
        set => SetProperty(ref qualityId, value);
    }

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

    public CreateCollectionItemViewModel(ICollectionItemService service)
    {
        _service = service;

        PickObverseCommand = new AsyncRelayCommand(PickObverseAsync);
        PickReverseCommand = new AsyncRelayCommand(PickReverseAsync);
        SubmitCommand = new AsyncRelayCommand(SubmitAsync, () => !IsSubmitting);
        GoBackCommand = new AsyncRelayCommand(GoBackAsync);
        CancelCommand = new AsyncRelayCommand(GoBackAsync);
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

        if (TypeId <= 0 || CountryId <= 0 || CollectionStatusId <= 0 ||
            string.IsNullOrWhiteSpace(Value) || string.IsNullOrWhiteSpace(Currency))
        {
            await ShowAlert("Validation", "Please fill all required fields.");
            return;
        }

        IsSubmitting = true;
        SubmitCommand.NotifyCanExecuteChanged();
        try
        {
            var request = new CollectionItemCreateRequest
            {
                TypeId = TypeId,
                CountryId = CountryId,
                CollectionStatusId = CollectionStatusId,
                SpecialStatusId = SpecialStatusId,
                QualityId = QualityId,
                CollectionId = _collectionGuid,
                Value = Value,
                Currency = Currency,
                AdditionalInfo = AdditionalInfo,
                SerialNumber = SerialNumber,
                Description = Description,
                ObverseImage = _obverseFile,
                ReverseImage = _reverseFile
            };

            var id = await _service.CreateCollectionItemAsync(request);

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

    private Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..");
    }

    private static Task ShowAlert(string title, string message)
        => Shell.Current.DisplayAlert(title, message, "OK");
}
