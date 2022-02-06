using Assessment.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Linq;

namespace AssessmentTests
{
    public class TestDataStore
    {
        private DataStore _dateStore;

        [SetUp]
        public void Setup()
        {
            System.Environment.SetEnvironmentVariable("CoinMarkKey", "b54bcf4d-1bca-4e8e-9a24-22ff2c3d462c");
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<Assessment.CoinMarketCapAPI.API>();
            var api = new Assessment.CoinMarketCapAPI.API(logger, "https://sandbox-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");
            var dataRepo = new Assessment.Models.DateItemRepostoryInMemory() ;
            _dateStore = new Assessment.Models.DataStore(dataRepo, api);
        }

        [Test]
        public void TestDataStoreEndToEnd()
        {
            var data = _dateStore.GetLastDataItem(LimitResults: 10);
            Assert.IsTrue(data.Count > 0,"No items were returned");
            Assert.IsTrue(data.First().quotes.Count() > 0, "No quotes were returned");
        }
    }
}
