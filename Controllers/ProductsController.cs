using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockFlowAPI.Data;
using StockFlowAPI.Models;
using System.Text;

namespace StockFlowAPI2._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StockFlowDbContext _context;

        public ProductsController(StockFlowDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int? companyId,
            [FromQuery] int? productTypeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "name",
            [FromQuery] string sortOrder = "asc")
        {
            var query = _context.Products
                .Include(p => p.Company)
                .Include(p => p.ProductType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower));
            }

            if (companyId.HasValue && companyId.Value > 0)
            {
                query = query.Where(p => p.CompanyId == companyId.Value);
            }

            if (productTypeId.HasValue && productTypeId.Value > 0)
            {
                query = query.Where(p => p.ProductTypeId == productTypeId.Value);
            }

            string cleanSortBy = (sortBy ?? "name").Trim().ToLower();
            bool isDesc = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

            query = cleanSortBy switch
            {
                "price" => isDesc ? query.OrderByDescending(p => (double)p.Price) : query.OrderBy(p => (double)p.Price),
                "stock" => isDesc ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
                "stockquantity" => isDesc ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
                _ => isDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            };

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    id = p.Id,
                    productName = p.Name,
                    price = p.Price,
                    stockQuantity = p.StockQuantity,
                    companyId = p.CompanyId,
                    companyName = p.Company != null ? p.Company.Name : "N/A",
                    productTypeId = p.ProductTypeId,
                    productTypeName = p.ProductType != null ? p.ProductType.TypeName : "N/A"
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                products
            });
        }

        // GET: api/Products/export-excel
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel(
            [FromQuery] string? search,
            [FromQuery] int? companyId,
            [FromQuery] int? productTypeId)
        {
            var query = _context.Products
                .Include(p => p.Company)
                .Include(p => p.ProductType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower));
            }

            if (companyId.HasValue && companyId.Value > 0)
            {
                query = query.Where(p => p.CompanyId == companyId.Value);
            }

            if (productTypeId.HasValue && productTypeId.Value > 0)
            {
                query = query.Where(p => p.ProductTypeId == productTypeId.Value);
            }

            var list = await query.ToListAsync();

            var builder = new StringBuilder();
            builder.Append('\uFEFF');
            builder.AppendLine("ID,Product Name,Company,Category,Stock Quantity,Price ($)");

            foreach (var p in list)
            {
                var compName = p.Company?.Name?.Replace("\"", "\"\"") ?? "N/A";
                var catName = p.ProductType?.TypeName?.Replace("\"", "\"\"") ?? "N/A";
                var prodName = p.Name.Replace("\"", "\"\"");

                builder.AppendLine($"{p.Id},\"{prodName}\",\"{compName}\",\"{catName}\",{p.StockQuantity},{p.Price:F2}");
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return File(bytes, "text/csv", $"StockFlow_Inventory_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest(new { message = "Product name is required." });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var transaction = new StockTransaction
            {
                ProductId = product.Id,
                Quantity = product.StockQuantity,
                TransactionType = "Stock In (Creation)",
                Description = $"Product '{product.Name}' added to inventory.",
                TransactionDate = DateTime.UtcNow
            };
            _context.StockTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            int oldStock = existingProduct?.StockQuantity ?? 0;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                int stockDiff = product.StockQuantity - oldStock;
                if (stockDiff != 0)
                {
                    var transaction = new StockTransaction
                    {
                        ProductId = product.Id,
                        Quantity = Math.Abs(stockDiff),
                        TransactionType = stockDiff > 0 ? "Stock Adjustment (+)" : "Stock Adjustment (-)",
                        Description = $"Stock updated for '{product.Name}'. Old: {oldStock}, New: {product.StockQuantity}",
                        TransactionDate = DateTime.UtcNow
                    };
                    _context.StockTransactions.Add(transaction);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var transaction = new StockTransaction
            {
                ProductId = null,
                Quantity = product.StockQuantity,
                TransactionType = "Stock Out (Deletion)",
                Description = $"Product '{product.Name}' deleted from inventory.",
                TransactionDate = DateTime.UtcNow
            };
            _context.StockTransactions.Add(transaction);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}