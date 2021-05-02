using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Models;

namespace FlowerMadness.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public decimal Discount { get; set; }
        public string Comments { get; set; }
        public OrderStatus Status { get; set; }

        public CustomerViewModelForOrder Customer { get; set; }

        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

    public class OrderDtoModel
    {
        public string Comments { get; set; }
    }
}
