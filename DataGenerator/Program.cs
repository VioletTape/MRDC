using System;
using System.IO;
using System.Linq;

namespace DataGenerator {
    internal class Program {
        private static void Main(string[] rowArgs) {
            var args = ValidateArgs(rowArgs);

            if (!args.ok) {
                return;
            }

            using (var fileStream = File.OpenWrite(args.file.FullName)) {
                var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("[");
                for (var i = 0; i < args.records; i++) {
                    var t = 1495984110000 - i * 10_000;
                    streamWriter.Write("{\r\n    \"DateTime\": \"/Date(" + t + ")/\",\r\n    \"DataPointId\": 100,\r\n    \"Instrument\": {\r\n      \"InstrumentId\": 100,\r\n      \"Name\": \"Name1\"\r\n    },\r\n    \"Value\": \"100\"\r\n  }");
                    streamWriter.Write(",");
                }
                streamWriter.Write("{\r\n    \"DateTime\": \"/Date(" + 1495984110000 + ")/\",\r\n    \"DataPointId\": 100,\r\n    \"Instrument\": {\r\n      \"InstrumentId\": 100,\r\n      \"Name\": \"Name1\"\r\n    },\r\n    \"Value\": \"100\"\r\n  }");
                streamWriter.Write("]");
                streamWriter.Close();
            }

            Console.WriteLine("Done");
        }

        private static (FileInfo file, int records, bool ok) ValidateArgs(string[] args) {
            FileInfo fileInfo = null;
            var entries = 0;

            if (!args.Any()) {
                InfoMessage();
                return (fileInfo, entries, false);
            }

            if (args.Length != 2) {
                Console.WriteLine("Too many/less arguments.");
                InfoMessage();
                return (fileInfo, entries, false);
            }

            try {
                var fullPath = Path.GetFullPath(args[0]);
                fileInfo = new FileInfo(fullPath);
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
            catch (Exception e) {
                Console.WriteLine("Invalid path.");
                InfoMessage();
                return (fileInfo, entries, false);
            }

            if (!int.TryParse(args[1], out entries)) {
                Console.WriteLine("Can't parse numbe of records");
                InfoMessage();
                return (fileInfo, entries, false);
            }

            if (entries <= 0) {
                Console.WriteLine("Number of records should be positive number.");
                InfoMessage();
                return (fileInfo, entries, false);
            }
            return (fileInfo, entries, true);
        }

        private static void InfoMessage() {
            Console.WriteLine("Tool that generates big json file for testing.");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Provide path for source file and amount of records.");
            Console.WriteLine("For instance: c:\\\\Temp\\big.json 1000000");
        }
    }
}
