using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreData;
using CoreData.CoreComm;
using CoreDate.CoreApi;
using CoreModels;
using CoreModels.XyApi.JingDong;
using CoreWebApi.ApiTask;
using Dapper;
using MySql.Data.MySqlClient;

namespace tasks.jos
{
    /// <summary>
    /// order_api_get 的摘要说明
    /// </summary>
    public class order_api_get : CoreWebApi.ApiTask.BaseApi
    {

        public override string Name
        {
            get { return "从JOS下载订单数据"; }
        }

        public override string Author
        {
            get { return "827869959@qq.com"; }
        }

        public override int Timeout
        {
            get { return 300; }
        }

        public override int Interval
        {
            get { return 300; }
        }
        
        public override int Type{
            get {return 3; }
        }

        protected override void Execute(CoreWebApi.ApiTask.ApiRunData apiData)
        {

            var modified = apiData.Job.RunTimestamp.HasValue ? apiData.Job.RunTimestamp.Value.AddSeconds(1).ToString() : DateTime.Today.AddDays(-3).ToString();           
            var now = DateTime.Now.AddMinutes(-3).ToString();

            string orderIDs = "";

            if (!apiData.Args.IsNullOrEmpty())
            {
                modified = "";
                now = "";
                foreach (var arg in apiData.Args)
                {
                    orderIDs = arg.ToString();
                }
            }else{
                var monthlater = Convert.ToDateTime(modified).AddDays(3);
                if (DateTime.Compare(monthlater, Convert.ToDateTime(now)) < 0)
                { //启动时间戳与当前时间相差不能查过3天
                    now = monthlater.ToString();
                }
            }

            apiData.Job.RunResult = 0;
            this.GetPage(apiData, modified, now, 1, orderIDs);
            
        }
         protected void GetPage(CoreWebApi.ApiTask.ApiRunData apiData, string bof, string eof, int pageNo, string orderIDs)
        {

            try
            {
                var response = new DataResult(1,null);
                if (orderIDs == "")
                {
                    response = JingDHaddle.jdOrderDownload(bof, eof, pageNo, 100, apiData.Job.Shop.Token);
                }
                else
                {                    
                    List<string> order_ids = orderIDs.Split(',').ToList<string>(); 
                    response = JingDHaddle.orderDownByIds(order_ids, "", apiData.Job.Shop.Token);
                }

                if (!(response.s == 1))
                {
                    if (response.d.ToString().IndexOf("重新授权") > -1)
                    {
                        ShopHaddle.TokenExpired(apiData.Job.Shop.ID.ToString(),apiData.Job.Shop.CoID.ToString());
                        using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                            try{
                                conn.Execute(@"UPDATE api_job SET api_job.enabled = 0 WHERE api_job.job_id = @job_id",new {job_id = apiData.Job.JobId });
                            }catch{
                                conn.Dispose();
                            }
                        }
                    }
                    throw new ApiTaskException(601,response.d.ToString()); 
                }

                var shop = apiData.Job.Shop;
                Task[] tasks = new Task[10];
                order_search orderSearch = response.d as order_search;
                List<order_info_list> orderinfos = orderSearch.order_info_list; 
                apiData.Job.RunResult += orderinfos.Count;


                if (orderinfos.Count > 0)
                {
                    for (int i = 0; i < orderinfos.Count; i++)
                    {
                        if (i == 0)
                        {
                            tasks[i] = Task.Factory.StartNew(() =>
                            {
                                jdToLocal(orderinfos[i], int.Parse(shop.CoID.ToString()), shop.ShopName, shop.ShopSite, shop.Token, shop.ID);
                            });
                            System.Threading.Thread.Sleep(1000);
                        }
                        else
                        {
                            try
                            {
                                tasks[i % 10] = Task.Factory.StartNew(() =>
                                {
                                    if (i != 100)
                                    {

                                        jdToLocal(orderinfos[i], int.Parse(shop.CoID.ToString()), shop.ShopName, shop.ShopSite, shop.Token, shop.ID);
                                    }
                                });
                                System.Threading.Thread.Sleep(1000);
                            }
                            catch
                            {
                                //XyComm.threadlog.InsertThreadLog("店铺：" + shop.ShopName + "下，单号：" + orderinfos[i].order_id + "按时间段导入报错", ex.Message, DateTime.Now.ToString(), "线程运行", "");
                                continue;
                            }
                        }
                        if (i % 10 == 9)
                        {
                            Task.WaitAll(tasks);
                            tasks = new Task[10];
                        }
                    }
                    if (orderIDs == "")
                    {                        
                        apiData.Job.RunTimestamp = Convert.ToDateTime(orderinfos.LastOrDefault().modified);//最后一条数据获得的修改时间（京东订单检索以修改时间为基准）
                    }
                    
                        
                }
                
                long pageTotal = Convert.ToInt64(orderSearch.order_total);
                if (pageNo < pageTotal)
                {
                    pageNo++;
                    this.GetPage(apiData, bof, eof, pageNo, orderIDs);
                }

            }
            catch (ApiTaskException ex)
            {
                throw new ApiTaskException(602, string.Format("{0}", ex.Message));
            }
        }

