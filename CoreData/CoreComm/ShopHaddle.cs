
using CoreModels;
using Dapper;
using System;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreData.CoreComm
{
    public static class ShopHaddle
    {
       
        ///<summary>
        ///查询店铺资料
        ///<summary>
        public static DataResult GetShopAll(ShopParam IParam)
        {
            var s = 1;
            string wheresql = "where 1=1";
            if(IParam.CoID!=1)
            {
                
                wheresql = wheresql + " AND CoID="+IParam.CoID;
            }
            if(!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper()!="ALL")//是否启用
            {
                wheresql = wheresql + " AND Enable = "+ (IParam.Enable.ToUpper()=="TRUE"?true:false);
            }
            if(!string.IsNullOrEmpty(IParam.Filter))//过滤条件
            {
                wheresql = wheresql +
                " AND ( ShopName LIKE '%"+IParam.Filter+
                "%' OR ShopSite LIKE '%"+IParam.Filter+
                "%' OR Creator LIKE '%"+IParam.Filter+"%')";
            }
            if(!string.IsNullOrEmpty(IParam.SortField)&& !string.IsNullOrEmpty(IParam.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+IParam.SortField +" "+ IParam.SortDirection;
            }
            wheresql = "select ID,ShopName,Enable,ShopSite,ShopUrl,Shopkeeper,UpdateSku,DownGoods,UpdateWayBill,TelPhone,SendAddress,CreateDate,Istoken from Shop "+ wheresql;
            var ShopLst = CoreData.DbBase.CommDB.Query<ShopQuery>(wheresql).AsList();
            IParam.DataCount = ShopLst.Count;
            decimal pagecnt = Math.Ceiling(decimal.Parse(IParam.DataCount.ToString())/decimal.Parse(IParam.PageSize.ToString()));
            IParam.PageCount = Convert.ToInt32(pagecnt);
            int dataindex = (IParam.PageIndex - 1)*IParam.PageSize;
            wheresql = wheresql + " limit " + dataindex.ToString() + " ," + IParam.PageSize;//分页            
            IParam.ShopLst = CoreData.DbBase.CommDB.Query<ShopQuery>(wheresql).AsList();           
            return new DataResult(s,IParam);
        }
        ///<summary>
        ///查询单笔店铺资料
        ///<summary>
        public static DataResult GetShopEdit(string coid,string shopid)
        {
            var s = 1;
            var sname = "shop"+coid+shopid;
            Shop su = null;
           // var su = CacheBase.Get<Shop>(sname);
            // if(su==null)
            // {
                var u = DbBase.CommDB.Query<Shop>("select * from Shop where id = @sid and CoID = @coid",new{sid = shopid,coid=coid}).AsList();
                if(u.Count==0)
                {
                    s=3001;
                }
                else
                {
                    su = u[0];
                    //CacheBase.Set<Shop>(sname,su);
                }
            // }
            return new DataResult(s,su);
        }

        ///<summary>
        ///启用、停用店铺
        ///<summary>
        public static DataResult UptShopEnable(Dictionary<int,string> IDsDic,string Company,string UserName,bool Enable)
        {
            var s = 1;
            string contents = string.Empty;            
            string uptsql = @"update Shop set Enable = @Enable where ID in @ID";
            var args = new {ID = IDsDic.Keys.AsList(),Enable = Enable};          

            int count = DbBase.CommDB.Execute(uptsql,args);
            if(count<=0)
            {
                s=3003;
            }
            else
            {
                if(Enable)
               {
                   contents = "店铺状态启用：";
               }
               else
               {
                   contents = "店铺状态停用：";
               }
               contents+= string.Join(",", IDsDic.Values.AsList().ToArray());
               CoreUser.LogComm.InsertUserLog("修改店铺资料", "Shop", contents, UserName, Company, DateTime.Now);
            }
            
            return new DataResult(s,contents);
        }
    }   
}