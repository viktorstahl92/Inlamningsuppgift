using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Services
{
    public interface IOrderManager
    {
        Task<IActionResult> CreateAsync(List<CartItem> cartItems, string email);
        Task<IEnumerable<OrderEntity>> GetAllAsync();
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

        public async Task<IEnumerable<OrderEntity>> GetAllAsync()
        {
            return await _context.Orders.Include(x => x.OrderRows).ToListAsync();
        }
    }
}
