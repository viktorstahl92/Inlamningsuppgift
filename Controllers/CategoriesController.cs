using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inlamningsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly DataContext _context;
        private readonly ICategoryManager _categoryManager;

        public CategoriesController(DataContext context, ICategoryManager categoryManager)
        {
            _context = context;
            _categoryManager = categoryManager;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductInfo>>> GetCategories() => new OkObjectResult(await _categoryManager.GetAllCategories());

        [HttpGet("{id}")]

        public async Task<ActionResult<IEnumerable<ProductInfo>>> GetCategory(int id) => new(await _categoryManager.GetCategoryOnId(id));
    }
}
