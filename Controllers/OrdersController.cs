using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class OrdersController : ControllerBase
    {
        private readonly IOrderManager _orderManager;

        public OrdersController(IOrderManager orderManager)
        {
            _orderManager = orderManager;
        }


        [HttpPost]
        public async Task<IActionResult> PostOrder(List<CartItem> cartItems)
        {
            string userEmail = "";
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                userEmail = identity.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("User not authorized");
            }

            return await _orderManager.CreateAsync(cartItems, userEmail);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() => new OkObjectResult(await _orderManager.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByID(int id) => await _orderManager.GetByIdAsync(id);

        [HttpGet("LoggedInOrders")]
        public async Task<IActionResult> GetLoggedInOrders()
        {
            int userID;
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                userID = int.Parse(identity.FindFirst("UserId").Value);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("User not authorized");
            }

            return await _orderManager.GetByCustomerIdAsync(userID);

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrder(OrderInfoModel order, int id) => await _orderManager.UpdateAsync(order, id);

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id) => await _orderManager.Delete(id);
    }
}
