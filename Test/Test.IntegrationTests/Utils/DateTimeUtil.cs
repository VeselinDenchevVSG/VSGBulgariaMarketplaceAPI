namespace Test.IntegrationTests.Utils
{
    internal static class DateTimeUtil
    {
        internal static DateTime RoundToNearestSecond(DateTime dateTime)
        {
            return new(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second,
                dateTime.Kind // Preserve the DateTimeKind (Local, Utc, or Unspecified)
            );
        }
    }
}