        #region 订单存入本地
        public static void jdToLocal(order_info_list orderinfo, int CoID, string ShopName, string ShopSite, string token, int shopid)
        {
            // try
            // {

            //     ImpOrdModule OrdMo = new ImpOrdModule();
            //     decimal sellerDiscount = decimal.Parse(orderinfo.seller_discount);//卖家折扣
            //     decimal equalDiscount = decimal.Round(sellerDiscount % orderinfo.item_info_list.Count, 2);//
            //     decimal overplus = sellerDiscount - equalDiscount * orderinfo.item_info_list.Count; //若平均有余

            //     OrdMo.Type = 0;

            //     OrdMo.OSource = (int)Enum.Parse(typeof(Dos.Model.Enum.OrderE.OrdSource), orderinfo.order_source, true);
            //     OrdMo.ODate = Convert.ToDateTime(orderinfo.order_start_time);
            //     OrdMo.BuyerShopID = orderinfo.consignee_info.fullname;
            //     OrdMo.ShopName = ShopName;
            //     OrdMo.ShopSit = ShopSite;
            //     OrdMo.SoID = long.Parse(orderinfo.order_id);
            //     OrdMo.PayAmount = decimal.Parse(orderinfo.order_payment); //实际付款
            //     OrdMo.ExAmount = decimal.Parse(orderinfo.freight_price);
            //     OrdMo.ShopStatus = orderinfo.order_state;
            //     OrdMo.RecName = orderinfo.consignee_info.fullname;
            //     #region 京东地址与本地不一致，顾稍作修改
            //     //自治区时，城市同一为自治区名+市，比如上海， RecCity则为 上海市，黄浦区之类后移至 RecDistrict，城镇不记录
            //     if (MUNICIPALITIES.Contains(orderinfo.consignee_info.province))
            //     {
            //         OrdMo.RecLogistics = orderinfo.consignee_info.province;
            //         OrdMo.RecCity = orderinfo.consignee_info.province + "市";
            //         OrdMo.RecDistrict = orderinfo.consignee_info.city;
            //     }
            //     else
            //     {
            //         OrdMo.RecLogistics = orderinfo.consignee_info.province + "省";
            //         OrdMo.RecCity = orderinfo.consignee_info.city;
            //         OrdMo.RecDistrict = orderinfo.consignee_info.county;
            //     }
            //     #endregion
            //     OrdMo.RecAddress = orderinfo.consignee_info.full_address;
            //     OrdMo.RecTel = orderinfo.consignee_info.mobile;
            //     OrdMo.RecPhone = orderinfo.consignee_info.telephone;
            //     OrdMo.ExCost = decimal.Parse(orderinfo.freight_price);

            //     OrdMo.SkuAmount = 0; //商品金额
            //     int num = 1;
            //     ImpOrdDetailModule itemdetail = new ImpOrdDetailModule();
            //     OrdMo.detail = new List<ImpOrdDetailModule>();
            //     foreach (item_info_list iteminfo in orderinfo.item_info_list)
            //     {
            //         itemdetail.SkuID = iteminfo.sku_id;
            //         itemdetail.SkuName = iteminfo.sku_name;
            //         var resSku = XyOrder.DataQuery.GetSkuEdit(CoID, itemdetail.SkuID).DataCount;
            //         if (Convert.ToInt32(resSku) == 0)
            //         {
            //             if (!skuIdaAutoAdd(iteminfo.sku_id, token, CoID))
            //                 break;
            //         }

            //         itemdetail.Qty = Convert.ToInt32(iteminfo.item_total);
            //         if (num == orderinfo.item_info_list.Count) //实际价格 = 商品本来价格 - 折扣价格
            //         {
            //             itemdetail.RealPrice = decimal.Parse(iteminfo.jd_price) - equalDiscount - overplus;
            //         }
            //         else
            //         {
            //             itemdetail.RealPrice = decimal.Parse(iteminfo.jd_price) - equalDiscount;
            //         }
            //         itemdetail.Amount = itemdetail.RealPrice * decimal.Parse(iteminfo.item_total);
            //         //itemdetail.ShopSkuID = 1;//iteminfo.outer_sku_id
            //         itemdetail.Remark = orderinfo.order_remark;
            //         OrdMo.detail.Add(itemdetail);
            //         OrdMo.SkuAmount += itemdetail.RealPrice;
            //         itemdetail = new ImpOrdDetailModule();//init
            //         num++;
            //     }
            //     OrdMo.Amount = OrdMo.SkuAmount + decimal.Parse(orderinfo.freight_price);

            //     //OrdMo.IsInvoice = orderinfo.invoice_info; 发票
            //     if (orderinfo.invoice_info == "不需要开具发票")
            //     {
            //         OrdMo.IsInvoice = false;
            //     }
            //     else
            //     {
            //         string[] invoiceDetail = orderinfo.invoice_info.Split(';');
            //         OrdMo.IsInvoice = true;
            //         OrdMo.InvoiceType = invoiceDetail[0].Split(':')[1];
            //         OrdMo.InvoiceTitle = invoiceDetail[1].Split(':')[1];
            //         //OrdMo.InvoiceDate ;
            //     }

            //     OrdMo.IsPaid = true;//支付   京东获取到的订单都为已付款订单
            //     OrdMo.PayDate = Convert.ToDateTime(orderinfo.order_start_time);
            //     OrdMo.PayNbr = orderinfo.order_id;
            //     OrdMo.pay = new ImpPayInfoModule();
            //     OrdMo.pay.Payment = "1";//orderinfo.pay_type;
            //     OrdMo.pay.PayAccount = orderinfo.consignee_info.fullname;
            //     OrdMo.pay.SellerAccount = ShopName;
            //     OrdMo.pay.Platform = orderinfo.order_source;
            //     OrdMo.pay.PayDate = Convert.ToDateTime(orderinfo.order_start_time);
            //     OrdMo.pay.PayAmount = decimal.Parse(orderinfo.order_payment);

            //     BaseResult baseRe = XyOrder.DataUpdate.ImpOrd(OrdMo, CoID, "进程运行");
            //     if (!baseRe.IsSuccess)
            //     {
            //         XyComm.threadlog.InsertThreadLog("店铺：" + ShopName + "下，单号：" + orderinfo.order_id + "导入报错", baseRe.Message, DateTime.Now.ToString(), "", "");
            //     }
            //     else
            //     {
            //         // XyComm.threadlog.InsertThreadLog("店铺：" + ShopName + "下，单号：" + orderinfo.order_id + "成功", baseRe.Message, DateTime.Now.ToString(), "", "");
            //     }


            // }
            // catch (Exception ex)
            // {
            //     XyComm.threadlog.InsertThreadLog("店铺：" + ShopName + "下，单号：" + orderinfo.order_id + "导入报错", ex.Message, DateTime.Now.ToString(), "", "");
            // }
        }
        #endregion

