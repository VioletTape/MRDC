using System;
using System.Globalization;

namespace MRDC.Services {
    public interface IDateRangeStrategy {
        DateRange Get(DateTime startDate);
    }

    public class Last7Days : IDateRangeStrategy {
        public DateRange Get(DateTime startDate) {
            return new DateRange(startDate, startDate.AddDays(6));
        }
    }

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
