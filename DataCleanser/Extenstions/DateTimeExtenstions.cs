using System;

namespace MRDC.Extenstions {
    public static class DateTimeExtenstions {
        public static DateTime GetFullHours(this DateTime dt) {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0 , 0);
        }
    }
}