using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAllWithFiltersAsync(int? productCategoryId, decimal? maxPrice, decimal? minPrice, string search);
        Task<Product> GetByIdAsync(int id);
        Task<Product> PostAsync(Product product);
        Product Put(Product product);
        Task DeleteAsync(int id);
    }

    public interface IProductCategoryRepository : IRepository<ProductCategory>
    {
        Task<List<ProductCategory>> GetAllAsync();
        Task<ProductCategory> GetByIdAsync(int id);
        Task<ProductCategory> PostAsync(ProductCategory product);
        ProductCategory Put(ProductCategory product);
        Task DeleteAsync(int id);
    }
}
