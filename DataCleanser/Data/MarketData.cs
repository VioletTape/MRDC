using System;
using System.Collections.Generic;
using System.Text;
using MRDC.Extenstions;

namespace MRDC.Data {
    public class MarketData : IComparable<MarketData> {
        public Instrument Instrument;
        public int DataPointId;
        public DateTime DateTime;
        public string Value;


        public int CompareTo(MarketData other) {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;
            return DateTime.CompareTo(other.DateTime);
        }

        private sealed class ValueEqualityComparer : IEqualityComparer<MarketData> {
            public bool Equals(MarketData x, MarketData y) {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.DateTime, y.DateTime);
            }

            public int GetHashCode(MarketData obj) {
                return obj.DateTime.GetHashCode();
            }
        }

        public static IEqualityComparer<MarketData> DateComparer { get; } = new ValueEqualityComparer();
    }

    public static class MarketDataExtension {
        public static (bool Result, string ErrorMessage) SelfValidate(this MarketData data) {
            if (data == null) {
                return (false, "Market data is null");
            }

            var errorMessage = new StringBuilder();
            if (data.Instrument == null) {
                errorMessage.Append("Instrument is null. ");
            }
            else {
                var isValidInstrument = data.Instrument.SelfValidate();
                if (!isValidInstrument.Result) {
                    errorMessage.Append(isValidInstrument.ErrorMessage);
                }
            }

            if (data.DataPointId <= 0) {
                errorMessage.Append("DataPointId should be positive number. ");
            }

            if (data.DateTime.Year < 2011) {
                errorMessage.Append("Date of MarketData before 2011, that seems to be incorrect. ");
            }

            if (data.Value.IsNullOrEmpty()) {
                errorMessage.Append("Value for MarketData is missed.");
            }

            var message = errorMessage.ToString();

            return (message.IsNullOrEmpty(), message);
        }
    }
}
