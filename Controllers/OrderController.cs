﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class OrderController : ControllerBase
    {
        private readonly IOrderManager _orderManager;

        public OrderController(IOrderManager orderManager)
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
    }
}
