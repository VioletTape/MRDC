using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using MRDC.Services;
using NUnit.Framework;

namespace MRDC.Tests.SerializationServiceTests {
    [TestFixture]
    [Category("Integration")]
    public class WhenDeserializeExternalData {
        private SerializationService service;

        private FileInfo GetFileInfo(string name) {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDir, "TestData", name);
            return new FileInfo(path);
        }

        [SetUp]
        public void Init() {
            service = new SerializationService();
        }

        [Test]
        public void ShouldDeserializeValidRecord() {
            // act
            var marketData = service.Deserialize(GetFileInfo("1_valid_record"))
                                    .Single();

            marketData.DataPointId.Should().Be(100);
            marketData.Value.Should().Be("100");
            marketData.DateTime.Should().Be(new DateTime(2017, 5, 27, 22, 0, 0)); // 2017-05-27 22:00:00
            marketData.Instrument.InstrumentId.Should().Be(100);
            marketData.Instrument.Name.Should().Be("Name1");
        }

        [Test]
        public void ShouldDeserializeIncompleteRecords() {
            // act
            var marketData = service.Deserialize(GetFileInfo("9_incorrect_records.json"));

            marketData.Count
                      .Should()
                      .Be(9);
        }
    }
}
