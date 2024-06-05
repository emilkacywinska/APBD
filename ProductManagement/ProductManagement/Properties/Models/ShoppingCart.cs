
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Properties.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }

        public Account Account { get; set; }
        public Product Product { get; set; }
    }
}
