using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData;
using CoreData.CoreApi;
using CoreData.CoreCore;
using CoreModels;
using CoreModels.XyApi.Tmall;
using CoreModels.XyCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Api.Tmall{   
    public class TmItemControllers : ControllBase{

        #region  获取店铺在售商品
        [HttpGetAttribute("/core/Api/TmItem/onsaleGet")]
        public ResponseResult onsaleGet(string page,string pageSize,string start_modified="",string end_modified=""){
            var m = new DataResult(1,null);    
            var coid = GetCoid();
            var uname = GetUname();
            m = TmallItemHaddle.onsaleGet(page,pageSize,start_modified,end_modified);
            dynamic items = m.d as dynamic;
            var tasks = new Task[10];
            int i= 0;
            TmallSku sku = new TmallSku();
            foreach (var item in items)
            {                
                sku.CoID = int.Parse(coid);
                sku.Creator = uname;
                sku.Enable = true;
                sku.GoodsCode = item.outer_id;
                sku.SkuName = item.title;
                sku.Img = item.pic_url;
                sku.IsParent = true;
                sku.Norm = "";
                sku.SalePrice = item.price;
                sku.SkuID = item.num_iid;
                tasks[i] = Task.Factory.StartNew(()=>{
                    CoreSkuHaddle.createSku(sku);
                });
                System.Threading.Thread.Sleep(100);
                i++;
                if(i == 10){
                    i = 0;
                    Task.WaitAll(tasks);
                }                                                            
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region  获取下载店铺在售商品
        [HttpGetAttribute("/core/Api/TmItem/sellerGet")]
        public ResponseResult sellerGet(){
            var m = new DataResult(1,null);    
            var coid = GetCoid();
            var uname = GetUname();
            var param = new CoreSkuParam();
            param.PageIndex = 1;
            param.PageSize = 350;
            var goods = CoreSkuHaddle.getWareGoodsInner(coid);
            long num_iid= CacheBase.Get<long>("sku_num_iid");
            //Console.WriteLine(num_iid);  
            try{                    
                foreach(var good in goods){                    
                    if(Convert.ToInt64(good.SkuID)>num_iid){
                        Console.WriteLine(good.SkuID);
                        num_iid = Convert.ToInt64(good.SkuID);
                        m = TmallItemHaddle.sellerGet(good.SkuID);                    
                        dynamic items = m.d as dynamic;  
                        if(items != null){          
                            var tasks = new Task[10];
                            int i= 0;                                                                           
                            TmallSku sku = new TmallSku();
                            foreach (var item in items)
                            {                
                                sku.CoID = int.Parse(coid);
                                sku.Creator = uname;
                                sku.Enable = true;
                                sku.SkuName = good.SkuName;
                                sku.GoodsCode = good.GoodsCode;
                                sku.GoodsName = good.GoodsCode;
                                sku.SafeQty = item.quantity;
                                sku.IsParent = false;
                                sku.Norm = item.properties_name;
                                sku.SalePrice = item.price;
                                sku.SkuID = item.outer_id;
                                tasks[i] = Task.Factory.StartNew(()=>{
                                    CoreSkuHaddle.createSku(sku);
                                });
                                System.Threading.Thread.Sleep(100);
                                i++;
                                if(i == 10){
                                    i = 0;
                                    Task.WaitAll(tasks);
                                }                                                            
                            }
                            CacheBase.Remove("sku_num_iid");
                            CacheBase.Set<long>("sku_num_iid", num_iid);
                        }
                        //return CoreResult.NewResponse(m.s, m.d, "Api");
                    }
                    
                }
            }catch(Exception ex){
                m.s = -1;
                m.d = ex.Message;
            }    
            
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion


    }
}