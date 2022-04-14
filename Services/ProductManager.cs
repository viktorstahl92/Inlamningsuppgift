using Inlamningsuppgift.Data;
using Inlamningsuppgift.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inlamningsuppgift.Services
{

    public interface IProductManager
    {
        Task<IActionResult> CreateProductAsync(NewProductModel newProduct);
        Task<IEnumerable<ProductInfo>> GetAllAsync();
    }

    public class ProductManager : IProductManager
    {
        private readonly DataContext _context;
        private readonly ICategoryManager _categoryManager;

        public ProductManager(DataContext context, ICategoryManager categoryManager)
        {
            _context = context;
            _categoryManager = categoryManager;
        }

        public async Task<IActionResult> CreateProductAsync(NewProductModel newProduct)
        {
            if (!await _context.Products.AnyAsync(x => x.ProductNumber == newProduct.ProductNumber))
            {
                var product = new ProductEntity
                {
                    ProductName = newProduct.ProductName,
                    ProductDescription = newProduct.ProductDescription,
                    ProductNumber = newProduct.ProductNumber,
                    ProductPrice = newProduct.ProductPrice,
                    CategoryId = (await _categoryManager.GetOrCreateAsync(newProduct.Category)).CategoryId
                };
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return new OkResult();
            }
            return new BadRequestResult();
        }
    
        public async Task<IEnumerable<ProductInfo>> GetAllAsync()
        {
            var items = new List<ProductInfo>();
            foreach ( var item in await _context.Products.Include(x=>x.Category).ToListAsync())
            {
                items.Add(new ProductInfo
                {
                    ProductDescription = item.ProductDescription,
                    ProductName = item.ProductName,
                    ProductNumber = item.ProductNumber,
                    ProductPrice = item.ProductPrice,
                    Category = item.Category.Name
                });
            }

            return items;
        }

    
    
    }

}
