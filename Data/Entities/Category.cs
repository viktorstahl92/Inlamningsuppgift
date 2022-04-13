using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inlamningsuppgift.Entities
{
    public class Category
    {

        public Category()
        {
            Products = new List<Product>();
        }

        [Key]
        public int CategoryId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [InverseProperty("Category")]
        public virtual ICollection<Product> Products { get; set; }

    }
}
