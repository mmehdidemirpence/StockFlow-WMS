using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockFlowAPI.Data;
using StockFlowAPI.Models;

namespace StockFlowAPI2._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransactionsController : ControllerBase
    {
        private readonly StockFlowDbContext _context;

        public StockTransactionsController(StockFlowDbContext context)
        {
            _context = context;
        }

        // GET: api/StockTransactions
        [HttpGet]
        public async Task<IActionResult> GetStockTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.StockTransactions
                .Include(t => t.Product)
                .OrderByDescending(t => t.TransactionDate);

            var totalCount = await query.CountAsync();

            var rawTransactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var transactions = rawTransactions.Select(t => new
            {
                id = t.Id,
                productName = t.Product != null 
                    ? t.Product.Name 
                    : (t.Description.Contains("'") ? t.Description.Split('\'')[1] : "Deleted Product"),
                quantity = t.Quantity,
                transactionType = t.TransactionType,
                description = t.Description,
                transactionDate = t.TransactionDate
            }).ToList();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                transactions
            });
        }

        // DELETE: api/StockTransactions/clear-before?date=2026-10-01
        [HttpDelete("clear-before")]
        public async Task<IActionResult> ClearTransactionsBefore([FromQuery] DateTime date)
        {
            var oldTransactions = _context.StockTransactions
                .Where(t => t.TransactionDate < date);

            _context.StockTransactions.RemoveRange(oldTransactions);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logs older than the specified date have been deleted successfully." });
        }
    }
}