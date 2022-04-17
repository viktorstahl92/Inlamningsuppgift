#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inlamningsuppgift.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UseApiKey]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManager _productManager;

        public ProductsController(DataContext context, IProductManager productManager)
        {
            _productManager = productManager;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductInfo>>> GetProducts() => new OkObjectResult(await _productManager.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id) => await _productManager.GetByIdAsync(id);

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PutProduct(int id, NewProductModel product) => await _productManager.UpdateAsync(id, product);

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostProduct(NewProductModel product) => await _productManager.CreateProductAsync(product);

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id) => await _productManager.DeleteByID(id);
    }
}
