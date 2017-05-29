using System.Text;
using MRDC.Extenstions;

namespace MRDC.Data {
    public class Instrument {
        public int InstrumentId;
        public string Name;

        public (bool Result, string ErrorMessage) SelfValidate() {
            var errorMessage = new StringBuilder();
            if (InstrumentId == 0) {
                errorMessage.Append("Instrument is not initizlized, because InstrumentId is 0. ");
            }

            if (Name.IsNullOrEmpty()) {
                errorMessage.Append("Name for the instrument was not provided. ");
            }

            var message = errorMessage.ToString();

            return (message.IsNullOrEmpty(), message);
        }
    }
}
