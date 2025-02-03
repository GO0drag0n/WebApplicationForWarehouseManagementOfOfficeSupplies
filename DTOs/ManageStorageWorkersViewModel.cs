using System.Collections.Generic;

namespace WebApplicationForWarehouseManagementOfOfficeSupplies.DTOs
{
    public class ManageStorageWorkersViewModel
    {
        public List<StorageWorkerViewModel> StorageWorkers { get; set; }
    }

    public class StorageWorkerViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
