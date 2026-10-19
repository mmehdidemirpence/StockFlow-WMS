namespace StockFlowAPI.DTOs
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public int CompanyId { get; set; }
        public int ProductTypeId { get; set; }
    }
}