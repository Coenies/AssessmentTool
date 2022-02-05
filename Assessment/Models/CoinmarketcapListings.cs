using System.ComponentModel.DataAnnotations;

namespace Assessment.Models.CoinmarketCap
{
    /// <summary>
    /// This class is solely to de-serialize the JSON response date from coinmarketcap response 
    /// at /v1/cryptocurrency/listings/latest and is a JSON paste. 
    /// </summary>
    public class CoinmarketcapListingsModel
    {
        public Status status { get; set; }
        public Datum[] data { get; set; }
    }

    public class Status
    {
        [DataType(DataType.DateTime)]
        public DateTime timestamp { get; set; }
        public int error_code { get; set; }
        public string error_message { get; set; }
        public int elapsed { get; set; }
        public int credit_count { get; set; }
        public object notice { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string slug { get; set; }
        public int cmc_rank { get; set; }
        public int num_market_pairs { get; set; }
        public int circulating_supply { get; set; }
        public int total_supply { get; set; }
        public int max_supply { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime last_updated { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime date_added { get; set; }
        public string[] tags { get; set; }
        public object platform { get; set; }
        public Quote quote { get; set; }
    }

    public class Quote
    {
        public Fiat USD { get; set; }
        public Fiat ZAR { get; set; }
        public Fiat EUR { get; set; }
        public Fiat GBP { get; set; }
    }

    public class Fiat
    {
        public float price { get; set; }
        public int volume_24h { get; set; }
        public float volume_change_24h { get; set; }
        public float percent_change_1h { get; set; }
        public float percent_change_24h { get; set; }
        public float percent_change_7d { get; set; }
        public float market_cap { get; set; }
        public int market_cap_dominance { get; set; }
        public float fully_diluted_market_cap { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime last_updated { get; set; }
    }

}

