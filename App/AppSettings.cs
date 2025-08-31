namespace App;

internal static class AppSettings
{
    private const string ServerUrlKey = "ServerUrl";
    private const string RefreshTokenKey = "RefreshToken";
    private const string RefreshTokenExpiryKey = "RefreshTokenExpiry";
    private const string UsernameKey = "Username";

    private const string DefaultServerUrl = "https://localhost:7163";

    public static string ServerUrl
    {
        get => Preferences.Default.Get(ServerUrlKey, DefaultServerUrl);
        set => Preferences.Default.Set(ServerUrlKey, value);
    }

    public static string RefreshToken
    {
        get => Preferences.Default.Get(RefreshTokenKey, string.Empty);
        set => Preferences.Default.Set(RefreshTokenKey, value);
    }

    public static DateTime? RefreshTokenExpiry
    {
        get
        {
            var expiryValue = Preferences.Default.Get(RefreshTokenExpiryKey, string.Empty);

            if (DateTime.TryParse(expiryValue, out var result))
                return result;

            return null;
        }

        set => Preferences.Default.Set(RefreshTokenExpiryKey, value?.ToString("o") ?? string.Empty);
    }

    public static string Username
    {
        get => Preferences.Default.Get(UsernameKey, string.Empty);
        set => Preferences.Default.Set(UsernameKey, value);
    }
}
