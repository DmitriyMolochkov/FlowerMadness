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
    public class OrdersRepository : Repository<Order>, IOrdersRepository
    {
        public OrdersRepository(DbContext context) : base(context)
        { }
        
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public Order GetCurrentOrderForCustomer(int id)
        {
            return _appContext.Orders
                .Include(x => x.OrderDetails).ThenInclude(x => x.Product)
                .Include(x => x.Customer)
                .Where(x => x.CustomerId == id && x.Status == (byte)OrderStatus.InProcess)
                .ToList()
                .OrderBy(x => x.DateCreated)
                .LastOrDefault();
        }

        public List<Order> GetOrderHistory(string userId)
        {
            return _appContext.Orders
                .Include(x => x.OrderDetails).ThenInclude(x => x.Product)
                .Include(x => x.Customer)
                .Where(x => x.Customer.ApplicationUserId == userId)
                .OrderBy(x => x.DateCreated)
                .ToList();
        }
    }

    public class OrderDetailsRepository : Repository<OrderDetail>, IOrderDetailsRepository
    {
        public OrderDetailsRepository(DbContext context) : base(context)
        { }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public OrderDetail GetCurrentOrderDetailForOrder(int id, int productId)
        {
            return _appContext.OrderDetails
                .Include(x => x.Product)
                .Include(x => x.Order)
                .Where(x => x.OrderId == id && x.ProductId == productId)
                .ToList()
                .LastOrDefault();
        }
    }
}
