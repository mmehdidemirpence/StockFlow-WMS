using System;
using System.ComponentModel.DataAnnotations;

namespace StockFlowAPI.Models
{
    public class StockTransaction
    {
        public int Id { get; set; }
        
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }

        [Required, MaxLength(50)]
        public string TransactionType { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}