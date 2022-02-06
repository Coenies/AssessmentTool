using Assessment.CoinMarketCapAPI;
using Assessment.Models.CoinmarketCap;
using Assessment.ORM;
using System.Text.Json;

namespace Assessment.Models
{
    public class DataStore
    {
        private readonly IDataItemRepository _dataItemRepository;
        private readonly API _cmcApi;
        private readonly object _locker = new object();

        public DataStore(IDataItemRepository dataItemRepository, CoinMarketCapAPI.API cmcApi)
        {
            this._dataItemRepository = dataItemRepository;
            _cmcApi = cmcApi;
        }

        /// <summary>
        /// Gets the newest item by RetrievedOn date
        /// If no items are found that are less then 10 minutes old call api for latest results. 
        /// If that call fails go back 10 more minutes and try the function again up to 10 times
        /// </summary>
        /// <returns>ORM.DataItem</returns>
        public List<DataItem> GetLastDataItem(int MinutesBack = -10, bool DontBranch = false, int LimitResults = 5000)
        {
            //we don't want race conditions here duplicating data on the database or wasting API calls. 
            lock (_locker)
            {
                var ttlExpire = DateTime.UtcNow.AddMinutes(MinutesBack);
                var result = _dataItemRepository.GetByNewerThanSaveDate(ttlExpire);
                bool PermanetFailure = false;

                if (result.Count() == 0 && !DontBranch)
                {
                    int counter = 0;
                    while (counter < 10 && result.Count() == 0 && !PermanetFailure)
                    {

                        try
                        {
                            result = GetLatestMarketCapData(LimitResults).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Fatal -")) PermanetFailure = true;
                        }
                        if (PermanetFailure)
                        {

                            //So we are now unable to use the API. lets get the very last bit of data we have in the DB and hope that someone 
                            //is looking at the logs and fixes the issue as there is nothing more we can do here. 
                            var LastEverDate = _dataItemRepository.GetLatestSavedDate();
                            if (LastEverDate != null)
                            {
                                result = _dataItemRepository.GetByNewerThanSaveDate(LastEverDate.Value);
                            }
                        }
                        //There was a transient failure with the API service. Lets assume that it might last longer than this loop.
                        //So lets see if we can grab some slightly older data instead. The time this takes might be better use of resource
                        //than putting the thread to sleep before we try again.
                        else if (result.Count() == 0)
                        {
                            ttlExpire.AddMinutes(-10);
                            result = _dataItemRepository.GetByNewerThanSaveDate(ttlExpire);

                            if (result.Count() == 0)     //Okay so that was a waste of time. Lets sleep a bit and give the API or internet a bit of time to recover. 
                                System.Threading.Thread.Sleep(2000);
                        }

                    }
                }
                return result;
            }
        }


        /// <summary>
        /// This gets the latest data from the coin market cap api. 
        /// </summary>
        /// <returns></returns>
        internal async Task<List<DataItem>> GetLatestMarketCapData(int LimitResults)
        {
            var result = new List<DataItem>();
            var apiResults = await GetLatestFromCMCAPIAsync(LimitResults);
            if (apiResults != null)
                foreach (var apiResult in apiResults)
                {
                    var dataItem = TranslateToORM(apiResult);
                    _dataItemRepository.Insert(dataItem);
                    result.Add(dataItem);
                }
            return result;
        }

        /// <summary>
        /// Transform datum object into a ORM.DataItem object using reflection
        /// so that it can be saved into the database with a slightly different schema 
        /// </summary>
        /// <param name="datum"></param>
        /// <returns></returns>

        public ORM.DataItem TranslateToORM(Datum datum)
        {
            //Transform datum object into a ORM.DataItem object using reflection
            //Set the retrieved on date to use as a key for getting the latest data
            var dataItem = new DataItem() { RetrievedOn = DateTime.UtcNow };

            foreach (var datumProp in datum.GetType().GetProperties())
            {
                foreach (var itemProp in dataItem.GetType().GetProperties())
                    if (datumProp.Name.ToLower() == itemProp.Name.ToLower() && datumProp.Name.ToLower() != "id")
                    {
                        if (datumProp.Name.ToLower() == "tags" && datum.tags != null)
                        {
                            dataItem.Tags = String.Join(",", datum.tags);
                        }
                        else if (datumProp.Name.ToLower() == "platform" && datum.platform != null)
                        {
                            dataItem.Platform = JsonSerializer.Serialize(datum.platform);
                        }
                        else
                        {
                            itemProp.SetValue(dataItem, datumProp.GetValue(datum, null));
                        }
                    }
            }

            //translate the various currency quote values to their database equivalent
            //This is done using reflection so that additional currencies added to
            //the array in quotes will also be added automatically (DRY)
            foreach (var quoteProp in datum.quote.GetType().GetProperties())
            {
                var quotevaue = (CoinmarketCap.Fiat?)quoteProp.GetValue(datum.quote);
                if (quotevaue != null)
                {
                    var newQuote = TranslateToORMFiat(quotevaue);
                    newQuote.CurrencyCode = quoteProp.Name;
                    dataItem.quotes.Add(newQuote);
                }
            }
            return dataItem;
        }


        /// <summary>
        /// Translates the sub array of quotes to their respective ORM counterparts
        /// </summary>
        /// <param name="fiat"></param>
        /// <returns>ORM.Fiat</returns>
        internal ORM.Fiat TranslateToORMFiat(CoinmarketCap.Fiat fiat)
        {
            var fiatItem = new ORM.Fiat();

            foreach (var datumProp in fiat.GetType().GetProperties())
            {
                foreach (var itemProp in fiatItem.GetType().GetProperties())
                    if (datumProp.Name.ToLower() == itemProp.Name.ToLower() && datumProp.Name.ToLower() != "id")
                    {
                        itemProp.SetValue(fiatItem, datumProp.GetValue(fiat, null));
                    }
            }
            return fiatItem;
        }

        internal async Task<Datum[]?> GetLatestFromCMCAPIAsync(int LimitResults)
        {
            var data = await _cmcApi.GetLatestListingsAsync(limit: LimitResults);
            if (data != null)
            {
                return data.data;
            }
            return null;
        }
    }
}
