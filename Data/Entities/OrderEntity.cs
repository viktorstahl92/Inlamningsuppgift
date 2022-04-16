using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inlamningsuppgift.Entities
{
    public class OrderEntity
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Address { get; set; } = null!;

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; } = null!;

        public virtual ICollection<OrderRowEntity> OrderRows { get; set; } = null!;
    }

    public class OrderRowEntity
    {
        [Key]
        public int OrderRowID { get; set; }
        
        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(10)]
        public int ProductNumber { get; set; }

        [Required]
        [StringLength(100)]

        public string ProductName { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal ProductPrice { get; set; }

        public OrderEntity Order { get; set; } = null!;
    }
}
