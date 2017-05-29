using FluentAssertions;
using MRDC.Data;
using NUnit.Framework;

namespace MRDC.Tests.DateRangeTests {
    public class WhenCompare {
        [Test]
        public void SameRangeShouldBeEqual() {
            // act
            var dateRange = new DateRange(1.January(2017), 1.January(2017));

            // assert
            dateRange.Equals(dateRange)
                     .Should()
                     .BeTrue();
        }

        [Test]
        public void IncludingRangeShouldBeTreatedAsEqual() {
            // act
            var biggerRange = new DateRange(1.January(2017), 5.January(2017));
            var smallerRange = new DateRange(2.January(2017), 4.January(2017));

            // assert
            biggerRange.Equals(smallerRange)
                       .Should()
                       .BeTrue();
        }

        [Test]
        public void PointShouldBeEqualIfFallInRange() {
            // act
            var range = new DateRange(1.January(2017), 5.January(2017));

            // assert
            range.Equals(2.January(2017))
                 .Should()
                 .BeTrue();
        }
    }
}
