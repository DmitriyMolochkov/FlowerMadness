using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Order : AuditableEntity
    {
        public int Id { get; set; }
        public decimal Discount { get; set; }
        public string Comments { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public byte Status { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }

    public enum OrderStatus
    {
        InProcess,
        Confirmed,
        PaidUp,
        Sent,
        Delivered,
        Canceled
    }
}
