namespace Common;

public static class Settings
{
    public static TimeSpan AccessTokenExpiration => TimeSpan.FromMinutes(15);
    public static TimeSpan RefreshTokenExpiration => TimeSpan.FromDays(30);
}
