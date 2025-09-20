using System.Windows.Input;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

[QueryProperty(nameof(Item), "item")]
public partial class CollectionItemDetailsViewModel : ObservableObject
{
    private readonly IImageViewerService _imageViewerService;

    private CollectionItem? _item;
    public CollectionItem? Item
    {
        get => _item!;
        set => SetProperty(ref _item, value);
    }

    public ICommand OpenImageCommand { get; init; }

    public CollectionItemDetailsViewModel(IImageViewerService imageViewerService)
    {
        _imageViewerService = imageViewerService;

        OpenImageCommand = new AsyncRelayCommand<string?>(OpenImageAsync);
    }

    private async Task OpenImageAsync(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || Item is null)
            return;

        var images = new List<string>(2);
        if (!string.IsNullOrWhiteSpace(Item.ObverseImageUrl)) images.Add(Item.ObverseImageUrl!);
        if (!string.IsNullOrWhiteSpace(Item.ReverseImageUrl)) images.Add(Item.ReverseImageUrl!);
        if (images.Count == 0) return;

        var startIndex = Math.Max(0, images.FindIndex(path => string.Equals(path, imagePath)));
        await _imageViewerService.OpenAsync(images, startIndex);
    }
}
