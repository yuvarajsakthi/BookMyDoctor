namespace BookMyDoctor.Server.Common
{
    public static class DateTimeHelper
    {
        public static DateTime ToIst(DateTime utcDateTime)
        {
            TimeZoneInfo istTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);
        }
    }
}