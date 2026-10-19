using System.ComponentModel.DataAnnotations;

namespace StockFlowAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        public int ProductTypeId { get; set; }
        public ProductType? ProductType { get; set; }

        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    }
}