        // #region 如果sku未获取到，自动从线上获取
        // private static bool skuIdaAutoAdd(string skuid, string token, int Coid)
        // {
        //     BaseResult result = jdSku.jdFindSkuById(skuid, token);
        //     if (!result.IsSuccess)
        //     {

        //         return false;
        //     }
        //     return skuFunction(result.Data as jdSearchSkuListData, Coid).IsSuccess;

        // }
        // #endregion
        // private static BaseResult skuFunction(jdSearchSkuListData skuinfo, int Coid)
        // {

        //     var IParm = new CoreSkuParms();
        //     IParm.CoID = Coid;
        //     IParm.SkuID = skuinfo.skuId;
        //     BaseResult res = XyCore.CoreSku.GetCoreSkuDetail(IParm);
        //     if (res.Data != null)
        //         return res;

        //     BaseResult cres = new BaseResult();
        //     BaseResult sres = new BaseResult();

        //     if (skuinfo.saleAttrs != null)
        //     {
        //         foreach (SkuSaleAttr saleattr in skuinfo.saleAttrs)
        //         {
        //             if (saleattr.attrId == "1000000014" || saleattr.attrId == "1000012739" || saleattr.attrId == "1000012721")//判断颜色是否存在: Y->取颜色ID&Name;N->新增颜色
        //             {
        //                 var CParm = new ColorParms();
        //                 CParm.CoID = Coid;
        //                 CParm.Name = saleattr.attrValueAlias[0];
        //                 cres = XyGoods.CoreColor.GetColorByName(CParm);

