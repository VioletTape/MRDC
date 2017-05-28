using System.Collections.Generic;
using System.IO;
using System.Linq;
using MRDC.Data;
using MRDC.Services;
using Serilog;

namespace MRDC {
    public class MarketDataCleanser {
        private ILogger log;

        public MarketDataCleanser() {
            log = Log.Logger;
        }

        public IEnumerable<MarketData> CleanupDataIn(IEnumerable<MarketData> marketData) {
            var validMarketData = marketData.Where(x => x.SelfValidate().Result).Select(x => x).ToList();
            log.Information("Correct records in set: {correctRecords}", validMarketData.Count);

            return new List<MarketData>();
        }

        public IEnumerable<MarketData> CleanupDataIn(DirectoryInfo soureDir) {
            log.Information("Start processing {SourceDir}", soureDir);

            var fileSevice = new FileSevice();
            var fileInfos = fileSevice.GetFiles(soureDir);

            var serializationService = new SerializationService();
            foreach (var fileInfo in fileInfos) {
                var marketData = serializationService.Deserialize(fileInfo);
                CleanupDataIn(marketData);
            }

            return new List<MarketData>();
        }
    }
}
