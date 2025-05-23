﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;


namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Company
    {
        [Key]
        public Guid CompanyId { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }
        [Required]
        [StringLength(100)]
        public string CompanyAddress { get; set; }
        [Required]
        [StringLength(100)]
        public string CompanyPhone { get; set; }
        [Required]
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }
        public ICollection<UserCompany> UserCompanies { get; set; }
        [Required]
        [StringLength(50)] 
        public string VATNumber { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuarterOrderValue { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountLevel { get; set; }
    }
}
