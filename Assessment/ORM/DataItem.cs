using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment.ORM
{
    /// <summary>
    /// Main model class for storing data
    /// </summary>
    //This index makes queries much faster but uses up some storage
    [Index(nameof(RetrievedOn), IsUnique = false)]
    public class DataItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Slug { get; set; }
        public int Cmc_Rank { get; set; }
        public int Num_Market_Pairs { get; set; }
        public double Circulating_Supply { get; set; }
        public float? self_reported_circulating_supply { get; set; }
        public float? self_reported_market_cap { get; set; }
        public double Total_Supply { get; set; }
        public double? Max_Supply { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Last_Updated { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Date_Added { get; set; }
        public string? Tags { get; set; }
        
        public string? Platform { get; set; }
        public List<Fiat> quotes { get; set; } = new List<Fiat>();

        [DataType(DataType.DateTime)]
        public DateTime RetrievedOn { get; set; }
    }
}
