using System;
using System.Collections.Generic;
using System.IO;
using Jil;
using MRDC.Data;
using Newtonsoft.Json;
using Serilog;

namespace MRDC.Services {
    internal class SerializationService {
//        internal List<MarketData> Read(FileInfo readFrom) {
//            var marketData = new List<MarketData>();
//            var rawData = File.ReadAllText(readFrom.FullName);
//            using (var stringReader = new StringReader(rawData)) {
//                marketData = JSON.Deserialize<List<MarketData>>(stringReader);
//            }
//            return marketData;
//        }
//
//        internal List<MarketData> Read2(FileInfo readFrom) {
//            var marketData = new List<MarketData>();
//            var rawData = File.ReadAllText(readFrom.FullName);
//            var settings = new JsonSerializerSettings {
//                                                          MissingMemberHandling = MissingMemberHandling.Ignore
//                                                          , MaxDepth = 3
//                                                      };
//            marketData = JsonConvert.DeserializeObject<List<MarketData>>(rawData, settings);
//            return marketData;
//        }

        internal List<MarketData> Deserialize(FileInfo readFrom) {
            Log.Logger.Information("Trying to read and deserialize file {FileToProcess}", readFrom.Name);
            var marketData = new List<MarketData>();

            try {
                using (var file = File.OpenText(readFrom.FullName))
                {
                    marketData = (List<MarketData>)new JsonSerializer().Deserialize(file, typeof(List<MarketData>));
                }
            }
            catch (Exception e) {
                Log.Logger.Error(e, "Something goes wrong with {FileToProcess}", readFrom.Name);
            }
            Log.Logger.Information("{FileToProcess} processed. {MarketDataNum} found.", readFrom.Name, marketData.Count);
            return marketData;
        }

        internal bool Serialize(List<MarketData> marketData, FileInfo writeTo) {
            using (var stringWriter = new StringWriter()) {
                JSON.Serialize(marketData, stringWriter, Options.ISO8601CamelCase);
                File.WriteAllText(writeTo.FullName, stringWriter.ToString());
            }
            return true;
        }
    }
}
