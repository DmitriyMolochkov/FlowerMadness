using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories.Interfaces
{
    public interface IOrdersRepository : IRepository<Order>
    {
        Order GetCurrentOrderForCustomer(int id);
        List<Order> GetOrderHistory(string userId);
    }

    public interface IOrderDetailsRepository : IRepository<OrderDetail>
    {
        OrderDetail GetCurrentOrderDetailForOrder(int id, int productId);
    }
}
