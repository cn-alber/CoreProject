using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
     public partial class AUser
    {
        public string Account { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        public bool Enable { get; set; }
        public int CompanyID { get; set; }
        public int RoleID { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public List<AWarehouse> AWhLst { get; set; }
        public string IPAddress { get; set; }
        public string ExpressIP { get; set; }
        public string BarIp { get; set; }
    }
    public class AUserParam
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}