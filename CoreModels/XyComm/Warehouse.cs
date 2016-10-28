using System;
namespace CoreModels.XyComm
{
    public class WarehouseInsert
    {
        public int id{get;set;}
        public string warehousename0 {get;set;}
        public string warehousename1 {get;set;}
        public string warehousename2 {get;set;}
        public string warehousename3 {get;set;}
        public string warehousename4 {get;set;}
        public string warehousename5 {get;set;}
        public string contract {get;set;}
        public string phone {get;set;}
        public int logistics {get;set;}
        public int city {get;set;}
        public int district {get;set;}
        public string address {get;set;}
        public bool enable{get;set;}
    }
    public class Warehouse
    {
        public int id{get;set;}
        public int parentid {get;set;}
        public string warehousename {get;set;}
        public int type {get;set;}
        public string contract {get;set;}
        public string phone {get;set;}
        public int logistics {get;set;}
        public int city {get;set;}
        public int district {get;set;}
        public string address {get;set;}
        public bool enable {get;set;}  
        public string creator {get;set;}
        public DateTime createdate {get;set;}
        public string modifier {get;set;}
        public DateTime modifydate {get;set;}
        public int coid {get;set;}
    }
}