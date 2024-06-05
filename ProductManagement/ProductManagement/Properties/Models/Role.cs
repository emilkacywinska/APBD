using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Properties.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        
        public List<Account> Accounts { get; set; }
    }
}
