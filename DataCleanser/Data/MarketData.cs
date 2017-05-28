using System;
using System.Text;
using MRDC.Extenstions;

namespace MRDC.Data {
    public class MarketData {
        public Instrument Instrument;
        public uint DataPointId;
        public DateTime DateTime;
        public string Value;

        public (bool Result, string ErrorMessage) SelfValidate() {
            var errorMessage = new StringBuilder();
            if (Instrument == null) {
                errorMessage.Append("Instrument is null. ");
            }
            else {
                var isValidInstrument = Instrument.SelfValiate();
                if (!isValidInstrument.Result)
                    errorMessage.Append(isValidInstrument.ErrorMessage);
            }

            if (DateTime.Year < 2011) {
                errorMessage.Append("Date of MarketData before 2011, that seems to be incorrect. ");
            }

            if (Value.IsNullOrEmpty()) {
                errorMessage.Append("Value for MarketData is missed.");
            }

            var message = errorMessage.ToString();

            return (message.IsNullOrEmpty(), message);
        }
    }
}
