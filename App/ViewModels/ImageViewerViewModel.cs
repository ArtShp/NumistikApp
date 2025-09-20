using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

public partial class ImageViewerViewModel : ObservableObject
{
    private readonly ObservableCollection<string> _images = [];
    private int _index;

    public IReadOnlyList<string> Images => _images;
    public string? CurrentImage => _images[_index];
    public bool HasPrev => _index > 0;
    public bool HasNext => _index < _images.Count - 1;
    public bool HasMany => _images.Count > 1;

    public ICommand PrevCommand { get; init; }
    public ICommand NextCommand { get; init; }
    public ICommand ShareCommand { get; init; }
    public ICommand CopyCommand { get; init; }

    public ImageViewerViewModel()
    {
        PrevCommand = new RelayCommand(Prev, () => HasPrev);
        NextCommand = new RelayCommand(Next, () => HasNext);
        ShareCommand = new AsyncRelayCommand(ShareAsync);
        CopyCommand = new AsyncRelayCommand(CopyAsync);
    }

    public void Initialize(IEnumerable<string> images, int startIndex)
    {
        _images.Clear();
        foreach (var path in images)
        {
            if (!string.IsNullOrWhiteSpace(path)) _images.Add(path);
        }

        _index = _images.Count == 0 ? 0 : Math.Clamp(startIndex, 0, _images.Count - 1);
        RaiseAll();
    }

    private void Prev()
    {
        if (!HasPrev) return;

        _index--;
        RaiseAll();
    }

    private void Next()
    {
        if (!HasNext) return;

        _index++;
        RaiseAll();
    }

    private async Task ShareAsync()
    {
        var path = CurrentImage;
        if (string.IsNullOrWhiteSpace(path)) return;

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share image",
            File = new ShareFile(path, GetContentType(path))
        });
    }

    private async Task CopyAsync()
    {
        var path = CurrentImage;
        if (string.IsNullOrWhiteSpace(path)) return;

#if WINDOWS
        var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
        var dp = new Windows.ApplicationModel.DataTransfer.DataPackage();
        dp.SetBitmap(Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(file));
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dp);
#else
        await Clipboard.Default.SetTextAsync(path);
#endif
    }

    private static string GetContentType(string path)
    {
        var ext = Path.GetExtension(path)?.ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }

    private void RaiseAll()
    {
        OnPropertyChanged(nameof(CurrentImage));
        OnPropertyChanged(nameof(HasPrev));
        OnPropertyChanged(nameof(HasNext));
        OnPropertyChanged(nameof(HasMany));
        (PrevCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (NextCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }
}
