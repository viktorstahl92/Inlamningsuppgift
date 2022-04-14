using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inlamningsuppgift.Entities
{
    public class ProductEntity
    {
        [Key]
        public int ProductId { get; set; }
        [Required, StringLength(100)]
        public string ProductName { get; set; } = null!;

        [Required]
        public int ProductNumber { get; set; }
        
        [StringLength(1000)]
        public string ProductDescription { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal ProductPrice { get; set; }
        [Column("CategoryId")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Products")]
        public virtual CategoryEntity Category { get; set; }

    }
}
