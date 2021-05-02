using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAllWithFiltersAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> PostAsync(Product product);
        Product Put(Product product);
        Task DeleteAsync(int id);
    }
}
