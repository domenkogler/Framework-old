using System;

namespace Kogler.Framework
{
    public static partial class DateSpanExtensions
    {
        public static DateSpan AddDays(this IDateSpan source, int days)
        {
            return new DateSpan(source.Start, source.End.AddDays(days > 0 ? days : days < source.Duration().Days ? days : source.Duration().Days));
        }

        public static DateSpan ShiftDays(this IDateSpan source, int days)
        {
            return new DateSpan(source.Start.AddDays(days), source.End.AddDays(days));
        }

        public static DateSpan FullWeeks(this DateTime date)
        {
            return new DateSpan(date.BeginOfWeek(), date.EndOfWeek());
        }

        public static DateSpan FullWeeks(this IDateSpan source)
        {
            return new DateSpan(source.Start.BeginOfWeek(), source.End.EndOfWeek());
        }

        public static DateSpan WeekDays(this DateTime date)
        {
            return date.FullWeeks().AddDays(-2);
        }

        public static DateSpan WeekDays(this IDateSpan source)
        {
            return new DateSpan(source.Start.AddDays(-1).Next(DayOfWeek.Monday), source.End.AddDays(-1).FullWeeks().End.AddDays(-2));
        }

        public static DateSpan FullMonths(this DateTime date)
        {
            return new DateSpan(date.BeginOfMonth(), date.DaysInMonth());
        }

        public static DateSpan FullMonths(this IDateSpan source)
        {
            return new DateSpan(source.Start.BeginOfMonth(), source.End.EndOfMonth());
        }

        public static bool Contains(this IDateSpan source, DateTime date)
        {
            return ((source.Start < date) && (date < source.End));
        }

        public static bool ContainsInclusive(this IDateSpan source, DateTime date)
        {
            return ((source.Start <= date) && (date <= source.End));
        }

        public static bool Contains(this IDateSpan source, IDateSpan other)
        {
            return ((source.Start < other.Start) && (source.End > other.End));
        }

        public static bool ContainsInclusive(this IDateSpan source, IDateSpan other)
        {
            return ((source.Start <= other.Start) && (source.End >= other.End));
        }

        public static bool ContainsPartial(this IDateSpan source, IDateSpan other)
        {
            return Contains(source, other.Start) || Contains(source, other.End);
        }

        public static bool ContainsPartial(this IDateSpan source, TimeSpan other)
        {
            return ContainsPartial(source, new DateSpan(source.Start.Date, other));
        }

        public static bool ContainsPartialInclusive(this IDateSpan source, IDateSpan other)
        {
            return ContainsInclusive(source, other.Start) || ContainsInclusive(source, other.End);
        }

        public static TimeSpan Duration(this IDateSpan dateSpan)
        {
            return (dateSpan.End - dateSpan.Start);
        }

        public static bool IntersectsWith(this IDateSpan source, IDateSpan other)
        {
            if ((source == null) || (other == null))
            {
                return false;
            }
            return (((other.Start <= source.Start) && (source.Start < other.End)) || ((source.Start <= other.Start) && (other.Start < source.End)));
        }

        public static DateSpan Translate(this IDateSpan dateSpan, TimeSpan span)
        {
            return new DateSpan(dateSpan.Start + span, dateSpan.End + span);
        }

        public static bool Validate(this IDateSpan dateSpan)
        {
            return (((dateSpan.Start != DateTime.MinValue) && (dateSpan.End != DateTime.MaxValue)) && (dateSpan.Start <= dateSpan.End));
        }
    }
}
