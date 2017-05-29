using System;
using System.IO;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using MRDC.Data;
using MRDC.Services;
using NUnit.Framework;

namespace MRDC.Tests.FileMergeTests {
    [TestFixture]
    public class WhenWriteData {
        private FileInfo GetFileInfo() {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDir, "TestData", "merge.json");
            return new FileInfo(path);
        }

        [Test]
        public void WrittenChunksCanBeDeserialized() {
            // arrange
            var serializationService = new SerializationService();

            var instrument = new Instrument {
                                                InstrumentId = 1
                                                , Name = "name"
                                            };
            var marketData = Builder<MarketData>.CreateListOfSize(10)
                                                .All()
                                                .With(x => x.Instrument = instrument)
                                                .Build()
                                                .ToList();

            var fileInfo = GetFileInfo();

            // act
            using (var fileMerge = new FileMerge(fileInfo, serializationService)) {
                fileMerge.Append(marketData.Take(3).ToList());
                fileMerge.Append(marketData.Skip(3).Take(3).ToList());
                fileMerge.Append(marketData.Skip(6).Take(3).ToList());
                fileMerge.Append(marketData.Skip(9).Take(1).ToList(), true);
            }

            // assert
            serializationService.DeserializeFast(fileInfo)
                                .Count
                                .Should()
                                .Be(10);
        }
    }
}
