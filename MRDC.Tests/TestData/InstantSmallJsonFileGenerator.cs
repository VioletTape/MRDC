using System;
using System.IO;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using MRDC.Data;
using MRDC.Services;
using NUnit.Framework;

namespace MRDC.Tests.TestData {
    [TestFixture]
    public class InstantSmallJsonFileGenerator {
        [Test]
        [Ignore("Instant small file generator")]
        public void GenerateSmallFiles() {
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
    }
}
