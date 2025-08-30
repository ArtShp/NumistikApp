namespace App;

internal static class AppSettings
{
    private const string ServerUrlKey = "ServerUrl";

    private const string DefaultServerUrl = "https://localhost:7163";

    public static string ServerUrl
    {
        get => Preferences.Default.Get(ServerUrlKey, DefaultServerUrl);
        set => Preferences.Default.Set(ServerUrlKey, value);
    }
}
