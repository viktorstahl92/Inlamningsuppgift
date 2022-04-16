#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManager _productManager;

        public ProductsController(DataContext context, IProductManager productManager)
        {
            _productManager = productManager;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductInfo>>> GetProducts() => new OkObjectResult(await _productManager.GetAllAsync());

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id) => await _productManager.GetByIdAsync(id);

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, NewProductModel product) => await _productManager.UpdateAsync(id, product);

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProduct(NewProductModel product) => await _productManager.CreateProductAsync(product);

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id) => await _productManager.DeleteByID(id);
    }
}
