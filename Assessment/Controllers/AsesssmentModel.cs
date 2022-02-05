using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AsesssmentModel : ControllerBase
    {
      
        private readonly ILogger<AsesssmentModel> _logger;

        public AsesssmentModel(ILogger<AsesssmentModel> logger)
        {
            _logger = logger;
        }

     
        [HttpGet(Name = "GetLatestRedis")]
        public IEnumerable<WeatherForecast> Get()
        {
            //For performance in a live environment

            //For demo purposes


            return null;
        }
    }
}