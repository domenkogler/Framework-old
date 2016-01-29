using System;
using System.Globalization;

namespace Kogler.Framework
{
    public interface IDateSpan
    {
        DateTime Start { get; }
        DateTime End { get; }
        TimeSpan Duration { get; }
    }

    public struct DateSpan : IDateSpan, IComparable, IComparable<IDateSpan>, IEquatable<IDateSpan>, IFormattable
    {
        #region << Construstors >>

        private readonly Calendar Calendar;

        public DateSpan(DateTime start, DateTime end, bool ignoreLastDay = false, Calendar calendar = null)
        {
            m_IgnoreLastDay = ignoreLastDay;
            Calendar = calendar ?? CultureInfo.CurrentCulture.Calendar;
            this.start = start;
            this.end = end;
        }

        public DateSpan(IDateSpan other, bool ignoreLastDay = false, Calendar calendar = null) : this(other.Start, other.End, ignoreLastDay, calendar) { }

        public DateSpan(DateTime start, TimeSpan duration, bool ignoreLastDay = false, Calendar calendar = null) : this(start, start.Add(duration), ignoreLastDay, calendar) { }

        /// <summary>
        /// Add days to DateTime
        /// </summary>
        /// <param name="start">start date</param>
        /// <param name="days">days to add</param>
        /// <param name="ignoreLastDay"></param>
        /// <param name="calendar"></param>
        public DateSpan(DateTime start, int days, bool ignoreLastDay = false, Calendar calendar = null) : this(start, start.AddDays(days), ignoreLastDay, calendar) { }

        #endregion

        #region << Properties >>

        public TimeSpan Duration { get { return end - start; } }
        public int Days { get { return Duration.Days; } }

        private bool m_IgnoreLastDay;
        public bool IgnoreLastDay
        {
            get { return m_IgnoreLastDay; }
            set { m_IgnoreLastDay = value; }
        }

        public DateTime EndIgnored
        {
            get
            {
                if (IgnoreLastDay)
                {
                    var endIgnored = end.AddDays(-1);
                    return endIgnored < start ? start : endIgnored;
                }
                return end;
            }
        }

        public bool IsSameYear { get { return start.Year.Equals(EndIgnored.Year); } }
        public bool IsSameMonth { get { return IsSameYear && start.Month.Equals(EndIgnored.Month); } }
        public bool IsSameWeek
        {
            get
            {
                return Calendar.GetWeekOfYear(start, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                    .Equals(Calendar.GetWeekOfYear(EndIgnored, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
            }
        }
        public bool IsSameDay { get { return start.Equals(EndIgnored); } }

        #region IDateSpan Members

        private DateTime start;
        public DateTime Start
        {
            get { return start; }
            set
            {
                //if (start == value) return;
                start = value;
                //RaisePropertyChanged("Start", "Duration", "Days");
            }
        }

        private DateTime end;

        public DateTime End
        {
            get { return end; }
            set
            {
                //if (end == value) return;
                end = value;
                //RaisePropertyChanged("End", "EndIgnored", "Duration", "Days");
            }
        }

        #endregion

        #endregion

        #region << Overrides of Object Type >>>

        public override bool Equals(object obj)
        {
            var other = obj as IDateSpan;
            return (((other != null) && (Start == other.Start)) && (End == other.End));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ start.GetHashCode();
                result = (result * 397) ^ end.GetHashCode();
                return result;
            }
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDateSpan other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDateSpan other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "Start: {0}, End: {1}", new object[] { start, end });
        }

        #endregion

        #region << Operators >>

        public static DateSpan operator +(DateSpan date, TimeSpan time)
        {
            date.End.Add(time);
            return date;
        }

        public static DateSpan operator -(DateSpan date, TimeSpan time)
        {
            date.End.Add(-time);
            return date;
        }

        public static DateSpan operator +(DateSpan date, DateSpan date1)
        {
            date.End.Add(date1.Duration);
            return date;
        }

        public static DateSpan operator -(DateSpan date, DateSpan date1)
        {
            date.End.Add(-date1.Duration);
            return date;
        }

        public static bool operator ==(DateSpan d1, DateSpan d2)
        {
            return d1.Start == d2.Start &&
                   d1.End == d2.End &&
                   d1.IgnoreLastDay == d2.IgnoreLastDay;
        }

        public static bool operator !=(DateSpan d1, DateSpan d2)
        {
            return !(d1 == d2);
        }

        public static bool operator <(DateSpan d1, DateSpan d2)
        {
            return d1.Duration < d2.Duration;
        }

        public static bool operator <=(DateSpan d1, DateSpan d2)
        {
            return d1.Duration <= d2.Duration;
        }

        public static bool operator >(DateSpan d1, DateSpan d2)
        {
            return d1.Duration > d2.Duration;
        }

        public static bool operator >=(DateSpan d1, DateSpan d2)
        {
            return d1.Duration >= d2.Duration;
        }

        #endregion

        public static DateSpan ThisWeek
        {
            get { return new DateSpan(DateTime.Today.BeginOfWeek(), 7); }
        }

        public static DateSpan NextWeek
        {
            get { return ThisWeek.ShiftDays(7); }
        }

        public static DateSpan PreviousWeek
        {
            get { return ThisWeek.ShiftDays(-7); }
        }

        public static DateSpan ThisMonth
        {
            get { return DateTime.Today.FullMonths(); }
        }

        public static DateSpan NextMonth
        {
            get { return DateTime.Today.EndOfMonth().AddDays(1).FullMonths(); }
        }
    }
}
