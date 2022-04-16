namespace Inlamningsuppgift.Data.Models
{
    public class CartItem
    {
        public int ProductNumber { get; set; }

        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal ProductPrice { get; set; }

    }
}
