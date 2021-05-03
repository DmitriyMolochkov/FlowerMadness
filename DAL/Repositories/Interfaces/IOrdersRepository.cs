using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories.Interfaces
{
    public interface IOrdersRepository : IRepository<Order>
    {
        Order GetCurrentOrderForCustomer(int id, OrderStatus? status);
        List<Order> GetAllOrders(OrderStatus? status);
        List<Order> GetOrderHistory(string userId, OrderStatus? status);
        List<Order> GetOrderHistory(int customerId, OrderStatus? status);
        Order GetOrderById(int id);
    }

    public interface IOrderDetailsRepository : IRepository<OrderDetail>
    {
        OrderDetail GetCurrentOrderDetailForOrder(int id, int productId);
    }
}
