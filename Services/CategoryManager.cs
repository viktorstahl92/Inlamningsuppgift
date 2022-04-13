global using Inlamningsuppgift.Entities;
using Inlamningsuppgift.Data;

namespace Inlamningsuppgift.Services
{
    public class CategoryManager
    {
        private readonly DataContext _context;

        public CategoryManager(DataContext context)
        {
            _context = context;
        }

        //SKRIV OM DENNA FFS...
        public async Task<Category> GetOrCreateAsync(string categoryName)
        {
            var category = await _context.Categories.FindAsync(categoryName);

            if (category != null) return category;

            category = new Category { Name = categoryName };

            _context.Categories.Add(category);

            return category;
             
        }
    }
}
