global using Inlamningsuppgift.Entities;
using Inlamningsuppgift.Data;

namespace Inlamningsuppgift.Services
{

    public interface ICategoryManager
    {
        Task<Category> GetOrCreateAsync(string categoryName);
    }


    public class CategoryManager : ICategoryManager
    {
        private readonly DataContext _context;

        public CategoryManager(DataContext context)
        {
            _context = context;
        }

        public async Task<Category> GetOrCreateAsync(string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync<Category>(x=> x.Name == categoryName);

            if (category == null)
            {
                category = new Category { Name = categoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            return category;

        }
    }
}
