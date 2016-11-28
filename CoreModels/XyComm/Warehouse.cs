using System;
using System.Collections.Generic;

namespace CoreModels.XyComm
{
    public class WarehouseInsert
    {
        public int id { get; set; }
        public string name0 { get; set; }
        public string name1 { get; set; }
        public string name2 { get; set; }
        public string name3 { get; set; }
        public string name4 { get; set; }
        public string name5 { get; set; }
        public string contract { get; set; }
        public string phone { get; set; }
        public List<int> area { get; set; }
        public string address { get; set; }
        public bool enable { get; set; }
    }

    public class WarehouseResponse
    {
        public string name0 { get; set; }
        public string name1 { get; set; }
        public string name2 { get; set; }
        public string name3 { get; set; }
        public string name4 { get; set; }
        public string name5 { get; set; }
        public string contract { get; set; }
        public string phone { get; set; }
        public List<int> area { get; set; }
        public string address { get; set; }
        public bool enable { get; set; }
    }



    public class Warehouse
    {
        public int id { get; set; }
        public int parentid { get; set; }
        public string warehousename { get; set; }
        public int type { get; set; }
        public string contract { get; set; }
        public string phone { get; set; }
        public int logistics { get; set; }
        public int city { get; set; }
        public int district { get; set; }
        public string address { get; set; }
        public bool enable { get; set; }
        public string creator { get; set; }
        public DateTime createdate { get; set; }
        public string modifier { get; set; }
        public DateTime modifydate { get; set; }
        public int coid { get; set; }
    }

    public class Warehouse_view
    {
        public string ID { get; set; }
        public string WhName { get; set; }
        public string CoID{get;set;}
    }
    
    public class WarehouseTree{
        public int value{get;set;}
        public string label{get;set;}
    }



}