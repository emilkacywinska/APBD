using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Properties.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }
        public List<ShoppingCart> ShoppingCart { get; set; }
    }
}
