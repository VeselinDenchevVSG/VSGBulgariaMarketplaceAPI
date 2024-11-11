namespace Test.IntegrationTests.Extensions;

public static class DateTimeExtensions
{
    public static DateTime? RoundToNearestSecond(this DateTime? nullableDateTime)
    {
        if (nullableDateTime is null) return null;

        return RoundToNearestSecond(nullableDateTime.Value);
    }

    public static DateTime RoundToNearestSecond(this DateTime dateTime)
    {
        return new(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            dateTime.Minute,
            dateTime.Second,
            // Preserve the DateTimeKind (Local, Utc, or Unspecified)
            dateTime.Kind);
    }
}