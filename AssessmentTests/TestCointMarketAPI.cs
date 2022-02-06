using Assessment.CoinMarketCapAPI;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;

namespace AssessmentTests
{
    public class TestCointMarketAPI
    {
        private API _API;
        private API _BadlyconfiguredAPI;

        [SetUp]
        public void Setup()
        {
            //Using the provided public test key. 
            System.Environment.SetEnvironmentVariable("CoinMarkKey", "b54bcf4d-1bca-4e8e-9a24-22ff2c3d462c");
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<Assessment.CoinMarketCapAPI.API>();
            _API = new Assessment.CoinMarketCapAPI.API(logger, "https://sandbox-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");
            System.Environment.SetEnvironmentVariable("CoinMarkKey", "");
            //The sandbox does not care what the key is and does not throw an exception so we have to use production for this test. 
            _BadlyconfiguredAPI = new Assessment.CoinMarketCapAPI.API(logger, "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");
        }
               
    

        [Test]
        public void TestSuccessfulAPICall()
        {
            Assert.DoesNotThrow(() => _API.GetLatestListingsAsync(limit: 10).GetAwaiter().GetResult(), "API call to sandbox threw error.");
        }

        [Test]
        public void TestDataReturned()
        {
            var data = _API.GetLatestListingsAsync(limit: 10).GetAwaiter().GetResult();
            Assert.IsTrue(data != null && data.data.Length > 0, "API call did not provide any data");
        }

        [Test]
        public void TestInvalidDataThrows()
        {
            Assert.Throws(typeof(Exception),() => _BadlyconfiguredAPI.GetLatestListingsAsync(limit:10).GetAwaiter().GetResult(), "API should fail with no key");
        }
    }
}