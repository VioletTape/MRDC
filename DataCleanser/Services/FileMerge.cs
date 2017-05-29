using System;
using System.Collections.Generic;
using System.IO;
using MRDC.Data;

namespace MRDC.Services {
    /// <summary>
    ///     Continious writing to a really big json file. 
    /// </summary>
    internal class FileMerge : IDisposable {
        private readonly SerializationService serialization;
        private readonly FileStream fileStream;
        private readonly StreamWriter streamWriter;
        private bool isClosed;

        /// <summary>
        /// Continious writing to a really big json file. Use as any <see cref="IDisposable"/> implementation. 
        /// </summary>
        /// <param name="fileInfo">Target file</param>
        /// <param name="serializationService">Service that knows how to serialize data.</param>
        public FileMerge(FileInfo fileInfo, SerializationService serializationService) {
            serialization = serializationService;

            fileStream = File.Open(fileInfo.FullName, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);

            streamWriter.Write("[");
        }

        public void Dispose() {
            Close();
        }

        /// <summary>
        /// Append new data to file
        /// </summary>
        /// <param name="marketData">List of market data</param>
        /// <param name="isLastChunk">Marker that file going to be closed</param>
        public void Append(List<MarketData> marketData, bool isLastChunk = false) {
            var json = serialization.SerializeFast(marketData);
            json = json.Substring(1, json.Length - 2);
            var delimeter = isLastChunk ? "" : ",";
            streamWriter.Write(json + delimeter);
        }

        public void Close() {
            if (isClosed) {
                return;
            }

            streamWriter.Write("]");
            streamWriter.Close();
            fileStream.Close();

            isClosed = true;
        }
    }
}
