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
        private static string baseURL = "https://sandbox-api/coinmarketcap.com/v1/cryptopcurrency/listings/latest";


        public API()
        {
            //Check if the key is empty and try to set it.
            //Fail fatally if not set to ensure error is well known
            //Only call environment variable once for performance
            if (API_Key == string.Empty)
                try
                {
                    API_Key = Environment.GetEnvironmentVariable(envKey);

                }
                catch (Exception)
                {
                    throw new Exception("Must set environment variable \"" + envKey + "\" with the API key for the CoinMarketCap API");
                }
            if (string.IsNullOrWhiteSpace(API_Key))
                throw new Exception("No useful data was found under environment variable \"" + envKey + "\"");

        }


        public async void MakeAPICall()
        {
            //create a querystring
            var querystr = HttpUtility.ParseQueryString(string.Empty);
            querystr["start"] = "1";
            querystr["limit"] = "5000";
            querystr["convert"] = "USD";

            var url = new UriBuilder(baseURL);
            url.Query = querystr.ToString();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", API_Key);
            client.DefaultRequestHeaders.Add("Accepts", "application/json");

            var resp = await client.GetAsync(url.ToString());

            //Handle API errors as per documentation and provide some useful
            //error messages for the logs
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


                throw new Exception(Error);


            }
            else
            {

            }
        }





    }
}
