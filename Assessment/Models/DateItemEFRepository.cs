using Assessment.ORM;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Models
{
    public class DateItemEFRepository : IDataItemRepository
    {
        private readonly EFModel datacontext;

        public DateItemEFRepository(ORM.EFModel datacontext)
        {
            this.datacontext = datacontext;
        }

        /// <summary>
        /// Deletes an item, if found, from the database
        /// </summary>
        /// <param name="id"></param>
        public void DeleteById(int id)
        {
            var item = datacontext.DataItems.Where(c => c.Id == id).FirstOrDefault();
            if (item != null) { 
                datacontext.DataItems.Remove(item);
                datacontext.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves the item from the database based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataItem? GetById(int id)
        {
            var item = datacontext.DataItems.Where(c => c.Id == id).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Retrieves a collection of items where date is after the parameter dateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public List<DataItem> GetByNewerThanSaveDate(DateTime dateTime)
        {
            var item = datacontext.DataItems.Where(c => c.RetrievedOn >= dateTime).ToList();
            return item;
        }


        /// <summary>
        /// Retrieves the last received on date from the database
        /// </summary>
        /// <returns></returns>
        public DateTime? GetLatestSavedDate()
        {
           var maxDate = datacontext.DataItems.Max(c => (DateTime?)c.RetrievedOn);
            return maxDate; 
        }

        /// <summary>
        /// Updates the record to the database
        /// Also adds it if it is a new record
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        public DataItem Update(DataItem dataItem)
        {

            var item = datacontext.DataItems.Where(c => c.Id == dataItem.Id).FirstOrDefault();
            if (item != null)          //Update item
            {
                datacontext.DataItems.Attach(dataItem);
                datacontext.Entry(dataItem).State = EntityState.Modified;
            }
            else //Create new
            {
                datacontext.DataItems.Add(dataItem);
            }
            datacontext.SaveChanges();
            return dataItem;
        }

        /// <summary>
        /// Insert record into db. 
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>

        public DataItem Insert(DataItem dataItem)
        {
            //this is not very performant. But its the norm. 
            //to make it much faster turn of change tracking and do a detect changes before save once all has been added. But I don’t want to overcomplicate this
            datacontext.DataItems.Add(dataItem);
            datacontext.SaveChanges();
            return dataItem;
        }
    }
}
