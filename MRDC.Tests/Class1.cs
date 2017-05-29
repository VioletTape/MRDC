using System;
using System.IO;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using MRDC.Data;
using MRDC.Services;
using NUnit.Framework;
using Serilog;

namespace MRDC.Tests {
    [TestFixture]
    public class Class1 {
        [OneTimeSetUp]
        public void LogSetup() {
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.LiterateConsole()
                    .CreateLogger();
        }

        [Test]
        public void testname() {
            var instruments = Builder<Instrument>.CreateListOfSize(3)
                                                 .Build()
                                                 .ToList();

            var marketData = Builder<MarketData>.CreateListOfSize(10)
                                                .TheFirst(3)
                                                .With(x => x.Instrument = instruments[0])
                                                .TheNext(4)
                                                .With(x => x.Instrument = instruments[1])
                                                .TheNext(3)
                                                .With(x => x.Instrument = instruments[2])
                                                .Build()
                                                .ToList();

            var rd = new SerializationService();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var combine = Path.Combine(baseDir, "TestData\\in_valid_records.json");
            var fileInfo = new FileInfo(combine);

            new FileSevice().CreateFile(fileInfo);

            rd.SerializeFast(marketData, fileInfo);

            var marketDatas = rd.Deserialize(fileInfo);
            marketDatas.Count.Should().Be(10);
        }

        [Test]
        public void ReadIncorrectData() {
            var rd = new SerializationService();

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var combine = Path.Combine(baseDir, "TestData\\incorrect_records.json");
            var fileInfo = new FileInfo(combine);

            var marketDatas = rd.Deserialize(fileInfo);
            marketDatas.Count.Should().Be(9);
        }

        [Test]
        public void HugeFileGenerare() {
            // 100M > 10Gb

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var combine = Path.Combine(baseDir, "TestData\\big.json");
            var fileInfo = new FileInfo(combine);

            using (var fileStream = File.OpenWrite(fileInfo.FullName)) {
                var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("[");
                for (var i = 0; i < 5_000_000; i++) {
                    var t = 1495984110000 - i * 10_000;
                    streamWriter.Write("{\r\n    \"DateTime\": \"/Date(" + t + ")/\",\r\n    \"DataPointId\": 100,\r\n    \"Instrument\": {\r\n      \"InstrumentId\": 100,\r\n      \"Name\": \"Name1\"\r\n    },\r\n    \"Value\": \"100\"\r\n  }");
                    streamWriter.Write(",");
                }
                streamWriter.Write("{\r\n    \"DateTime\": \"/Date(" + 1495984110000 + ")/\",\r\n    \"DataPointId\": 100,\r\n    \"Instrument\": {\r\n      \"InstrumentId\": 100,\r\n      \"Name\": \"Name1\"\r\n    },\r\n    \"Value\": \"100\"\r\n  }");
                streamWriter.Write("]");
                streamWriter.Close();
            }
        }


        [Test]
        public void BigRead() {
            var readFrom = new FileInfo(@"D:\Projects\MRDC\ConsoleRunner\bin\Debug\Result\cleanData.json");
            var deserializeFast = new SerializationService().DeserializeFast(readFrom);
        }
    }
}
