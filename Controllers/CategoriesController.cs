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
    public class CategoriesController : ControllerBase
    {

        private readonly ICategoryManager _categoryManager;

        public CategoriesController(DataContext context, ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }


        [HttpGet]
        public async Task<ActionResult> GetCategories() => new OkObjectResult(await _categoryManager.GetAllCategories());

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategory(int id) => (await _categoryManager.GetCategoryOnId(id));
    }
}
