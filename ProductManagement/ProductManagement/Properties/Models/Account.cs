using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Properties.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Role Role { get; set; }
        public List<ShoppingCart> ShoppingCart { get; set; }
    }
}
