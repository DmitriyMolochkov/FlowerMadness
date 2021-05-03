using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DAL.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;


        public CustomerRepository(ApplicationDbContext context) : base(context)
        { }

        public IEnumerable<Customer> GetTopActiveCustomers(int count)
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<Customer> GetAllCustomersData()
        {
            return _appContext.Customers
                .Include(c => c.Orders).ThenInclude(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(c => c.ApplicationUser)
                //.Include(c => c.Orders).ThenInclude(o => o.Cashier)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public Customer GetCurrentCustomerForUser(string userId)
        {
            var result =  _appContext.Customers
                .Include(c => c.Orders).ThenInclude(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(c => c.ApplicationUser)
                .Where(c => c.ApplicationUserId == userId)
                .ToList()
                .OrderByDescending(x => x.DateCreated)
                .LastOrDefault();

            return result;
        }

        public Task<Customer> GetByIdAsync(int id)
        {
            return _appContext.Customers.Where(x => x.Id == id)
                .Include(x => x.Orders)
                .ThenInclude(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer> PostAsync(Customer customer)
        {
            return (await _appContext.Customers.AddAsync(customer)).Entity;
        }

        public Customer Put(Customer customer)
        {
            return _appContext.Customers.Update(customer).Entity;
        }

        public async Task DeleteAsync(int id)
        {
            _appContext.Customers.Remove(await GetByIdAsync(id));
        }
    }
}
