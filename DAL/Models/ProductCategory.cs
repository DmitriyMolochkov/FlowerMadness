using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DAL.Models
{
    public class ProductCategory : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [MaxLength(500)]
        [Column(TypeName = "TEXT")]
        public string Description { get; set; }
        [MaxLength(65535)]
        [Column(TypeName = "TEXT")]
        public string Icon { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
