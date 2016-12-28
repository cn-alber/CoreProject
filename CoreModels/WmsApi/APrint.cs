using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class APrintParams
    {
        public int CoID { get; set; }
        public int PrintType { get; set; }
        public string PrintID { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
        public string BarCode { get; set; }
        public int OID { get; set; }
        public string ExCode { get; set; }
    }

    public class APrinter
    {
        public int ID { get; set; }
        public int CoID { get; set; }
        public int PrintType { get; set; }
        public string PrintName { get; set; }
        public string PrintID { get; set; }
        public string IPAddress { get; set; }
        public bool IsDefault { get; set; }
        public bool Enabled { get; set; }
        public string Creator { get; set; }
        public string CreatDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }

    public class ABarCodeLoc
    {
        public ASkuScan SkuAuto { get; set; }
        public string PCode { get; set; }
        public string Content { get; set; }
    }


}