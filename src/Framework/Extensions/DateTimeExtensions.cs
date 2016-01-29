using System;

namespace Kogler.Framework
{
    public static class DateTimeExtensions
    {
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static DateTime Next(this DateTime date, DayOfWeek day)
        {
            return date.AddDays(((int)day - (int)date.DayOfWeek + 7) % 7);
        }

        public static DateTime Previous(this DateTime date, DayOfWeek day)
        {
            return Next(date, day).AddDays(-7);
        }

        public static DateTime BeginOfWeek(this DateTime date, DayOfWeek weekBeginsWith = DayOfWeek.Monday)
        {
            return Next(date.AddDays(-6), weekBeginsWith);
        }

        public static DateTime EndOfWeek(this DateTime date, DayOfWeek weekBeginsWith = DayOfWeek.Monday)
        {
            return BeginOfWeek(date, weekBeginsWith).AddDays(7);
        }

        public static DateTime BeginOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DaysInMonth(date));
        }

        public static int DaysInMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }

        public static bool IsBetween(this DateTime date, DateTime start, DateTime stop)
        {
            return start <= date && stop >= date.AddDays(1);
        }

        public static DateTime RoundUp(this DateTime dateTime, TimeSpan timeSpan)
        {
            double a = (dateTime.Ticks / (double)timeSpan.Ticks);
            return new DateTime((timeSpan.Ticks * (long)Math.Ceiling(a)));
        }

        public static TimeSpan RoundUp(this TimeSpan dateTime, TimeSpan timeSpan)
        {
            double a = (dateTime.Ticks / (double)timeSpan.Ticks);
            return new TimeSpan((timeSpan.Ticks * (long)Math.Ceiling(a)));
        }

        public static DateTime RoundDown(this DateTime dateTime, TimeSpan timeSpan)
        {
            double a = (dateTime.Ticks / (double)timeSpan.Ticks);
            return new DateTime((timeSpan.Ticks * (long)Math.Floor(a)));
        }

        public static TimeSpan RoundDown(this TimeSpan dateTime, TimeSpan timeSpan)
        {
            double a = (dateTime.Ticks / (double)timeSpan.Ticks);
            return new TimeSpan((timeSpan.Ticks * (long)Math.Floor(a)));
        }
    }
}