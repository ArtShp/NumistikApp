namespace App.Services;

public interface IImageViewerService
{
    Task OpenAsync(IList<string> images, int startIndex);
}
