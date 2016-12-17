using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class ASaleParams
    {
        public int CoID { get; set; }
        public int BatchType { get; set; }
        public int BatchID { get; set; }
        public string BarCode { get; set; }
        public int Skuautoid { get; set; }
    }
    public class OutItemBatch
    {
        public int BatchID { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
        public int ItemQty { get; set; }
        public int OutID { get; set; }
        public string ExCode { get; set; }
        public string ExpName { get; set; }
        public string IsExpPrint { get; set; }
    }

    public class ASaleOutData
    {
        public ASkuScan SkuAuto { get; set; }
        public OutItemBatch OItemAuto { get; set; }
        public int ID { get; set; }//拣货记录ID

    }
    public class ASaleOutSet
    {
        public ASkuScan SkuAuto { get; set; }
        public OutItemBatch OItemAuto { get; set; }
        public int ID { get; set; }//拣货记录ID
        public int Contents { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }

    }
}