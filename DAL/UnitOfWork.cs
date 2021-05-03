using DAL.Repositories;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        ICustomerRepository _customers;
        IProductRepository _products;
        IProductCategoryRepository _productCategory;
        IOrdersRepository _orders;
        IOrderDetailsRepository _orderDetails;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICustomerRepository Customers
        {
            get
            {
                if (_customers == null)
                    _customers = new CustomerRepository(_context);

                return _customers;
            }
        }
        
        public IProductRepository Products
        {
            get
            {
                if (_products == null)
                    _products = new ProductRepository(_context);

                return _products;
            }
        }

        public IProductCategoryRepository ProductCategories
        {
            get
            {
                if (_productCategory == null)
                    _productCategory = new ProductCategoryRepository(_context);

                return _productCategory;
            }
        }

        public IOrdersRepository Orders
        {
            get
            {
                if (_orders == null)
                    _orders = new OrdersRepository(_context);

                return _orders;
            }
        }

        public IOrderDetailsRepository OrderDetails
        {
            get
            {
                if (_orderDetails == null)
                    _orderDetails = new OrderDetailsRepository(_context);

                return _orderDetails;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
