using System;
using System.Text;
using MRDC.Extenstions;

namespace MRDC.Data {
    public class MarketData {
        public Instrument Instrument;
        public int DataPointId;
        public DateTime DateTime;
        public string Value;
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
                var isValidInstrument = data.Instrument.SelfValiate();
                if (!isValidInstrument.Result) {
                    errorMessage.Append(isValidInstrument.ErrorMessage);
                }
            }

            if (data.DataPointId < 0) {
                errorMessage.Append("DataPointId should be non-negative number. ");
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
