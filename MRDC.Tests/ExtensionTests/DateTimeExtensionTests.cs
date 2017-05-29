using System;
using FluentAssertions;
using MRDC.Extenstions;
using NUnit.Framework;

namespace MRDC.Tests.ExtensionTests {
    [TestFixture]
    public class DateTimeExtensionTests {
        [Test]
        public void ShouldTruncateMunites() {
            // arrange
            var sourceTime = new DateTime(2017, 1, 2, 3, 4, 5);

            // assert 
            sourceTime.GetFullHours()
                      .Should()
                      .Be(new DateTime(2017, 1, 2, 3, 0, 0));


        }
    }
}
