using Assessment.ORM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assessment.Models
{
    /// <summary>
    /// Mock class for testing of DataStore
    /// </summary>
    internal class DateItemRepostoryInMemory : Assessment.Models.IDataItemRepository
    {
        private List<Assessment.ORM.DataItem> _data = new List<Assessment.ORM.DataItem>();

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public DataItem? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<DataItem> GetByNewerThanSaveDate(DateTime dateTime)
        {
            return _data.Where(c => c.RetrievedOn >= dateTime).ToList();
        }

        public DateTime? GetLatestSavedDate()
        {
            return _data.Max(c => (DateTime?)c.RetrievedOn);
        }

        public DataItem Insert(DataItem dataItem)
        {
            _data.Add(dataItem);
            return dataItem;
        }

        public DataItem Update(DataItem dataItem)
        {
            var item = _data.Where(c => c.Id != dataItem.Id).FirstOrDefault();
            if (item != null)
                item = dataItem;
            else
                _data.Add(dataItem);
            return dataItem;
        }

      
    }
}