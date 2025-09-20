namespace App.Helpers;

public static class FileMimeHelper
{
    public static string GetContentType(FileResult fileResult)
    {
        if (!string.IsNullOrWhiteSpace(fileResult.ContentType))
            return fileResult.ContentType;

        return ExtensionToType(Path.GetExtension(fileResult.FileName ?? fileResult.FullPath));
    }

    public static string GetContentType(string path)
    {
        return ExtensionToType(Path.GetExtension(path));
    }

    private static string ExtensionToType(string? ext) => ext?.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
    };
}
