using CoreModels;
using Dapper;
using System;
using CoreModels.XyComm;
using CoreData;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData.CoreCore;
using CoreModels.XyCore;
using static CoreModels.Enum.OrderE;
using System.Threading.Tasks;

namespace CoreDate.CoreComm
{
    public static class PrintDataHaddle
    {
        
         /// <summary>
		/// 销售出库单
		/// </summary>
        public static DataResult getSaleForm(int id,int oid,string coid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{                
                    var order = new OrderDetail();
                    var sale = new SaleOutItem();
                    var ware = new WareDetail();
                    var shop = new Shop();
   
                    var tasks = new Task[3];
                    tasks[0] = Task.Factory.StartNew(() =>
                    {
                        var aa = getOrderDetail(oid,coid);
                        if(aa.s == -1)
                        {
                            result.s = -1;
                            result.d = aa.d;
                        }else{
                            order = aa.d as OrderDetail;
                            shop = ShopHaddle.ShopQuery(coid,order.ShopID).d as Shop;
                        }
                    });
                    tasks[1] = Task.Factory.StartNew(() =>
                    {
                        var bb = getSingleSaleOrder(id,coid);
                        if(bb.s == -1)
                        {
                            result.s = -1;
                            result.d =bb.d;
                        }else{
                            sale = bb.d as SaleOutItem;                        
                        }
                    });
                    tasks[2] = Task.Factory.StartNew(() =>
                    {
                        var cc = getSendWare(coid);
                        if(cc.s == -1)
                        {
                            result.s = -1;
                            result.d =cc.d;
                        }else{
                            ware = cc.d as WareDetail;                        
                        }
                    });                    
                    Task.WaitAll(tasks);
                    if(result.s == -1){ return result;}

                    result.d = new {
                        io_id =  sale.ID ,
                        o_id = sale.OID,
                        so_id = sale.SoID,
                        io_date = sale.DocDate,
                        io = -1,
                        co_id = coid,
                        wh_id = order.WarehouseID,
                        warehouse = sale.SendWarehouse,
                        quality_type = 1,
                        type="销售出仓",
                        status = getStatus(sale.Status),
                        link_wh_id = "",
                        link_warehouse = "",
                        link_io_id = "",
                        created = sale.CreateDate,
                        creator =0,
                        modified = sale.ModifyDate,
                        modifier = 0,
                        creator_name = sale.Creator,
                        modifier_name = sale.Modifier,
                        confirm_user = "",
                        receiver_country = "",
                        receiver_state = sale.RecLogistics,
                        receiver_city = sale.RecCity,
                        receiver_district =sale.RecDistrict,
                        receiver_district_id = "",
                        receiver_address = sale.RecAddress,
                        receiver_name =sale.RecName,
                        receiver_phone = order.RecTel,
                        receiver_mobile = sale.RecPhone,
                        buyer_message = sale.RecMessage,
                        shipment = "",
                        logistics_company = sale.ExpName,
                        lc_id = order.ExpNamePinyin,
                        l_id = sale.ExCode,
                        freight = order.ExCost,
                        weight = order.ExWeight,
                        remark = sale.Remark,
                        carry_id = "",
                        is_invoice = order.IsInvoice,
                        invoice_title = order.InvoiceTitle,
                        invoice_type = order.InvoiceType,
                        pay_date = order.InvoiceDate,
                        shop_id = 0,
                        is_delivery = sale.IsDeliver,
                        as_id = 0,
                        is_print = sale.IsOrdPrint,
                        is_print_express = sale.IsExpPrint,
                        sku_bin = "",
                        bin_id = 0,
                        pay_amount = order.Amount,
                        paid_amount = order.PaidAmount,
                        free_amount= order.Amount - order.PaidAmount,
                        seller_flag = "",
                        tag = "",
                        send_state = ware.send_state,
                        send_city = ware.send_city,
                        send_district = ware.send_district,
                        send_address = ware.address,
                        print_date = "",
                        send_full_address = ware.send_state+ware.send_city+ware.send_district,
                        receiver_full_address = sale.RecLogistics+sale.RecCity+sale.RecDistrict,
                        receiver_mobile_phone = sale.RecPhone,
                        item_amount = 0,
                        item_base_amount = 0,
                        sku_id = sale.sku_id,
                        name = sale.name,
                        properties_value = sale.properties_value,
                        name_properties_value = sale.name_properties_value,
                        sale_price = sale.sale_price,
                        item_pattern = sale.item_pattern,
                        shop_url = shop.ShopUrl,
                        shop_name = order.ShopName,
                        items = sale.items
                     };
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }


        /// <summary>
		/// 
		/// </summary>
        public static DataResult getSingleSaleOrder(int id,string coid){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    var sql = @"select ID,OID,SoID,DocDate,Status,ExID,ExpName,ExCode,BatchID,IsOrdPrint,IsExpPrint,RecMessage,RecLogistics,RecDistrict,RecCity,RecAddress,
                                  RecName,RecPhone,ExWeight,RealWeight,ShipType,ExCost,IsDeliver,Remark,OrdQty, SendWarehouse, CreateDate,Creator,
                                  ModifyDate,Modifier,RecZip  
                                  from saleout where ID="+id+" AND CoID="+coid;
                    var saleorder = conn.Query<SaleOutItem>(sql).AsList();
                    if(saleorder.Count == 0) {
                        result.s = -1;
                        result.d= "订单编号不存在";
                    }else{
                        var sale=saleorder[0];                        
                        string sqlcommand = @"select 
                                           ID as oi_id ,
                                           OID as oid,
                                           CoID as co_id,
                                           SkuID as sku_id,
                                           Qty as qty,
                                           SalePrice as base_price,
                                           RealPrice as price,
                                           Amount as amount,
                                           SkuName as name,
                                           DiscountRate as discount_rate,
                                           img as pic,
                                           ShopSkuID as shop_sku_id,
                                           Weight as weight,
                                           TotalWeight as total_weight,
                                           CreateDate as created,
                                           Creator as creator,
                                           ModifyDate as modified,
                                           Modifier as modifier,
                                           IsGift as is_gift,
                                           Norm as properties_value,
                                           Remark as remark,
                                           SalePrice as sale_price
                                         from saleoutitem  where sid = @ID and coid = @Coid and IsGift = false";
                        var items = conn.Query<SaleOutPrintItem>(sqlcommand,new{ID = sale.ID,Coid =coid}).AsList();
                        foreach(var i in  items) {
                            i.outer_oi_id = sale.SoID;
                            i.sku_type = "nomal";
                            i.sku_tag = "";
                            i.name_properties_value =i.name + i.properties_value;
                            i.free_amount = i.amount - i.price*i.qty;
                            i.index = 1;
                            i.pic60 = i.pic;
                            i.pic100 = i.pic;
                            i.pic160 = i.pic;
                            sale.qty += i.qty;
                        }
                        sale.sku_id = items.Count >0 ? items[0].sku_id : "";
                        sale.name = items.Count > 0 ? items[0].name : "";
                        sale.properties_value = items.Count > 0 ? items[0].properties_value : "";
                        sale.name_properties_value = items.Count > 0 ? items[0].name_properties_value : "";
                        sale.sale_price = items.Count > 0 ? items[0].sale_price : 0;
                        sale.item_pattern = "";
                        
                        sale.items = items;   
                        result.d = sale;
                    }

                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        public static DataResult getOrderDetail(int id,string coid){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sqlcommand = @"select ID,
                                            ShopName,
                                            OSource,
                                            PayDate,
                                            BuyerShopID,
                                            ExAmount,
                                            Express,
                                            ExCode,
                                            RecTel,
                                            RecPhone,
                                            Status,
                                            AbnormalStatus,
                                            StatusDec as AbnormalStatusDec,
                                            ExID,
                                            SkuAmount,
                                            Amount,
                                            PaidAmount,
                                            WarehouseID,
                                            ExWeight,
                                            ExCost,
                                            IsInvoice,
                                            InvoiceTitle,
                                            InvoiceType,
                                            InvoiceDate,
                                            DealerType ,
                                            ShopID
                                            From `order` where id = @ID and coid = @Coid";
                    var order = conn.Query<OrderDetail>(sqlcommand,new{ID=id,Coid=coid}).AsList();
                    if(order.Count == 0)
                    {
                        result.s = -1;
                        result.d = "订单单号无效!";
                        return result;
                    }
                    order[0].OSource = Enum.GetName(typeof(OrdSource), int.Parse(order[0].OSource));
                    if(!string.IsNullOrEmpty(order[0].ExID.ToString()))
                    {
                        order[0].ExpNamePinyin = OrderHaddle.GetExpNamePinyin(int.Parse(coid),order[0].ExID);
                    }
         
                    if(order[0].PaidAmount == 0)
                    {
                        order[0].PayDate = null;
                    }
                    
                    result.d = order[0];

                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        public static DataResult getSendWare(string coid){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var sql = @"SELECT WarehouseName, Contract, Phone,Logistics,City,District,Address FROM warehouse
                                 where ParentID=0 AND CoID="+coid;
                    var saleorder = conn.Query<WareDetail>(sql).AsList();
                    if(saleorder.Count == 0) {
                        result.s = -1;
                        result.d= "仓库不存在";
                    }else{
                        var ware = saleorder[0];
                        ware.send_state = conn.Query<string>("SELECT `Name` FROM area WHERE ID = "+ware.logistics).AsList()[0];
                        ware.send_city = conn.Query<string>("SELECT `Name` FROM area WHERE ID = "+ware.city).AsList()[0];
                        ware.send_district = conn.Query<string>("SELECT `Name` FROM area WHERE ID = "+ware.district).AsList()[0];
                        result.d=saleorder[0];
                    }

                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        //状态(0:待出库;1.已出库(生效);2.已出库待发货;3.已出库并且已发货;4.归档;5.出库后作废;6.出库前作废;7.外部发货中)
        public static string getStatus(int a){
            string status = "";
            switch (a)
            {
                case 0:
                    status = "待出库";
                    break;
                case 1:
                    status = "已出库(生效)";
                    break;
                case 2:
                    status = "已出库待发货";
                    break;
                case 3:
                    status = "已出库并且已发货";
                    break;
                case 4:
                    status = "归档";
                    break;
                case 5:
                    status = "出库后作废";
                    break;
                case 6:
                    status = "出库前作废";
                    break; 
                case 7:
                    status = "外部发货中";
                    break;                                                                                                                                                               
                default:
                    status = "状态异常";
                    break;
            }
            return status;
        }














   
   
    }
}