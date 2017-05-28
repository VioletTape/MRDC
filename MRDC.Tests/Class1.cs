using System;
using System.IO;
using System.Linq;
using System.Reflection;
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


            var loggerConfiguration = Log.Logger as LoggerConfiguration;
            loggerConfiguration.WriteTo.LiterateConsole();
            Log.Logger = loggerConfiguration.CreateLogger();
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

            rd.Serialize(marketData, fileInfo);

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
    }
}
