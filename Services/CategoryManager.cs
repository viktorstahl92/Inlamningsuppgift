global using Inlamningsuppgift.Entities;
using Inlamningsuppgift.Data;

namespace Inlamningsuppgift.Services
{

    public interface ICategoryManager
    {
        Task CheckIfDeleteCategory(int id);
        Task<IEnumerable<CategoryEntity>> GetAllCategories();
        Task<CategoryEntity> GetOrCreateAsync(string categoryName);
    }


    public class CategoryManager : ICategoryManager
    {
        private readonly DataContext _context;

        public CategoryManager(DataContext context)
        {
            _context = context;
        }

        public async Task<CategoryEntity> GetOrCreateAsync(string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync<CategoryEntity>(x=> x.Name == categoryName);

            if (category == null)
            {
                category = new CategoryEntity { Name = categoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            return category;

        }

        public async Task CheckIfDeleteCategory(int id)
        {
            var Category = await _context.Categories.Include(x=>x.Products).Where(x=> x.CategoryId == id).FirstOrDefaultAsync();
            if (Category != null && Category.Products.Count == 0)
            {
                _context.Categories.Remove(Category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CategoryEntity>> GetAllCategories() => await _context.Categories.Include(x => x.Products).ToListAsync();
    }
}
