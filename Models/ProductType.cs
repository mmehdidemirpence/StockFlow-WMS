using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StockFlowAPI.Models
{
    public class ProductType
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}