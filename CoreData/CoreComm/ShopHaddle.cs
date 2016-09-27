
using CoreModels;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using CoreModels.XyComm;
using MySql.Data.MySqlClient;

namespace CoreData.CoreComm
{
    public static class ShopHaddle
    {

        ///<summary>
        ///查询店铺资料
        ///<summary>
        public static DataResult GetShopAll(ShopParam IParam)
        {            
            var result = new DataResult(1, null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string wheresql = "where 1=1";
                    if (IParam.CoID != 1)
                    {

                        wheresql = wheresql + " AND CoID=" + IParam.CoID;
                    }
                    if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
                    {
                        wheresql = wheresql + " AND Enable = " + (IParam.Enable.ToUpper() == "TRUE" ? true : false);
                    }
                    if (!string.IsNullOrEmpty(IParam.Filter))//过滤条件
                    {
                        wheresql = wheresql +
                        " AND ( ShopName LIKE '%" + IParam.Filter +
                        "%' OR ShopSite LIKE '%" + IParam.Filter +
                        "%' OR Creator LIKE '%" + IParam.Filter + "%')";
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        wheresql = wheresql + " ORDER BY " + IParam.SortField + " " + IParam.SortDirection;
                    }
                    wheresql = "select ID,ShopName,Enable,ShopSite,ShopUrl,Shopkeeper,UpdateSku,DownGoods,UpdateWayBill,TelPhone,SendAddress,CreateDate,Istoken from Shop " + wheresql;
                    var ShopLst = conn.Query<ShopQuery>(wheresql).AsList();
                    IParam.DataCount = ShopLst.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(IParam.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                    IParam.PageCount = Convert.ToInt32(pagecnt);
                    int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + IParam.PageSize;//分页            
                    IParam.ShopLst = conn.Query<ShopQuery>(wheresql).AsList();
                    result.d = IParam;
                }catch(Exception e){
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
                
            }
            return result;
        }
        ///<summary>
        ///查询单笔店铺资料
        ///<summary>
        public static DataResult ShopQuery(string coid, string shopid)
        {
            var result = new DataResult(1,null);
            var sname = "shop" + coid + shopid;

            var su = CacheBase.Get<Shop>(sname);
            if(su==null)
            {
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try
                    {
                        var u = DbBase.CommDB.Query<Shop>("select * from Shop where id = @sid and CoID = @coid", new { sid = shopid, coid = coid }).AsList();
                        if (u.Count == 0)
                        {
                            result.s = -3001;
                        }
                        else
                        {
                            su = u[0];
                            result.d = su;
                            CacheBase.Set<Shop>(sname,su);
                        }
                    }
                    catch (Exception e)
                    {
                        result.s = -1;
                        result.d= e.Message; 
                        conn.Dispose();
                    }
                }            
            }else{
                result.d = su;
            }
            return result;
        }