        //                 if (cres.DataCount == 0)
        //                 {
        //                     Dos.Model.Corecolor c = new Dos.Model.Corecolor();
        //                     c.Name = "";// GetIdentityName()[2];
        //                     c.CustomerColorID = "";
        //                     c.CoID = CParm.CoID;
        //                     c.Creator = "进程运行";// GetIdentityName()[0];
        //                     cres = XyGoods.CoreColor.InsertColor(c, "", "");
        //                 }
        //             }
        //             else if (saleattr.attrId == "1000000018" || saleattr.attrId == "1000012740" || saleattr.attrId == "1000012722")
        //             { //判断尺码是否存在: Y->取尺码ID&Name;N->新增尺码
        //                 var SParm = new SizeParms();
        //                 SParm.CoID = Coid;
        //                 SParm.Name = saleattr.attrValueAlias[0];
        //                 SParm.ParentID = "0";
        //                 sres = XyGoods.CoreSize.GetSizeByName(SParm);
        //                 if (sres.DataCount == 0)
        //                 {
        //                     Dos.Model.coresize s = new Dos.Model.coresize();
        //                     s.Name = saleattr.attrValueAlias[0];
        //                     s.Creator = "";// GetIdentityName()[0];
        //                     s.ParentID = "0";
        //                     s.CustomerSizeID = "进程运行";
        //                     sres = XyGoods.CoreSize.InssertSize(s, "", "");
        //                 }
        //             }
        //         }
        //     }
        //     var CLst = cres.Data as Dos.Model.Corecolor;
        //     var SLst = sres.Data as Dos.Model.coresize;
        //     var ckm = new CoreSkuAuto();
        //     var cki = new CoreSkuItem();
        //     ckm.CoID = Coid;
        //     ckm.Creator = "进程运行";
        //     //ckm.GoodsCode   = grid1.DataKeys[i][2].ToString();
        //     //ckm.GoodsName   = skuinfo.wareTitle;
        //     ckm.SkuName = skuinfo.wareTitle;
        //     cki.SkuID = skuinfo.skuId;

        //     ckm.Unit = "件";
        //     cki.ColorID = CLst != null ? CLst.ColorID : "0373";
        //     cki.ColorName = CLst != null ? CLst.Name : "携云科技";
        //     cki.SizeID = SLst != null ? SLst.SizeID : "108";
        //     cki.SizeName = SLst != null ? SLst.Name : "190;常规";
        //     //if (ParentID=="")
        //     //    cki.ParentID = SLst.ParentID;
        //     //else
        //     //    cki.ParentID = ParentID;
        //     BaseResult skures = XyCore.CoreSku.InsertCoreSku(ckm, cki);
        //     return skures;


        // }



   

    }
    public class orderByIdRequestForm
    {
        public static DateTime startT;
        public static DateTime endT;
        public static string orderID;
    }



}