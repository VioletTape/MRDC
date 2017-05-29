using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using FizzWare.NBuilder;
using FluentAssertions;
using MRDC.Data;
using MRDC.Services;
using NUnit.Framework;
using Serilog;

namespace MRDC.Tests.MarketDataCleanserTests {
    [TestFixture]
    public class WhenDeduplicateData {
        [OneTimeSetUp]
        public void LogSetup() {
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.LiterateConsole()
                    .CreateLogger();
        }

        [SetUp]
        public void Init() {
            var culture = (CultureInfo) CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        [Test]
        public void ShouldIndetifyDupsForUseLast7DayStrategy() {
            // arrange 
            var marketData = new List<MarketData> {
                                                      new MarketData {
                                                                         DateTime = 28.May(2017)
                                                                         , Value = "A"
                                                                     }
                                                      , new MarketData {
                                                                           DateTime = 29.May(2017)
                                                                           , Value = "B"
                                                                       }
                                                      , new MarketData {
                                                                           DateTime = 30.May(2017)
                                                                           , Value = "A"
                                                                       }
                                                  };
            // act 
            new MarketDataCleanser().DeduplicateValue(marketData, new Last7Days());

            // assert
            marketData[0].Value.Should().Be("A");
            marketData[1].Value.Should().Be("B");
            marketData[2].Value.Should().Be("0");
        }

        [Test]
        public void ShouldPassUseLast7DayStrategy_7days() {
            // arrange
            var marketData = new List<MarketData> {
                                                      new MarketData {
                                                                         DateTime = 28.May(2017)
                                                                         , Value = "A"
                                                                     }
                                                      , new MarketData {
                                                                           DateTime = 28.May(2017).AddDays(7)
                                                                           , Value = "A"
                                                                       }
                                                  };
            // act 
            new MarketDataCleanser().DeduplicateValue(marketData, new Last7Days());

            // assert
            marketData[1]
                    .Value
                    .Should()
                    .Be("A");
        }

        [Test]
        public void ShouldNotPassUseLast7DayStrategy_6days() {
            // arrange
            var marketData = new List<MarketData> {
                                                      new MarketData {
                                                                         DateTime = 28.May(2017)
                                                                         , Value = "A"
                                                                     }
                                                      , new MarketData {
                                                                           DateTime = 28.May(2017).AddDays(6)
                                                                           , Value = "A"
                                                                       }
                                                  };
            // act 
            new MarketDataCleanser().DeduplicateValue(marketData, new Last7Days());

            // assert
            marketData[1]
                    .Value
                    .Should()
                    .Be("0");
        }

        [Test]
        public void ShouldCorrectlyUseCalendarWeekStrategy() {
            // arrange 
            var marketData = new List<MarketData> {
                                                      new MarketData {
                                                                         DateTime = 28.May(2017)
                                                                         , Value = "A"
                                                                     }
                                                      , new MarketData {
                                                                           DateTime = 29.May(2017)
                                                                           , Value = "B"
                                                                       }
                                                      , new MarketData {
                                                                           DateTime = 29.May(2017)
                                                                           , Value = "A"
                                                                       }
                                                  };

            // act
            new MarketDataCleanser().DeduplicateValue(marketData, new CalendarWeek());

            // assert
            marketData[0].Value.Should().Be("A");
            marketData[1].Value.Should().Be("B");
            marketData[2].Value.Should().Be("A", "it is Monday");
        }


        [Test]
        public void ShouldCountDups() {
            // arrange
            var instrument = Builder<Instrument>.CreateNew()
                                                .Build();

            var marketData = Builder<MarketData>.CreateListOfSize(10)
                                                .All()
                                                .With(x => x.Instrument = instrument)
                                                .TheFirst(3)
                                                .With(x => x.Value = "A")
                                                .TheNext(4)
                                                .With(x => x.Value = "B")
                                                .TheNext(3)
                                                .With(x => x.Value = "C")
                                                .Build()
                                                .ToList();

            // assert
            new MarketDataCleanser().DeduplicateValue(marketData, new Last7Days())
                                    .Should()
                                    .Be(10 - 3);
        }
    }
}
