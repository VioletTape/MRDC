using System;

namespace MRDC.Data {
    /// <summary>
    /// Represent date range 
    /// </summary>
    public struct DateRange {
        private readonly DateTime startDate;
        private readonly DateTime endDate;

        public DateRange(DateTime startDate, DateTime endDate) {
            this.startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var tmp = endDate.AddDays(1);
            this.endDate = new DateTime(tmp.Year, tmp.Month, tmp.Day).AddMilliseconds(-1);
            ;
        }

        public DateRange(DateTime startDate) : this() {
            this.startDate = startDate;
            endDate = startDate;
        }

        public bool Contains(DateTime date) {
            if (startDate <= date) {
                return date <= endDate;
            }
            return false;
        }

        public bool Contains(DateRange dateRange) {
            if (startDate <= dateRange.startDate) {
                return dateRange.endDate <= endDate;
            }
            return false;
        }

        public bool Equals(DateRange other) {
            return Contains(other);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is DateRange && Contains((DateRange) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (startDate.GetHashCode() * 397) ^ endDate.GetHashCode();
            }
        }

        public static implicit operator DateRange(DateTime time) {
            return new DateRange(time);
        }
    }
}
