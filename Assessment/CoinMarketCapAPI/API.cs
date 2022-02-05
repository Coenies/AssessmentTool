using Assessment.Models.CoinmarketCap;
using Assessment.ORM;
using System.Net;
using System.Web;

namespace Assessment.CoinMarketCapAPI
{
    public class API
    {
        //Static variable to hold API key
        private static string API_Key = string.Empty;

        //Set the environment key name
        private const string envKey = "CoinMarkKey";
        //static base URL variable. 
        private const string baseURL = "https://sandbox-api/coinmarketcap.com/v1/cryptopcurrency/listings/latest";

        private readonly ILogger<API> _logger;
        

        public API(ILogger<API> logger)
        {
            _logger = logger;
           
            //Check if the key is empty and try to set it.
            //Fail fatally if not set to ensure error is well known
            //Only call environment variable once for performance
            if (string.IsNullOrWhiteSpace(API_Key))
                try
                {
                    API_Key = Environment.GetEnvironmentVariable(envKey);
                    if (API_Key == null)
                        throw new ArgumentNullException(envKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Must set environment variable \"" + envKey + "\" with the API key for the CoinMarketCap API");
                    throw new Exception("Must set environment variable \"" + envKey + "\" with the API key for the CoinMarketCap API",ex);
                }

            if (string.IsNullOrWhiteSpace(API_Key))
            {

                _logger.LogError("Must set environment variable \"" + envKey + "\" with the API key for the CoinMarketCap API");
                throw new Exception("No useful data was found under environment variable \"" + envKey + "\"");
            }
            else if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Got key ending {API_Key?[..-5]} from environment variable \"" + envKey + "\"");


        }

        /// <summary>
        /// Calls the coinmarketcap API and returns the result as an instance of 
        /// CoinmarketcapListingsModel. 
        /// NB: Method throws to allow calling code to handle errors gracefully
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<CoinmarketcapListingsModel?> GetLatestListingsAsync()
        {
            //create a querystring
            //TODO: Expand feature to accept these as parameters
            var querystr = HttpUtility.ParseQueryString(string.Empty);
            querystr["start"] = "1";
            querystr["limit"] = "5000";
            querystr["convert"] = "USD";
            var url = new UriBuilder(baseURL);
            url.Query = querystr.ToString();

            //Creat the client that will do the request and set the headers as per documentation.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", API_Key);
            client.DefaultRequestHeaders.Add("Accepts", "application/json");
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Calling URL {0} in GetLatestListingsAsync", url.ToString());

            //Though off http request.
            var resp = await client.GetAsync(url.ToString());

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Got response {0} in GetLatestListingsAsync", resp.Content.ToString());

            //Handle API errors as per documentation and provide some useful
            //error messages for the logs or calling classes
            if (!resp.IsSuccessStatusCode)
            {
                string Error = String.Empty;
                switch ((int)resp.StatusCode)
                {
                    case 401:
                        {
                            Error = "API Key not valid";
                            break;
                        }
                    case 402:
                        {
                            Error = "Payment issues";
                            break;
                        }
                    case 403:
                        {
                            if (resp.Content != null)
                            {
                                if (resp.Content.ToString().Contains("1005"))
                                    Error = "Key is required for this call";
                                if (resp.Content.ToString().Contains("1006"))
                                    Error = "Endpoint not allowed";
                                if (resp.Content.ToString().Contains("1007"))
                                    Error = "Key is disabled. Contact support";
                            }
                            break;
                        }
                    case 429:
                        {
                            if (resp.Content != null)
                            {
                                if (resp.Content.ToString().Contains("1008"))
                                    Error = "Minute Rate limit exceeded";
                                if (resp.Content.ToString().Contains("1009"))
                                    Error = "Daily rate limit exceeded";
                                if (resp.Content.ToString().Contains("1010"))
                                    Error = "Monthly rate limit exceeded";
                                if (resp.Content.ToString().Contains("1011"))
                                    Error = "IP Rate limit exceeded";
                            }
                            break;
                        }
                }
                if (Error == String.Empty)
                    Error = String.Format("An unknown error of code {0} with content {1} has occurred",
                        (int)resp.StatusCode,
                        resp.Content != null ? resp.Content.ToString() : "--No content");

                _logger.LogError("Error {0} on processing response", Error);
                throw new Exception(Error);
            }
            else
            {
                //finally process the received data into object instances. 
                try
                {
                    var data = await resp.Content.ReadFromJsonAsync<CoinmarketcapListingsModel>();
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
