using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class AdminManageCompanyClientsViewModel
    {
        public Guid CompanyId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Address")]
        public string CompanyAddress { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Phone")]
        public string CompanyPhone { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "VAT Number")]
        public string VATNumber { get; set; }

        [Display(Name = "Quarter Order Value")]
        [DataType(DataType.Currency)]
        public decimal QuarterOrderValue { get; set; }

        [Display(Name = "Discount Level")]
        [DataType(DataType.Currency)]
        public decimal DiscountLevel { get; set; }
    }
}
