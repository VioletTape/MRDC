using System.IO;
using FluentAssertions;
using MRDC.Extenstions;
using NUnit.Framework;

namespace MRDC.Tests.ExtensionTests {
    [TestFixture]
    public class DirectoryInfoExtenstionsTests
    {
        [Test]
        public void ShouldProvideValidPath() {
            // arrange 
            var directoryInfo = new DirectoryInfo(@"c:\temp");

            // act
            var tempFile = directoryInfo.GetTempFile();

            // assert
            tempFile.DirectoryName
                    .Should()
                    .Be(@"c:\temp");

            tempFile.Name
                    .Should()
                    .NotBeNullOrWhiteSpace();
        }
    }
}