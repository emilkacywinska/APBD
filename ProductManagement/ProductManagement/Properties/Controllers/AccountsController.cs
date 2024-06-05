using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Properties.Data;
using ProductManagement.Properties.Models;

namespace ProductManagement.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{accountId:int}")]
        public async Task<IActionResult> GetAccount(int accountId)
        {
            var account = await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.ShoppingCart)
                .ThenInclude(sc => sc.Product)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
                return NotFound();

            return Ok(new
            {
                firstName = account.FirstName,
                lastName = account.LastName,
                email = account.Email,
                phone = account.Phone,
                role = account.Role.Name,
                cart = account.ShoppingCart.Select(sc => new
                {
                    productId = sc.Product.Id,
                    productName = sc.Product.Name,
                    amount = sc.Amount
                })
            });
        }
    }
}
