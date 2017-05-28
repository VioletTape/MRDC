using System.Collections.Generic;
using System.IO;
using MRDC.Data;
using MRDC.Services;

namespace MRDC {
    public class MarketDataCleanser {
        public IEnumerable<MarketData> CleanupDataIn(IEnumerable<MarketData> marketData) {
            return new List<MarketData>();
        }

        public IEnumerable<MarketData> CleanupDataIn(DirectoryInfo soureDir) {
            var fileSevice = new FileSevice();
            var fileInfos = fileSevice.GetFiles(soureDir);


            return new List<MarketData>();
        }
    }
}
