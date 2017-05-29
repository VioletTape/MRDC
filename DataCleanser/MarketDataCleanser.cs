using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MRDC.Data;
using MRDC.Extenstions;
using MRDC.Services;
using Serilog;

namespace MRDC {
    public class MarketDataCleanser {
        private readonly ILogger log;
        private readonly List<(DateRange, string)> uniqueMarketData = new List<(DateRange, string)>();
        private readonly SerializationService serializationService;
        private readonly FileSevice fileSevice;

        public MarketDataCleanser(string logPath = "") {
            var pathFormat = Path.Combine(logPath, "log-{Date}.log");
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.Logger(Log.Logger)
                    .WriteTo.AsyncRollingFile(pathFormat)
                    .CreateLogger();

            log = Log.Logger;

            serializationService = new SerializationService();
            fileSevice = new FileSevice();


        }

        public void CleanupDataIn(DirectoryInfo soureDir, FileInfo saveToFile) {
            var stopwatch = Stopwatch.StartNew();

            Map(soureDir);
            Reduce(saveToFile);

            stopwatch.Stop();
            log.Information("Passed time {Reduce}", stopwatch.Elapsed.TotalSeconds);
        }

        internal void Map(DirectoryInfo soureDir) {
            log.Information("Start processing {SourceDir}", soureDir);

            fileSevice.CleanUpTempDir();

            foreach (var fileInfo in fileSevice.GetFiles(soureDir)) {
                var marketData = serializationService
                                         .Deserialize(fileInfo)
                                         .Where(x => x.SelfValidate().Result)
                                         .Select(x => x)
                                         .ToList();
                log.Information("Correct records in set: {correctRecords}", marketData.Count);

                var dictionary = marketData.GroupBy(key => key.DateTime.GetFullHours(), md => md)
                                                .ToDictionary(datas => datas.Key);

                foreach (var keyValue in dictionary) {
                    var tempDir = fileSevice.CreateTempDir(keyValue.Key.ToString("yyyyMMddHH"));
                    serializationService.SerializeFast(keyValue.Value.ToList(), tempDir.GetTempFile());
                }

                GC.Collect(2);
            }
        }

        internal void Reduce(FileInfo saveToFile) {

            fileSevice.CreateFile(saveToFile);
            var marketData = new List<MarketData>();
            var dateRangeStrategy = new Last7Days();

            using (var fileMerge = new FileMerge(saveToFile, serializationService)) {
                var sortedTempFolders = fileSevice.GetSortedTempFolders();
                var lastFolder = sortedTempFolders.Last();
                foreach (var directoryInfo in sortedTempFolders) {
                    var fileInfos = fileSevice.GetFiles(directoryInfo);
                    fileInfos.ForEach(i => marketData.AddRange(serializationService.DeserializeFast(i)));

                    marketData.Sort();
                    log.Information("Market data set sorted");

                    var dups = DeduplicateValue(marketData, dateRangeStrategy);
                    log.Information("Identified {MarketDataDups} duplicates", dups);

                    log.Information("Market data chunk sorted for {MarketDataDate}", directoryInfo.Name);
                    fileMerge.Append(marketData, lastFolder == directoryInfo);
                    marketData.Clear();
                }
            }

            fileSevice.CleanUpTempDir();
        }

        internal int DeduplicateValue(List<MarketData> marketData, IDateRangeStrategy strategy) {
            var counter = 0;
            foreach (var data in marketData)
                if (uniqueMarketData.Contains((data.DateTime, data.Value))) {
                    data.Value = "0";
                    counter++;
                }
                else {
                    uniqueMarketData.Add((strategy.Get(data.DateTime), data.Value));
                }
            return counter;
        }
    }
}
