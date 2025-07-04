using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.ExtensionMethods
{
    public static class DateTimeExtension
    {
        public static DateRangeDtos ThisYear(this DateTime date)
        {
            DateRangeDtos range = new DateRangeDtos();

            range.Start = new DateTime(date.Year, 1, 1);
            range.End = range.Start.AddYears(1).AddSeconds(-1);

            return range;
        }

        public static DateRangeDtos LastYear(this DateTime date)
        {
            DateRangeDtos range = new DateRangeDtos();

            range.Start = new DateTime(date.Year - 1, 1, 1);
            range.End = range.Start.AddYears(1).AddSeconds(-1);

            return range;
        }

        public static DateRangeDtos ThisMonth(this DateTime date)
        {
            DateRangeDtos range = new DateRangeDtos();

            range.Start = new DateTime(date.Year, date.Month, 1);
            range.End = range.Start.AddMonths(1).AddSeconds(-1);

            return range;
        }

        public static DateRangeDtos LastMonth(this DateTime date)
        {
            DateRangeDtos range = new DateRangeDtos();

            range.Start = (new DateTime(date.Year, date.Month, 1)).AddMonths(-1);
            range.End = range.Start.AddMonths(1).AddSeconds(-1);

            return range;
        }

        public static DateRangeDtos ThisWeek(this DateTime date)
        {
            DateRangeDtos range = new DateRangeDtos();

            //range.Start = date.Date.AddDays(-(int)date.DayOfWeek);
            //range.End = range.Start.AddDays(7).AddSeconds(-1);

            range.End = date;
            range.Start = range.End.AddDays(-6).AddSeconds(-1);

            return range;
        }

        public static DateRangeDtos LastWeek(this DateTime date)
        {
            DateRangeDtos range = ThisWeek(date);

            range.Start = range.Start.AddDays(-7);
            range.End = range.End.AddDays(-7);

            return range;
        }

        public static string GetPrettyDate(this DateTime date)
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(date);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0 || dayDiff >= 31)
            {
                return null;
            }

            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 minute ago";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} minutes ago",
                        Math.Floor((double)secDiff / 60));
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} hours ago",
                        Math.Floor((double)secDiff / 3600));
                }
            }
            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return string.Format("{0} days ago",
                    dayDiff);
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} weeks ago",
                    Math.Ceiling((double)dayDiff / 7));
            }
            return null;
        }
    }
}
