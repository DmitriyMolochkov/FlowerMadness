using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FlowerMadness.Helpers;

namespace FlowerMadness.ViewModels
{
    public class OrderDetailViewModel : OrderDetailDtoModel
    {
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }

        public ProductForCustomerViewModel Product { get; set; }
    }

    public class OrderDetailDtoModel
    {
        [Range(0, Int32.MaxValue)]
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}
