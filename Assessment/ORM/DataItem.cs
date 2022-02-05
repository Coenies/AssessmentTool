using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment.ORM
{
    public class DataItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Slug { get; set; }
        public int Cmc_Rank { get; set; }
        public int Num_Market_Pairs { get; set; }
        public int Circulating_Supply { get; set; }
        public int Total_Supply { get; set; }
        public int Max_Supply { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Last_Updated { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Date_Added { get; set; }
        public string[] Tags { get; set; }
        public object Platform { get; set; }
        public Fiat[] quotes { get; set; }
    }
}
