namespace Test.IntegrationTests.Extensions;

public static class DateTimeExtensions
{
    public static DateTime RoundToNearestSecond(this DateTime dateTime)
        => new(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            dateTime.Minute,
            dateTime.Second,
            // Preserve the DateTimeKind (Local, Utc, or Unspecified)
            dateTime.Kind);
}