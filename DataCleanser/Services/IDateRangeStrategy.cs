using System;
using System.Globalization;
using MRDC.Data;

namespace MRDC.Services {
    public interface IDateRangeStrategy {
        DateRange Get(DateTime startDate);
    }

    /// <summary>
    /// Create date range started at provided data and 7 days long
    /// </summary>
    public class Last7Days : IDateRangeStrategy {
        public DateRange Get(DateTime startDate) {
            return new DateRange(startDate, startDate.AddDays(7));
        }
    }

    /// <summary>
    /// Create date range started at first day of week for provided date
    /// </summary>
    public class CalendarWeek : IDateRangeStrategy {
        public DateRange Get(DateTime startDate) {
            var currentInfoFirstDayOfWeek = DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;

            var dayOfWeek = (int) startDate.DayOfWeek;
            var firstDayOfWeek = (int) currentInfoFirstDayOfWeek;
            var offset = dayOfWeek - firstDayOfWeek;
            if (offset == 0) {
                return new DateRange(startDate, startDate.AddDays(7));
            }
            var addDays = startDate.AddDays(-7 - offset);
            return new DateRange(addDays, addDays.AddDays(7));
        }
    }
}
