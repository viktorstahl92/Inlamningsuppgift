using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inlamningsuppgift.Entities
{
    public class CategoryEntity
    {

        public CategoryEntity()
        {
            Products = new List<ProductEntity>();
        }

        [Key]
        public int CategoryId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [InverseProperty("Category")]
        public virtual ICollection<ProductEntity> Products { get; set; }

    }
}
