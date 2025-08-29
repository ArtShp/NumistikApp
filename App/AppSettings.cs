namespace App;

internal static class AppSettings
{
    private const string ServerUrlKey = "ServerUrl";

    public static string ServerUrl
    {
        get => Preferences.Default.Get(ServerUrlKey, "");
        set => Preferences.Default.Set(ServerUrlKey, value);
    }
}
