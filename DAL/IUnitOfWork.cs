﻿using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUnitOfWork
    {
        ICustomerRepository Customers { get; }
        IProductRepository Products { get; }
        IProductCategoryRepository ProductCategories { get; }
        IOrdersRepository Orders { get; }
        IOrderDetailsRepository OrderDetails { get; }

        int SaveChanges();
    }
}
