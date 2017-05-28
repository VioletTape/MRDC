using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MRDC.Data;
using MRDC.Services;
using Serilog;

namespace MRDC {
    public class MarketDataCleanser {
        private readonly ILogger log;
        private List<(DateRange, string)> uniqueMarketData;

        public MarketDataCleanser() {
            log = Log.Logger;
        }

        public IEnumerable<MarketData> CleanupDataIn(IEnumerable<MarketData> marketData) {
            var validMarketData = marketData.Where(x => x.SelfValidate().Result).Select(x => x).ToList();
            log.Information("Correct records in set: {correctRecords}", validMarketData.Count);
            GC.Collect(2);
            return validMarketData;
        }

        public IEnumerable<MarketData> CleanupDataIn(DirectoryInfo soureDir) {
            log.Information("Start processing {SourceDir}", soureDir);

            var fileSevice = new FileSevice();
            var fileInfos = fileSevice.GetFiles(soureDir);

            var serializationService = new SerializationService();
            foreach (var fileInfo in fileInfos) {
                var marketData = serializationService.Deserialize(fileInfo);

                var validMarketData = marketData.Where(x => x.SelfValidate().Result).Select(x => x).ToList();
                log.Information("Correct records in set: {correctRecords}", validMarketData.Count);

                validMarketData.Sort();
                log.Information("Market data set sorted");

                var dups = DeduplicateValue(validMarketData, new Last7Days());
                log.Information("Identified {MarketDataDups} duplicates", dups);

                GC.Collect(2);
            }

            return new List<MarketData>();
        }

        public int DeduplicateValue(List<MarketData> marketData, IDateRangeStrategy strategy) {
            uniqueMarketData = new List<(DateRange, string)>();
            var counter = 0;
            foreach (var data in marketData)
            {
                if (uniqueMarketData.Contains(value: (data.DateTime, data.Value))) {
                    data.Value = "0";
                    counter++;
                }
                else {
                    uniqueMarketData.Add((strategy.Get(data.DateTime), data.Value));
                }
            }
            return counter;
        }
    }
}
