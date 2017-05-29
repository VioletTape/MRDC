using System;
using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Serilog;

namespace MRDC.Tests.MarketDataCleanserTests {
    [TestFixture]
    [Category("Integration")]
    public class WhenMapData {
        private StringBuilder logged;

        [OneTimeSetUp]
        public void LogSetup() {
            logged = new StringBuilder();
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.TextWriter(new StringWriter(logged))
                    .CreateLogger();
        }

        [Test]
        public void ShouldCleanUpIncorrectData() {
            var service = new MarketDataCleanser();

            service.Map(GetDirectoryInfoFor("9_incorrect_records.json"));

            logged.ToString()
                .Should()
                .Contain("Correct records in set: 0");
        }

        [Test]
        public void ShouldRecognizeCorrectData()
        {
            var service = new MarketDataCleanser();

            service.Map(GetDirectoryInfoFor("10_valid_records.json"));

            logged.ToString()
                  .Should()
                  .Contain("Correct records in set: 10");
        }

        [TearDown]
        public void CleanUp() {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var testDataPath = Path.Combine(baseDir, "TestData", "MapTest");
            Directory.Delete(testDataPath, true);
        }

        private DirectoryInfo GetDirectoryInfoFor(string name)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var testDataPath = Path.Combine(baseDir, "TestData");
            var destinationPath = Path.Combine(testDataPath, "MapTest");
            Directory.CreateDirectory(destinationPath);
            File.Copy(Path.Combine(testDataPath, name), Path.Combine(destinationPath, name), true);

            return new DirectoryInfo(destinationPath);
        }
    }
}