        ///<summary>
        ///启用、停用店铺
        ///<summary>
        public static DataResult UptShopEnable(Dictionary<int, string> IDsDic, string Company, string UserName, bool Enable,string Coid)
        {            
            var result = new DataResult(1,null);                    
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    //删除缓存
                    foreach (var item in IDsDic){
                        CacheBase.Remove("shop" + Coid + item.Key);                
                    }
                    string contents = string.Empty;
                    string uptsql = @"update Shop set Enable = @Enable where ID in @ID";
                    var args = new { ID = IDsDic.Keys.AsList(), Enable = Enable };

                    int count = conn.Execute(uptsql, args);
                    if (count <= 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        if (Enable)
                        {
                            contents = "店铺状态启用：";
                        }
                        else
                        {
                            contents = "店铺状态停用：";
                        }
                        contents += string.Join(",", IDsDic.Values.AsList().ToArray());
                        CoreUser.LogComm.InsertUserLog("修改店铺资料", "Shop", contents, UserName, Company, DateTime.Now);
                        result.d = contents;
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

        ///<summary>
        ///店铺新增
        ///<summary>
        public static DataResult InsertShop(Shop shop, int CoID, string UserName)
        {
            var result = new DataResult(1, null);
            if (ExistShop(shop.ShopName, 0, CoID))//判断店铺名称是否存在
            {
                result.s = -3002;
                result.d = "店铺名称已存在";
            }
            else
            {
                result = AddShop(shop, CoID, UserName);
            }

            return result;
        }

        ///<summary>
        ///店铺修改
        ///<summary>
        public static DataResult UpdateShop(Shop shop, int CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var sname = "shop" + CoID + shop.ID;
            string contents = string.Empty;
            if (ExistShop(shop.ShopName, shop.ID, shop.CoID))//判断店铺名称是否存在
            {
                result.s = -3005;
                //result.d = "店铺名称已存在";
            }
            else
            {

                var res = ShopQuery(shop.CoID.ToString(), shop.ID.ToString());
                var shopOld = res.d as Shop;                
                //删除原有缓存
                CacheBase.Remove(sname);
                
                if (shopOld.ShopName != shop.ShopName)
                {
                    contents = contents + "店铺名称:" + shopOld.ShopName + "=>" + shop.ShopName + ";";
                }
                if (shopOld.ShortName != shop.ShortName)
                {
                    contents = contents + "店铺简称:" + shopOld.ShortName + "=>" + shop.ShortName + ";";
                }
                if (shopOld.SitType != shop.SitType)
                {
                    contents = contents + "归属平台:" + shopOld.SitType + "." + shopOld.ShopSite + "=>" + shopOld.SitType + "." + shop.ShopSite + ";";
                }
                if (shopOld.Shopkeeper != shop.Shopkeeper)
                {
                    contents = contents + "掌柜昵称:" + shopOld.Shopkeeper + "=>" + shop.Shopkeeper + ";";
                }
                if (shopOld.SendAddress != shop.SendAddress)
                {
                    contents = contents + "发货地址:" + shopOld.SendAddress + "=>" + shop.SendAddress + ";";
                }
                if (shopOld.ShopUrl != shop.ShopUrl)
                {
                    contents = contents + "店铺网址:" + shopOld.ShopUrl + "=>" + shop.ShopUrl + ";";
                }
                if (shopOld.TelPhone != shop.TelPhone)
                {
                    contents = contents + "联系电话:" + shopOld.TelPhone + "=>" + shop.TelPhone + ";";
                }
                if (shopOld.IDcard != shop.IDcard)
                {
                    contents = contents + "身份证号:" + shopOld.IDcard + "=>" + shop.IDcard + ";";
                }
                if (shopOld.ContactName != shop.ContactName)
                {
                    contents = contents + "退货联系人:" + shopOld.ContactName + "=>" + shop.ContactName + ";";
                }
                if (shopOld.ReturnAddress != shop.ReturnAddress)
                {
                    contents = contents + "退货地址:" + shopOld.ReturnAddress + "=>" + shop.ReturnAddress + ";";
                }
                if (shopOld.ReturnMobile != shop.ReturnMobile)
                {
                    contents = contents + "退货手机:" + shopOld.ReturnMobile + "=>" + shop.ReturnMobile + ";";
                }
                if (shopOld.ReturnPhone != shop.ReturnPhone)
                {
                    contents = contents + "退货固话:" + shopOld.ReturnPhone + "=>" + shop.ReturnPhone + ";";
                }
                if (shopOld.Postcode != shop.Postcode)
                {
                    contents = contents + "退货邮编:" + shopOld.Postcode + "=>" + shop.Postcode + ";";
                }
                if (shopOld.Enable != shop.Enable)
                {
                    contents = contents + "店铺状态:" + shopOld.Enable + "=>" + shop.Enable + ";";
                }
                if (shopOld.Istoken != shop.Istoken)
                {
                    contents = contents + "接口授权:" + shopOld.Istoken + "=>" + shop.Istoken + ";";
                }
                if (shopOld.Istoken != shop.Istoken)
                {
                    contents = contents + "上传库存（自动同步）:" + shopOld.UpdateSku + "=>" + shop.UpdateSku + ";";
                }
                if (shopOld.Istoken != shop.Istoken)
                {
                    contents = contents + "下载商品（自动同步）:" + shopOld.DownGoods + "=>" + shop.DownGoods + ";";
                }
                if (shopOld.Istoken != shop.Istoken)
                {
                    contents = contents + "上传快递单（发货信息）:" + shopOld.UpdateWayBill + "=>" + shop.UpdateWayBill + ";";
                }
                if (shopOld.ShopBegin != shop.ShopBegin)
                {
                    contents = contents + "创店时间:" + shopOld.ShopBegin + "=>" + shop.ShopBegin + ";";
                }
                var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
                var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
                CommDBconn.Open();
                UserDBconn.Open();
                var TransComm = CommDBconn.BeginTransaction();
                var TransUser = UserDBconn.BeginTransaction();
                try
                {
                    string str = @"update Shop
                                    Set ShopName = @ShopName,
                                    SitType = @SitType,
                                    ShopSite = @ShopSite,
                                    ShopType = @ShopType,
                                    ShopUrl = @ShopUrl,
                                    ShopSetting = @ShopSetting,
                                    Enable = @Enable,
                                    ShortName = @ShortName,
                                    Shopkeeper = @Shopkeeper,
                                    SendAddress = @SendAddress,
                                    TelPhone = @TelPhone,
                                    IDcard = @IDcard,
                                    ContactName = @ContactName,
                                    ReturnAddress = @ReturnAddress,
                                    ReturnMobile = @ReturnMobile,
                                    ReturnPhone = @ReturnPhone,
                                    Postcode = @Postcode,
                                    UpdateSku = @UpdateSku,
                                    DownGoods = @DownGoods,
                                    UpdateWayBill = @UpdateWayBill,
                                    ShopBegin = @ShopBegin,
                                    Istoken = @Istoken,
                                    Token = @Token
                                    where
                                    ID = @ID
                                    ";
                    int count = CommDBconn.Execute(str, shop, TransComm);
                    if (count <= 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLogTran(TransUser, "修改店铺资料", "Shop", contents, UserName, CoID.ToString(), DateTime.Now);
                        CacheBase.Set<Shop>(sname, shop);//缓存
                        TransComm.Commit();
                        TransUser.Commit();
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
                finally
                {
                    TransComm.Dispose();
                    TransUser.Dispose();
                    CommDBconn.Close();
                    UserDBconn.Clone();
                }
            }
            return result;
        }

        ///<summary>
        ///店铺是否存在
        ///<summary>    
        public static Boolean ExistShop(string ShopName, int ID, int CoID)
        {
            int count = 0;
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string query = string.Empty;
                    // object param = null;
                    StringBuilder querystr = new StringBuilder();
                    querystr.Append("select * from shop where CoID = @CoID and ShopName = @ShopName");
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    p.Add("@ShopName", ShopName);
                    if (ID > 0)
                    {
                        querystr.Append(" and ID !=@ID");
                        p.Add("@ID", ID);
                    }
                     count= conn.Query<Shop>(querystr.ToString(), p).Count();
                }
                catch{
                    conn.Dispose();
                }
            }
            if (count > 0)
                return true;
            else
                return false;



            // if (ID <= 0)
            // {
            //     query = @"select * from shop where CoID = @CoID and ShopName = @ShopName";
            //     param = new { CoID = CoID, ShopName = ShopName };
            // }
            // else
            // {
            //     query = @"select * from shop where CoID = @CoID and ShopName = @ShopName and ID != @ID";
            //     param = new { CoID = CoID, ShopName = ShopName, ID = ID };
            // }
            // int count = DbBase.CommDB.Query<Shop>(query, param).Count();
            // if (count > 0)
            //     return true;
            // else
            //     return false;
        }

        ///<summary>
        ///店铺新增
        ///<summary>
        public static DataResult AddShop(Shop shop, int CoID, string UserName)
        {
            var sname = "shop" + CoID + shop.ID;
            var result = new DataResult(1, null);
            string sqlCommandText = @"INSERT INTO Shop(ShopName,
                    SitType,
                    ShopSite,
                    ShopType,
                    ShopUrl,
                    ShopSetting,
                    Enable,
                    ShortName,
                    Shopkeeper,
                    SendAddress,
                    TelPhone,
                    IDcard,
                    ContactName,
                    ReturnAddress,
                    ReturnMobile,
                    ReturnPhone,
                    Postcode,
                    UpdateSku,
                    DownGoods,
                    UpdateWayBill,
                    ShopBegin,
                    Istoken,
                    Token,
                    CoID,
                    Creator,
                    CreateDate ) VALUES(
                    @ShopName,
                    @SitType,
                    @ShopSite,
                    @ShopType,
                    @ShopUrl,
                    @ShopSetting,
                    @Enable,
                    @ShortName,
                    @Shopkeeper,
                    @SendAddress,
                    @TelPhone,
                    @IDcard,
                    @ContactName,
                    @ReturnAddress,
                    @ReturnMobile,
                    @ReturnPhone,
                    @Postcode,
                    @UpdateSku,
                    @DownGoods,
                    @UpdateWayBill,
                    @ShopBegin,
                    @Istoken,
                    @Token, 
                    @CoID,
                    @Creator,
                    @CreateDate 
                    )";
            var sp = new Shop();
            sp.ShopName = shop.ShopName;
            sp.SitType = shop.SitType;
            sp.ShopSite = shop.ShopSite;
            sp.ShopType = shop.ShopSite;
            sp.ShopUrl = shop.ShopUrl;
            sp.ShopSetting = shop.ShopSetting;
            sp.Enable = shop.Enable;
            sp.ShortName = shop.ShortName;
            sp.Shopkeeper = shop.Shopkeeper;
            sp.SendAddress = shop.SendAddress;
            sp.TelPhone = shop.TelPhone;
            sp.IDcard = shop.IDcard;
            sp.ContactName = shop.ContactName;
            sp.ReturnAddress = shop.ReturnAddress;
            sp.ReturnMobile = shop.ReturnMobile;
            sp.ReturnPhone = shop.ReturnPhone;
            sp.Postcode = shop.Postcode;
            sp.UpdateSku = shop.UpdateSku;
            sp.DownGoods = shop.DownGoods;
            sp.UpdateWayBill = shop.UpdateWayBill;
            sp.ShopBegin = shop.ShopBegin;
            sp.Istoken = shop.Istoken;
            sp.Token = shop.Token;
            sp.CoID = CoID;
            sp.Creator = UserName;
            sp.CreateDate = DateTime.Now;

            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            CommDBconn.Open();
            UserDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            var TransUser = UserDBconn.BeginTransaction();
            try
            {
                int count = CommDBconn.Execute(sqlCommandText, sp, TransComm);
                // int count = CommDBconn.Execute(sqlCommandText, sp);
                if (count <= 0)
                {
                    result.s = -3002;
                }
                else
                {
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "新增店铺资料", "Shop", shop.ShopName, UserName, CoID.ToString(), DateTime.Now);
                    CacheBase.Set<Shop>(sname, shop);//缓存
                }
                TransComm.Commit();
                TransUser.Commit();
            }
            catch (Exception e)
            {
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransComm.Dispose();
                TransUser.Dispose();
                CommDBconn.Close();
                UserDBconn.Clone();
            }

            return result;
        }

