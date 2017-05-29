using System;
using System.Collections.Generic;
using System.IO;
using Jil;
using MRDC.Data;
using Newtonsoft.Json;
using Serilog;

namespace MRDC.Services {
    internal class SerializationService {
        /// <summary>
        /// Bulletproof deserialization by Newtonsoft.JSON, when formatting unknown
        /// </summary>
        /// <param name="readFrom">File with json data</param>
        /// <returns></returns>
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

        /// <summary>
        /// Serialize with compact format by JIL into file
        /// </summary>
        /// <param name="marketData">Data to serialize</param>
        /// <param name="writeTo">Target file</param>
        internal void SerializeFast(List<MarketData> marketData, FileInfo writeTo) {
            using (var stringWriter = new StringWriter()) {
                JSON.Serialize(marketData, stringWriter, Options.ISO8601CamelCase);
                File.WriteAllText(writeTo.FullName, stringWriter.ToString());
            }
        }

        /// <summary>
        /// Serialize with compact format by JIL into string
        /// </summary>
        /// <param name="marketData">Data to serialize</param>
        /// <returns>Serialized date</returns>
        internal string SerializeFast(List<MarketData> marketData) {
            return JSON.Serialize(marketData, Options.ISO8601CamelCase);
        }

        /// <summary>
        /// Deserialize from compact format by JIL
        /// </summary>
        /// <param name="readFrom">Source file</param>
        /// <returns>List of deserialied data</returns>
        internal List<MarketData> DeserializeFast(FileInfo readFrom) {
            var marketData = new List<MarketData>();
            try {
                using (var reader = File.OpenRead(readFrom.FullName)) {
                    var streamReader = new StreamReader(reader);
                    marketData = JSON.Deserialize<List<MarketData>>(streamReader, Options.ISO8601CamelCase);
                    streamReader.Close();
                }
            }
            catch (Exception e) {
                Log.Logger.Error(e, "Something goes wrong with {FileToSort}", readFrom.Name);
            }

            return marketData;
        }
    }
}
