using FizzWare.NBuilder;
using FluentAssertions;
using MRDC.Data;
using NUnit.Framework;
using static System.String;

namespace MRDC.Tests.MarketDataTests.FollowingErrorsShouldBeCaught {
    [TestFixture]
    public class ForMarketData {
        private MarketData marketData;

        [SetUp]
        public void Init()
        {
            var instrument = Builder<Instrument>.CreateNew()
                                                .With(x => x.InstrumentId = 1)
                                                .Build();
            marketData = Builder<MarketData>.CreateNew()
                                            .With(x => x.Instrument = instrument)
                                            .With(x => 1.January(2011))
                                            .Build();

            marketData.SelfValidate()
                      .Result
                      .Should()
                      .BeTrue("Everything is ok according to types and rules");
        }

        [Test]
        public void ObjectShouldNotBeNull() {
            // arrange
            MarketData data = null;

            // act
            var validation = data.SelfValidate();

            // assert
            validation.Should()
                      .Be((false, "Market data is null"));
        }

        [Test]
        public void DataPointShouldBeNonNegativeNumber()
        {
            // arrange
            marketData.DataPointId = -1;

            // act 
            var validation = marketData.SelfValidate();

            // assert
            validation.Result
                      .Should()
                      .BeFalse("DataPointId should be positive");

            validation.ErrorMessage
                      .Should()
                      .Be("DataPointId should be positive number. ");

        }

        [Test]
        public void DateShouldBeAfterYear2010() {
            // arrange
            marketData.DateTime = 31.January(2010);

            // act 
            var validation = marketData.SelfValidate();

            // assert
            validation.Result
                      .Should()
                      .BeFalse("Valid dates start from 2011");

            validation.ErrorMessage
                      .Should()
                      .Be("Date of MarketData before 2011, that seems to be incorrect. ");

        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\t")]
        [TestCase("\r\n")]
        [TestCase(null)]
        public void ValueIsMissed(string val)
        {
            // arrange 
            marketData.Value = val;

            // assert
            var validation = marketData.SelfValidate();
            validation.Result
                      .Should()
                      .BeFalse("Instrument is not initialized");

            validation.ErrorMessage
                      .Should()
                      .Be("Value for MarketData is missed.");
        }

        [Test]
        public void ShouldProvideAggregatedMessage() {
            // arrange 
            marketData.Instrument.Name = Empty;
            marketData.DateTime = 1.January(2000);
            marketData.Value = Empty;

            // assert
            marketData.SelfValidate()
                      .ErrorMessage
                      .Should()
                      .Be("Name for the instrument was not provided. Date of MarketData before 2011, that seems to be incorrect. Value for MarketData is missed.");
        }
    }
}
