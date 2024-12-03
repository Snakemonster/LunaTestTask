namespace LunaTestTask;

public static class Configuration
{
    public static string TokenSecretKey => Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "DefaultSecretKeyDefaultSecretKey";
}