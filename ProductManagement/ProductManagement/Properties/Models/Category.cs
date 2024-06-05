using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Properties.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }
    }
}
