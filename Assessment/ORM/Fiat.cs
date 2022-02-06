using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment.ORM
{
    /// <summary>
    /// Model class for string Fiat info
    /// </summary>
    public class Fiat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DataItemId { get; set; }
        public string CurrencyCode { get; set; }
        public double Price { get; set; }
        public double Volume_24h { get; set; }
        public double Volume_Change_24h { get; set; }
        public double Percent_Change_1h { get; set; }
        public float Percent_Change_24h { get; set; }
        public float Percent_Change_7d { get; set; }
        public double Market_Cap { get; set; }
        public float Market_Cap_Dominance { get; set; }
        public double Fully_Diluted_Market_Cap { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Last_updated { get; set; }
    }
}
