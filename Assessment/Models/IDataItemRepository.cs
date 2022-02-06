namespace Assessment.Models
{
    public interface IDataItemRepository
    {
        ORM.DataItem? GetById(int id);
        List<ORM.DataItem> GetByNewerThanSaveDate(DateTime dateTime);
        void DeleteById(int id);

        ORM.DataItem Update(ORM.DataItem dataItem);
        DateTime? GetLatestSavedDate();

        ORM.DataItem Insert(ORM.DataItem dataItem);
    }
}
