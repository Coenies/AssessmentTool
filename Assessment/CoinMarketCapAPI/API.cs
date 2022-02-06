using Assessment.Models.CoinmarketCap;
using Assessment.ORM;
using System.Net;
using System.Web;

namespace Assessment.CoinMarketCapAPI
{
    public class API
    {
        //Static variable to hold API key
        private static string _API_Key = string.Empty;

        //Set the environment key name
        private const string _envKey = "CoinMarkKey";
        //base URL of production api as default
        private readonly string _baseURL = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest";

        private readonly ILogger<API> _logger;
        

        public API(ILogger<API> logger, string baseURL = "")
        {
            _logger = logger;
            if (!string.IsNullOrWhiteSpace(baseURL))
                _baseURL = baseURL; 
            //Check if the key is empty and try to set it.
            //Fail fatally if not set to ensure error is well known
            //Only call environment variable once for performance
            if (string.IsNullOrWhiteSpace(_API_Key))
                try
                {
                    _API_Key = Environment.GetEnvironmentVariable(_envKey);
                    
                    if (_API_Key == null)
                        throw new ArgumentNullException(_envKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Must set environment variable \"" + _envKey + "\" with the API key for the CoinMarketCap API");
                    throw new Exception("Must set environment variable \"" + _envKey + "\" with the API key for the CoinMarketCap API",ex);
                }

            if (string.IsNullOrWhiteSpace(_API_Key))
            {

                _logger.LogError("Must set environment variable \"" + _envKey + "\" with the API key for the CoinMarketCap API");
                throw new Exception("No useful data was found under environment variable \"" + _envKey + "\"");
            }
            else if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Got key ending {_API_Key?[5]} from environment variable \"" + _envKey + "\"");


        }

        /// <summary>
        /// Calls the coinmarketcap API and returns the result as an instance of 
        /// CoinmarketcapListingsModel. 
        /// NB: Method throws to allow calling code to handle errors gracefully
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<CoinmarketcapListingsModel?> GetLatestListingsAsync(int start = 1, int limit = 5000, string convert = "USD")
        {
            //create a querystring
            //TODO: Expand feature to accept these as parameters
            var querystr = HttpUtility.ParseQueryString(string.Empty);
            querystr["start"] = start.ToString();
            querystr["limit"] = limit.ToString() ;
            querystr["convert"] = convert.ToString();
            var url = new UriBuilder(_baseURL);
            url.Query = querystr.ToString();

            //Creat the client that will do the request and set the headers as per documentation.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", _API_Key);
            client.DefaultRequestHeaders.Add("Accepts", "application/json");
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Calling URL {0} in GetLatestListingsAsync", url.ToString());

            //Though off http request.
            var resp = await client.GetAsync(url.ToString());

          

            //Handle API errors as per documentation and provide some useful
            //error messages for the logs or calling classes
            if (!resp.IsSuccessStatusCode)
            {
                string Error = String.Empty;
                var content = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Got error response {0} in GetLatestListingsAsync", content);

                switch ((int)resp.StatusCode)
                {
                    case 401:
                        {
                            Error = "Fatal - API Key not valid";
                            break;
                        }
                    case 402:
                        {
                            Error = "Fatal - Payment issues";
                            break;
                        }
                    case 403:
                        {
                            if (resp.Content != null)
                            {
                                if (content.Contains("1005"))
                                    Error = "Fatal - Key is required for this call";
                                if (content.Contains("1006"))
                                    Error = "Fatal - Endpoint not allowed";
                                if (content.Contains("1007"))
                                    Error = "Fatal - Key is disabled. Contact support";
                            }
                            break;
                        }
                    case 429:
                        {
                            if (resp.Content != null)
                            {
                                if (content.Contains("1008"))
                                    Error = "Minute Rate limit exceeded";
                                if (content.Contains("1009"))
                                    Error = "Fatal - Daily rate limit exceeded";
                                if (content.Contains("1010"))
                                    Error = "Fatal - Monthly rate limit exceeded";
                                if (content.Contains("1011"))
                                    Error = "IP Rate limit exceeded";
                            }
                            break;
                        }
                }
                if (Error == String.Empty)
                    Error = String.Format("An unknown error of code {0} with content {1} has occurred when calling {2}",
                        (int)resp.StatusCode,
                        resp.Content != null ? resp.Content.ReadAsStringAsync().GetAwaiter().GetResult() : "--No content",
                        url.ToString());

                _logger.LogError("Error {0} on processing response", Error);
                throw new Exception(Error);
            }
            else
            {
                //finally process the received data into object instances. 
                try
                {
                    var data = await resp.Content.ReadFromJsonAsync<CoinmarketcapListingsModel>();
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Got {0} records from API", data.data.Count());
                    return data;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to parse data");
                    //TODO: Add custom logic for known issues
                    throw;
                }
            }
        }





    }
}
