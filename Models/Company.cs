using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Bunu ekle

namespace StockFlowAPI.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string ContactInfo { get; set; } = string.Empty;

        [JsonIgnore] // Sonsuz döngü ve hatayı önlemek için kritik
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}