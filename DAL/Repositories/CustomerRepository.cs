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
                .OrderBy(c => c.Name)
                .ToList();
        }

        public Customer GetCurrentCustomerForUser(string userId)
        {
            var result =  _appContext.Customers
                .Include(c => c.Orders).ThenInclude(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Include(c => c.ApplicationUser)
                .OrderBy(c => c.Name)
                .Where(c => c.ApplicationUserId == userId)
                .ToList()
                .OrderBy(x => x.DateCreated)
                .LastOrDefault();

            return result;
        }

        public Task<Customer> GetByIdAsync(int id)
        {
            return _appContext.Customers.Where(x => x.Id == id)/*.Include(x => x.Parent).Include(x => x.Children)*/.FirstOrDefaultAsync();
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
