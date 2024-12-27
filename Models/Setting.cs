using System.ComponentModel.DataAnnotations;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.Models
{
    public class Setting
    {
        [Key]
        public int SettingID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}