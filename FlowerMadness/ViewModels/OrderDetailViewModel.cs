using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}
