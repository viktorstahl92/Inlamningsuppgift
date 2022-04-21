namespace Inlamningsuppgift.Data.Models
{
    public class OrderInfoModel
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public string OrderStatus { get; set; } = null!;
        public decimal TotalPrice => OrderRows.Sum(x => x.ProductPrice * x.Quantity);
        public List<OrderRowModel> OrderRows { get; set; } = new();

    }

    public class OrderRowModel
    {
        public int ProductNumber { get; set; }

        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
