using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment.ORM
{
    public class Fiat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DataItemId { get; set; }
        public string CurrencyCode { get; set; }
        public float Price { get; set; }
        public int Volume_24h { get; set; }
        public float Volume_Change_24h { get; set; }
        public float Percent_Change_1h { get; set; }
        public float Percent_Change_24h { get; set; }
        public float Percent_Change_7d { get; set; }
        public float Market_Cap { get; set; }
        public int Market_Cap_Dominance { get; set; }
        public float Fully_Diluted_Market_Cap { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Last_updated { get; set; }
    }
}
