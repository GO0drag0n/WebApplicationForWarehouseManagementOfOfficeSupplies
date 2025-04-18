using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class EditProductViewModel
    {
        [Required]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters.")]
        public string Model { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Delivery Price must be a positive value.")]
        public decimal DeliveryPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Row is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Row must be greater than 0.")]
        public int Row { get; set; }

        [Required(ErrorMessage = "Section is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Section must be greater than 0.")]
        public int Section { get; set; }

        [Required(ErrorMessage = "Section is required.")]
        public int MinQuantityThreshold { get; set; }
    }
}
