using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Services
{
    public interface IOrderManager
    {
        Task<IActionResult> CreateAsync(List<CartItem> cartItems, string email);
        Task<IEnumerable<OrderEntity>> GetAllAsync();
        Task<IActionResult> UpdateAsync(OrderInfoModel order, int id);
    }


    public class OrderManager : IOrderManager
    {
        private readonly DataContext _context;
        private readonly IUserManager _userManager;

        public OrderManager(DataContext context, IUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> CreateAsync(List<CartItem> cartItems, string email)
        {
            if (cartItems == null || cartItems.Count == 0) return new NotFoundObjectResult("Nothing in the cart");

            var user = await _userManager.GetUserByEmail(email);

            if (user == null) return new NotFoundObjectResult("User not found");


            var OrderEntity = new OrderEntity
            {
                CustomerName = $"{user.FirstName} {user.LastName}",
                CustomerId = user.UserId,
                Address = $"{user.Address}, {user.PostalCode} {user.City}",
                OrderDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                OrderStatus = "Received"
            };

            var OrderRows = new List<OrderRowEntity>();
            foreach (var cartItem in cartItems)
            {
                OrderRows.Add(new OrderRowEntity
                {
                    OrderId = OrderEntity.OrderId,
                    ProductNumber = cartItem.ProductNumber,
                    ProductName = cartItem.ProductName,
                    ProductPrice = cartItem.ProductPrice,
                    Quantity = cartItem.Quantity
                });
            }

            OrderEntity.OrderRows = OrderRows;

            _context.Orders.Add(OrderEntity);
            await _context.SaveChangesAsync();

            return new OkObjectResult(OrderEntity);

        }

        public async Task<IActionResult> UpdateAsync(OrderInfoModel order, int id)
        {
            if (order.OrderRows == null || order.OrderRows.Count == 0) return new NotFoundObjectResult("Nothing in the cart");

            var orderEnt = await _context.Orders.Include(x => x.OrderRows).FirstOrDefaultAsync(x => x.OrderId == id);
            if (orderEnt == null)
            {
                return new NotFoundObjectResult("No order found with specified ID.");
            }



            orderEnt.CustomerName = order.CustomerName;
            orderEnt.CustomerId = order.CustomerId;
            orderEnt.Address = order.Address;
            orderEnt.OrderDate = order.OrderDate; //TODO: Ska man kunna ändra OrderDate i efterhand?
            orderEnt.DueDate = order.DueDate;
            orderEnt.OrderStatus = order.OrderStatus;


            var OrderRows = new List<OrderRowEntity>(orderEnt.OrderRows);
            foreach (var cartItem in order.OrderRows)
            {
                var orderRow = OrderRows.FirstOrDefault(x=> x.ProductNumber == cartItem.ProductNumber);

                if (orderRow == null)
                {
                    OrderRows.Add(new OrderRowEntity
                    {
                        OrderId = orderEnt.OrderId,
                        ProductNumber = cartItem.ProductNumber,
                        ProductName = cartItem.ProductName,
                        ProductPrice = cartItem.ProductPrice,
                        Quantity = cartItem.Quantity
                    });
                }
                else
                {
                    orderRow.Quantity = cartItem.Quantity;
                    orderRow.ProductNumber = cartItem.ProductNumber;
                    orderRow.ProductName = cartItem.ProductName;
                    orderRow.ProductPrice = cartItem.ProductPrice;
                    _context.Entry(orderRow).State = EntityState.Modified;
                }
            }

            orderEnt.OrderRows = OrderRows;

            _context.Entry(orderEnt).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new OkObjectResult(orderEnt);

        }

        public async Task<IEnumerable<OrderEntity>> GetAllAsync()
        {
            return await _context.Orders.Include(x => x.OrderRows).ToListAsync();
        }
    }
}
