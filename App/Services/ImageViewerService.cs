using App.ViewModels;
using App.Views;

namespace App.Services;

internal class ImageViewerService(IServiceProvider services) : IImageViewerService
{
    private readonly IServiceProvider _services = services;

    public async Task OpenAsync(IList<string> images, int startIndex)
    {
        var imagesToShow = images.Where(path => !string.IsNullOrWhiteSpace(path)).ToList();
        if (imagesToShow.Count == 0) return;

        var viewModel = _services.GetRequiredService<ImageViewerViewModel>();
        var start = Math.Clamp(startIndex, 0, imagesToShow.Count - 1);
        viewModel.Initialize(imagesToShow, start);

        var page = new ImageViewerPage(viewModel);

        // On WinUI open in new window, otherwise modal
        if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
        {
            var window = new ImageViewerWindow(page);
            Application.Current?.OpenWindow(window);
        }
        else if (Shell.Current?.Navigation is { } navigation)
        {
            await navigation.PushModalAsync(page, true);
        }
    }
}
