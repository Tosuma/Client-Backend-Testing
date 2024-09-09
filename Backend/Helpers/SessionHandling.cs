namespace Backend.Helpers;

public static class SessionHandling
{
    public const string SemaphoreHeld = "SemaphoreHeld";
    public const string WaitingForSemaphore = "WaitingForSemaphore";
    public static bool? GetBool(this ISession session, string key)
    {
        var value = session.GetString(key);
        if (bool.TryParse(value, out var result))
        {
            return result;
        }
        return null;
    }

    public static void SetBool(this ISession session, string key, bool value)
        => session.SetString(key, value.ToString());
}
