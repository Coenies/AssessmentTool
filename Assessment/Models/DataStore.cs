using Assessment.Models.CoinmarketCap;
using Assessment.ORM;

namespace Assessment.Models
{
    public class DataStore
    {
        private readonly EFModel _dbcontext;

        public DataStore(ORM.EFModel dbcontext)
        {
            this._dbcontext = dbcontext;
        }

        /// <summary>
        /// Gets the newest item by Last_Updated date
        /// </summary>
        /// <returns>ORM.DataItem</returns>
        public ORM.DataItem? GetLastDataItem()
        {
            var result = _dbcontext.DataItems.OrderByDescending(b => b.Last_Updated).Take(1).FirstOrDefault();
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
            var dataItem = new DataItem();

            foreach (var datumProp in datum.GetType().GetProperties())
            {
                foreach (var itemProp in dataItem.GetType().GetProperties())
                    if (datumProp.Name.ToLower() == itemProp.Name.ToLower() && datumProp.Name.ToLower() != "id")
                    {
                        itemProp.SetValue(dataItem, datumProp.GetValue(datum, null));
                    }
            }

            //translate the various currency quote values to their database equivalent
            //This is done using reflection so that additional currencies added to
            //the array in quotes will also be added automatically (DRY)
            foreach (var quoteProp in datum.quote.GetType().GetProperties()) {
                var quotevaue = (CoinmarketCap.Fiat?)quoteProp.GetValue(datum.quote);
                if (quotevaue != null)
                {
                    var newQuote = TranslateToORMFiat(quotevaue);
                    newQuote.CurrencyCode = quoteProp.Name;
                    dataItem.quotes.Append(newQuote);
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
    }
}