        //summary:
        //  获取所有授权店铺
        public static DataResult GetTokenShopLst(int CoID)
        {
            var res = new DataResult(1, null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string query = "select * from shop where CoID=@CoID and Istoken=1 and Token!='' and Enable=true";
                    var Lst = conn.Query<Shop>(query, new { CoID = CoID }).ToList();
                    if (Lst.Count == 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        res.d = Lst;
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d= e.Message; 
                    conn.Dispose();
                }
            }                        
            return res;
        }

        public static DataResult GetOfflineShopLst(int CoID)
        {
             var res = new DataResult(1, null);
             using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string query = "select * from shop where CoID=@CoID and SitType=35 and Enable=true";
                    var Lst = conn.Query<Shop>(query, new { CoID = CoID }).ToList();
                    if (Lst.Count == 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        res.d = Lst;
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d= e.Message; 
                    conn.Dispose();
                }
            }
            
            return res;
        }

        public static DataResult TokenExpired(string shopid,string coid){
            var res = new DataResult(1, null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var rnt= conn.Execute("UPDATE shop SET shop.Token = 2 WHERE shop.ID = "+shopid);
                    if(rnt>0){
                        CacheBase.Remove("shop" + coid + shopid);
                        res.s = 1;
                    }else{
                        res.s = -1;
                    }
                }
                catch(Exception e)
                {
                    res.s = -1;
                    res.d= e.Message; 
                    conn.Dispose();
                }
            }
            return res;
        }




    }
}