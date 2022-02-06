using Assessment.Models;
using Assessment.ORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Assessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssessmentController: ControllerBase
    {
        private const string MemoryKeyName_LatestRates = "LatestRates";
        private readonly ILogger<AssessmentController> _logger;
        private readonly DataStore _datastore;
        private readonly IMemoryCache _memoryCash;

        public AssessmentController(ILogger<AssessmentController> logger, Models.DataStore datastore, IMemoryCache memoryCash)
        {
            _logger = logger;
            _datastore = datastore;
            _memoryCash = memoryCash;
        }

        [HttpGet(Name = "GetLatest")]
        public IEnumerable<ORM.DataItem> Get()
        {
            //For performance in a live environment with multiple nodes use redis instead. 
            //For demo purposes use memorycashe. 
            //Set datetime threshold for state data;
            var lastRatesDate = DateTime.UtcNow.AddMinutes(-10);
            var latestRates = _memoryCash.Get<List<DataItem>>(MemoryKeyName_LatestRates);
            if (latestRates == null || latestRates.Count() == 0 || latestRates.First().RetrievedOn < lastRatesDate)
            {
                _logger.LogDebug("Stale rates found in cash or no rates. Going for API call");
                try
                {
                    latestRates = _datastore.GetLastDataItem(LimitResults: 10);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "While retrieving rates");
                }
                
                if (latestRates != null)
                {
                    _memoryCash.Set<List<DataItem>>(MemoryKeyName_LatestRates, latestRates);
                }
            }
            return latestRates;
        }
    }
}