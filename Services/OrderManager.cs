using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Services
{
    public interface IOrderManager
    {
        Task<IActionResult> CreateAsync(List<CartItem> cartItems, string email);
        Task<IActionResult> Delete(int id);
        Task<IEnumerable<OrderEntity>> GetAllAsync();
        Task<ActionResult> GetByCustomerIdAsync(int id);
        Task<ActionResult> GetByIdAsync(int id);
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

        public async Task<IEnumerable<OrderEntity>> GetAllAsync() => await _context.Orders.Include(x => x.OrderRows).ToListAsync();

        public async Task<ActionResult> GetByIdAsync(int id)
        {
            var order = await _context.Orders.Include(x => x.OrderRows).FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null) return new NotFoundResult();

            var orderInfo = new OrderInfoModel
            {
                Address = order.Address,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                DueDate = order.DueDate,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,    
               
            };

            foreach (var item in order.OrderRows)
            {
                var newRow = new OrderRowModel
                {
                    ProductName = item.ProductName,
                    ProductNumber = item.ProductNumber,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity
                };

                orderInfo.OrderRows.Add(newRow);
            }


            return new OkObjectResult(orderInfo);
        }

        public async Task<ActionResult> GetByCustomerIdAsync(int id)
        {
            var orders = await _context.Orders.Include(x => x.OrderRows).Where(x => x.CustomerId == id).ToListAsync();

            if (orders == null) return new NotFoundResult();

            List<OrderInfoModel> ordersInfo = new List<OrderInfoModel>();   

            foreach (var order in orders)
            {
                var orderInfo = new OrderInfoModel
                {
                    Address = order.Address,
                    CustomerId = order.CustomerId,
                    CustomerName = order.CustomerName,
                    DueDate = order.DueDate,
                    OrderDate = order.OrderDate,
                    OrderStatus = order.OrderStatus,

                };

                foreach (var item in order.OrderRows)
                {
                    var newRow = new OrderRowModel
                    {
                        ProductName = item.ProductName,
                        ProductNumber = item.ProductNumber,
                        ProductPrice = item.ProductPrice,
                        Quantity = item.Quantity
                    };

                    orderInfo.OrderRows.Add(newRow);
                }

                ordersInfo.Add(orderInfo);
            }
           


            return new OkObjectResult(ordersInfo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.Include(x=>x.OrderRows).FirstOrDefaultAsync(x=> x.OrderId == id);
            if (order == null)
            {
                return new NotFoundResult();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
