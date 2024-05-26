namespace Domain;

public static class Constants
{
    public const string DevDbString =
        "Host=localhost;Port=5432;Database=exchangerates;Username=postgres;Password=DbPassword123_";
    public const string DevHangfireDbString =
        "Host=localhost;Port=5432;Database=exchangerates_hangfire;Username=postgres;Password=DbPassword123_";
    public const string DevJwtSecretKey =
        "dummy_secret_for_development_mode";
}