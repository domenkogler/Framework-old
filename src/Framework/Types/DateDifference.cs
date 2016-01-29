using System;

namespace Kogler.Framework
{
    public class DateDifference
    {
        #region << Members >>

        private readonly DateTime m_FromDate;
        private readonly DateTime m_ToDate;

        /// <summary>
        /// defining Number of days in month; index 0=> january and 11=> December
        /// february contain either 28 or 29 days, that's why here value is -1
        /// which wil be calculate later.
        /// </summary>
        private readonly int[] m_MonthDay = { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        #endregion

        #region << Constructor >>

        public DateDifference(DateTime d1, DateTime d2)
        {
            if (d1 > d2)
            {
                m_FromDate = d2;
                m_ToDate = d1;
            }
            else
            {
                m_FromDate = d1;
                m_ToDate = d2;
            }

            // 
            // Day Calculation
            // 
            int increment = 0;

            if (m_FromDate.Day > m_ToDate.Day)
            {
                increment = m_MonthDay[m_FromDate.Month - 1];
            }
            // if it is february month
            // if it's to day is less then from day
            if (increment == -1)
            {
                increment = DateTime.IsLeapYear(m_FromDate.Year) ? 29 : 28;
            }
            if (increment != 0)
            {
                Days = (m_ToDate.Day + increment) - m_FromDate.Day;
                increment = 1;
            }
            else
            {
                Days = m_ToDate.Day - m_FromDate.Day;
            }

            //
            //month calculation
            //
            if ((m_FromDate.Month + increment) > m_ToDate.Month)
            {
                Months = (m_ToDate.Month + 12) - (m_FromDate.Month + increment);
                increment = 1;
            }
            else
            {
                Months = (m_ToDate.Month) - (m_FromDate.Month + increment);
                increment = 0;
            }

            //
            // year calculation
            //
            Years = m_ToDate.Year - (m_FromDate.Year + increment);
        }

        #endregion

        #region << Properties >>

        public int Years { get; }

        public int Months { get; }

        public int Days { get; }

        #endregion
    }
}