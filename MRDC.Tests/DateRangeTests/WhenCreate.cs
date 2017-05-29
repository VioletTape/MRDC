using System;
using FluentAssertions;
using MRDC.Data;
using NUnit.Framework;

namespace MRDC.Tests.DateRangeTests {
    [TestFixture]
    public class WhenCreate {
        [Test]
        public void ItShouldCreateRangeStartedFromBeginningOfDay() {
            // act
            var dateRange = new DateRange(1.January(2017), 1.January(2017));

            // assert
            dateRange.Contains(new DateTime(2017, 1, 1, 0, 0, 0))
                     .Should()
                     .BeTrue();
        }

        [Test]
        public void ItShouldCreateRangeEndedOnLastMSofProvidedDay() {
            // act
            var dateRange = new DateRange(1.January(2017), 1.January(2017));

            // assert
            var dateTime = new DateTime(2017, 1, 2).AddMilliseconds(-1);
            dateRange.Contains(dateTime)
                     .Should()
                     .BeTrue();
        }

        [Test]
        public void IsShouldCreateMomentWhenProvidedSingleValue() {
            // act
            var moment = new DateTime(2017, 1, 1, 1, 1, 1);
            var dateRange = new DateRange(moment);

            // assert
            dateRange.Contains(moment.AddMilliseconds(1))
                     .Should()
                     .BeFalse();

            dateRange.Contains(moment.AddMilliseconds(-1))
                     .Should()
                     .BeFalse();

            dateRange.Contains(moment)
                     .Should()
                     .BeTrue();
        }

        [Test]
        public void SameRangeShouldBeEqual() {
            // act
            var dateRange = new DateRange(1.January(2017), 1.January(2017));

            dateRange.Equals(dateRange)
                .Should()
                .BeTrue();
        }
            
        
    }
}
