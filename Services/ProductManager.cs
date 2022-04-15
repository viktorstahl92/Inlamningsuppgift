using Inlamningsuppgift.Data;
using Inlamningsuppgift.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inlamningsuppgift.Services
{

    public interface IProductManager
    {
        Task<IActionResult> CreateProductAsync(NewProductModel newProduct);
        Task<ActionResult> DeleteByID(int id);
        Task<IEnumerable<ProductInfo>> GetAllAsync();
        Task<ActionResult> GetByIdAsync(int id);
        Task<ActionResult> UpdateAsync(int id, NewProductModel form);
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
                return new OkObjectResult(product);
            }
            return new BadRequestObjectResult("Product number already exists");
        }

        public async Task<IEnumerable<ProductInfo>> GetAllAsync()
        {
            var items = new List<ProductInfo>();
            foreach (var item in await _context.Products.Include(x => x.Category).ToListAsync())
            {
                items.Add(new ProductInfo
                {
                    ProductDescription = item.ProductDescription,
                    ProductName = item.ProductName,
                    ProductNumber = item.ProductNumber,
                    ProductPrice = item.ProductPrice,
                    ProductID = item.ProductId,
                    Category = item.Category.Name
                });
            }
            return items;
        }

        public async Task<ActionResult> GetByIdAsync(int id)
        {
            var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null) return new NotFoundResult();

            return new OkObjectResult(new ProductInfo
            {
                Category = product.Category.Name,
                ProductDescription = product.ProductDescription,
                ProductName = product.ProductName,
                ProductNumber = product.ProductNumber,
                ProductID = product.ProductId,
                ProductPrice = product.ProductPrice
            });
        }

        public async Task<ActionResult> DeleteByID(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new NotFoundResult();
            }
            int categoryID = product.CategoryId;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            await _categoryManager.CheckIfDeleteCategory(categoryID);

            return new NoContentResult();
        }

        public async Task<ActionResult> UpdateAsync(int id, NewProductModel form)
        {
            var productEnt = await _context.Products.FindAsync(id);
            if (productEnt == null)
            {
                return new NotFoundResult();
            }

            productEnt.ProductName = form.ProductName;
            productEnt.ProductNumber = form.ProductNumber;
            productEnt.ProductDescription = form.ProductDescription;
            productEnt.ProductPrice = form.ProductPrice;
            productEnt.CategoryId = (await _categoryManager.GetOrCreateAsync(form.Category)).CategoryId;

            _context.Entry(productEnt).State = EntityState.Modified;
            _context.SaveChanges();
            return new OkObjectResult(productEnt);
        }
    }

}
