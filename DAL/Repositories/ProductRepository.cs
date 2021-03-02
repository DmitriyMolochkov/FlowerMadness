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

        public async Task<List<Product>> GetAllWithFiltersAsync()
        {
            return await _appContext.Products.Include(x => x.Parent).Include(x => x.Children).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _appContext.Products.Where(x => x.Id == id).Include(x => x.Parent).Include(x => x.Children).FirstOrDefaultAsync();
        }

        public async Task<Product> PostAsync(Product product)
        {
            return (await _appContext.Products.AddAsync(product)).Entity;
        }

        public async Task<Product> PutAsync(Product product)
        {
            return _appContext.Products.Update(product).Entity;
        }

        public async Task DeleteAsync(int id)
        {
             _appContext.Products.Remove(await GetByIdAsync(id));
        }

    }
}
