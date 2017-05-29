using System;

namespace MRDC.Extenstions {
    public static class DateTimeExtenstions {
        /// <summary>
        /// Truncate information about minutes and less fractions. Only date and hour left.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetFullHours(this DateTime dt) {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0 , 0);
        }
    }
}