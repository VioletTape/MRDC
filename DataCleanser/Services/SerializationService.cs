using System;
using System.Collections.Generic;
using System.IO;
using Jil;
using MRDC.Data;
using Newtonsoft.Json;
using Serilog;

namespace MRDC.Services {
    internal class SerializationService {
        internal List<MarketData> Deserialize(FileInfo readFrom) {
            Log.Logger.Information("Trying to read and deserialize file {FileToProcess}", readFrom.Name);
            var marketData = new List<MarketData>();

            try {
                using (var file = File.OpenText(readFrom.FullName)) {
                    marketData = (List<MarketData>) new JsonSerializer().Deserialize(file, typeof(List<MarketData>));
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
