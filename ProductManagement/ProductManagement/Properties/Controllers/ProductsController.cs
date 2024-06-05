using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Properties.Data;
using ProductManagement.Properties.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                Name = productDto.ProductName,
                Weight = productDto.ProductWeight,
                Width = productDto.ProductWidth,
                Height = productDto.ProductHeight,
                Depth = productDto.ProductDepth
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            foreach (var categoryId in productDto.ProductCategories)
            {
                _context.ProductsCategories.Add(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = categoryId
                });
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(new
            {
                product.Id,
                product.Name,
                product.Weight,
                product.Width,
                product.Height,
                product.Depth,
                Categories = product.ProductCategories.Select(pc => new
                {
                    pc.CategoryId,
                    pc.Category.Name
                }).ToList()
            });
        }
    }

    public class ProductDto
    {
        public string ProductName { get; set; }
        public decimal ProductWeight { get; set; }
        public decimal ProductWidth { get; set; }
        public decimal ProductHeight { get; set; }
        public decimal ProductDepth { get; set; }
        public List<int> ProductCategories { get; set; }
    }
}
