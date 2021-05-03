using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
        
        public ProductRepository(DbContext context) : base(context)
        { }

        public async Task<List<Product>> GetAllWithFiltersAsync(int? productCategoryId, decimal? maxPrice, decimal? minPrice, string search)
        {
            search = search?.ToUpper();
            
            return await _appContext.Products
                .Where(
                    x => 
                        (productCategoryId == null || x.ProductCategoryId == productCategoryId) &&
                        (maxPrice == null || x.SellingPrice <= maxPrice) &&
                        (minPrice == null || x.SellingPrice >= minPrice) &&
                        (search == null || x.Name.ToUpper().Contains(search))
                    )
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _appContext.Products.Where(x => x.Id == id)/*.Include(x => x.Parent).Include(x => x.Children)*/.FirstOrDefaultAsync();
        }

        public async Task<Product> PostAsync(Product product)
        {
            return (await _appContext.Products.AddAsync(product)).Entity;
        }

        public Product Put(Product product)
        {
            return _appContext.Products.Update(product).Entity;
        }

        public async Task DeleteAsync(int id)
        {
             _appContext.Products.Remove(await GetByIdAsync(id));
        }

    }

    public class ProductCategoryRepository : Repository<ProductCategory>, IProductCategoryRepository
    {
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public ProductCategoryRepository(DbContext context) : base(context)
        { }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await _appContext.ProductCategories.Include(x => x.Products).ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            return await _appContext.ProductCategories.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ProductCategory> PostAsync(ProductCategory product)
        {
            return (await _appContext.ProductCategories.AddAsync(product)).Entity;
        }

        public ProductCategory Put(ProductCategory product)
        {
            return _appContext.ProductCategories.Update(product).Entity;
        }

        public async Task DeleteAsync(int id)
        {
            _appContext.ProductCategories.Remove(await GetByIdAsync(id));
        }

    }
}
