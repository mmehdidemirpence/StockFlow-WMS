using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockFlowAPI.Data;
using StockFlowAPI.Models;

namespace StockFlowAPI2._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : ControllerBase
    {
        private readonly StockFlowDbContext _context;

        public ProductTypesController(StockFlowDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductTypes
        [HttpGet]
        public async Task<IActionResult> GetProductTypes()
        {
            var productTypes = await _context.ProductTypes.ToListAsync();
            return Ok(productTypes);
        }

        // GET: api/ProductTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductType(int id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null) return NotFound();
            return Ok(productType);
        }

        // POST: api/ProductTypes
        [HttpPost]
        public async Task<IActionResult> PostProductType([FromBody] ProductType productType)
        {
            if (productType == null || string.IsNullOrWhiteSpace(productType.TypeName))
            {
                return BadRequest(new { message = "Product type name is required." });
            }

            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();

            return Ok(productType);
        }

        // PUT: api/ProductTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductType(int id, [FromBody] ProductType productType)
        {
            if (id != productType.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            _context.Entry(productType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ProductTypes.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/ProductTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            var relatedProducts = _context.Products.Where(p => p.ProductTypeId == id);
            _context.Products.RemoveRange(relatedProducts);

            _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}