namespace Inlamningsuppgift.Data.Models
{
    public class NewProductModel
    {
        public string ProductName { get; set; } = null!;
        public int ProductNumber { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public string Category { get; set; } = null!;

    }
}
