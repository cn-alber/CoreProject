using CoreModels;
using Dapper;
using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using CoreModels.XyComm;
using CoreModels.XyApi.Tmall;
using MySql.Data.MySqlClient;
using CoreData.CoreApi;

namespace CoreData.CoreComm
{
    public static class CustomKindHaddle
    {

        #region 获取类目列表
        public static DataResult GetKindLst(CusKindParam IParam)
        {
            var result = new DataResult(1, null);
            var Kind = new CustomKindData();
            string sql = @"SELECT * FROM customkind WHERE Type=@Type AND CoID=@CoID AND IsDelete=0 AND ParentID = @ParentID";
            var p = new DynamicParameters();
            p.Add("@Type", IParam.Type);
            p.Add("@CoID", IParam.CoID);
            p.Add("@ParentID", IParam.ParentID);
            if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
            {
                sql = sql + " AND Enable = @Enable";
                p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
            }
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    var Children = conn.Query<CustomKindData>(sql, p).AsList();
                    if (IParam.ParentID > 0)
                    {
                        var res = GetKind(IParam.ParentID, IParam.CoID.ToString());
                        Kind = res.d as CustomKindData;
                    }
                    Kind.Children = Children;
                    if(Children.Count > 0 && IParam.ParentID != 0){
                        System.Threading.Tasks.Task.Factory.StartNew(()=>{
                            conn.Execute("UPDATE customkind SET IsParent = TRUE WHERE ID = "+IParam.ParentID);
                        });
                        
                    }
                    // foreach (var p in ParentLst)
                    // {
                    //     var res = GetKindLst(Type, p.ID, CoID);
                    //     if (res.s == 1)
                    //     {
                    //         // p.Children = res.d as List<CustomKindData>;
                    //     }
                    //     else
                    //         break;
                    // }
                    result.d = Kind;
                    conn.Close();
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion

        #region 获取类目列表 -- 公共
        public static DataResult GetCustomCats(CusKindParam IParam)
        {
            var result = new DataResult(1, null);
            var Kind = new CustomKindData();
            string sql = @"SELECT customkind.ID ,customkind.KindName as NAME ,customkind.IsParent  FROM customkind WHERE Type=@Type AND CoID=@CoID AND IsDelete=0 AND ParentID = @ParentID";
            var p = new DynamicParameters();
            p.Add("@Type", IParam.Type);
            p.Add("@CoID", IParam.CoID);
            p.Add("@ParentID", IParam.ParentID);
            if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
            {
                sql = sql + " AND Enable = @Enable";
                p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
            }
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    result.d = conn.Query<CustomCategory>(sql, p).AsList();                                        
                    conn.Close();
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion




        #region 获取标准类目列表
        public static DataResult GetStdKindLst(int ParentID)
        {
            var result = new DataResult(1, null);
            var KindLst = new List<ItemCateStdData>();
            string sql = @"SELECT id,
                                `name`,
                                parent_id,
                                is_tb_parent AS is_parent
                            FROM Item_cates_standard
                            WHERE parent_id=@ParentID";
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    var ParentLst = conn.Query<ItemCateStdData>(sql, new { ParentID = ParentID }).AsList();
                    KindLst.AddRange(ParentLst);
                    
                    // foreach (var p in ParentLst)
                    // {
                    //     var res = GetStdKindLst(p.id);
                    //     if (res.s == 1)
                    //     {
                    //         p.Children = res.d as List<ItemCateStdData>;
                    //     }
                    //     else
                    //         break;
                    // }
                    result.d = ParentLst;
                    conn.Close();
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion 

        #region 获取单笔类目资料
        public static DataResult GetKind(int KindID, string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string cname = "customkind" + CoID + KindID;
                    var KindNode = CacheBase.Get<CustomKindData>(cname);//读取缓存
                    if (KindNode == null)
                    {
                        KindNode = conn.QueryFirst<CustomKindData>("SELECT * FROM customkind WHERE ID=@ID", new { ID = KindID });
                        CacheBase.Set(cname, KindNode);//新增缓存
                    }
                    result.d = KindNode;
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                    // CoreHelper.Debuger.exception(e);
                }
                return result;
            }
        }
        #endregion

        #region 获取单笔类目属性资料
        public static DataResult GetSkuKindProps(int KindID, string Enable, string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    // string cname = "customkindProps" + CoID + KindID;
                    // var props = CacheBase.Get<List<Customkind_props>>(cname);//读取缓存
                    // if (props == null)
                    // {
                    string sql = "SELECT * FROM customkind_props WHERE IsDelete=0 AND kindid=@ID AND CoID=@CoID";
                    var p = new DynamicParameters();
                    p.Add("@ID", KindID);
                    p.Add("@CoID",CoID);
                    if (!string.IsNullOrEmpty(Enable) && Enable.ToUpper() != "ALL")
                    {
                        sql = sql + " AND Enable=@Enable";
                        p.Add("@Enable", Enable.ToUpper() == "TRUE" ? true : false);
                    }
                    var props = conn.Query<Customkind_props>(sql, p).AsList();
                    // CacheBase.Set(cname, props);//新增缓存
                    // }
                    if (props.Count > 0)
                    {
                        string ValSql = @"SELECT propid,name from Customkind_props_value WHERE CoID=@CoID AND kindid = @KindID AND Enable=1 AND IsDelete=0";
                        var ValLst = conn.Query<Customkind_props_value>(ValSql, new { CoID = CoID, KindID = KindID });
                        foreach (var prop in props)
                        {
                            prop.ValLst = ValLst.Where(a => a.propid == prop.id).OrderBy(b => b.Order).AsList().Select(c => c.name).AsList();
                        }
                    }
                    result.d = props;
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
                return result;
            }
        }

        public static DataResult GetSkuKindProp(int PorpID, string CoID)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    // string cname = "customkindProps" + CoID + KindID;
                    // var props = CacheBase.Get<List<Customkind_props>>(cname);//读取缓存
                    // if (props == null)
                    // {
                    string sql = "SELECT * FROM customkind_props WHERE id=@ID AND CoID=@CoID AND IsDelete=0";
                    var p = new DynamicParameters();
                    p.Add("@ID", PorpID);
                    p.Add("@CoID", CoID);
                    var prop = conn.QueryFirst<Customkind_props>(sql, p);
                    // CacheBase.Set(cname, props);//新增缓存
                    // }
                    if (prop != null)
                    {
                        string ValSql = @"SELECT propid,name from Customkind_props_value WHERE CoID=@CoID AND propid = @propid AND Enable=1 AND IsDelete=0";
                        var ValLst = conn.Query<Customkind_props_value>(ValSql, new { CoID = CoID, propid = PorpID });
                        prop.ValLst = ValLst.Where(a => a.propid == prop.id).OrderBy(b => b.Order).AsList().Select(c => c.name).AsList();
                    }
                    result.d = prop;
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
                return result;
            }
        }
        #endregion

        #region 新增类目资料
        public static DataResult InsertKind(CustomKind kind)
        {
            var result = ExistKind(kind.KindName, kind.ParentID, 0, kind.CoID);//判断类目名称是否已存在
            if (result.s == 1)
            {                
                result = AddKind(kind);
            }
            return result;
        }

        public static DataResult AddKind(CustomKind kind)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    int count = conn.Execute(AddKindSql(), kind,Trans);
                    kind.ID = conn.QueryFirst<int>("select LAST_INSERT_ID()",Trans);
                    if (count < 0)
                    {
                        result.s = -3002;
                    }
                    else
                    {
                        var SkuPropLst = new List<Customkind_skuprops>();
                        //现有的自定义类目
                        if (kind.mode == 2)//新增自定义规格
                        {
                            kind.norm = string.Join(",", kind.NormLst.ToArray());
                            var OldPropLst = conn.Query<skuprops_data>(@"SELECT distinct pid,name FROM customkind_skuprops WHERE CoID=@CoID AND tb_cid=0",new{CoID=kind.CoID}).AsList();
                            foreach(var pname in kind.NormLst)
                            {
                                var prop = new Customkind_skuprops();
                                prop.CoID = kind.CoID;
                                prop.kindid = kind.ID;
                                prop.name = pname;
                                prop.Creator = kind.Creator;
                                prop.CreateDate = kind.CreateDate;
                                if(OldPropLst.Select(a=>a.name).Contains(pname))
                                {
                                    prop.pid = OldPropLst.Where(a=>a.name==pname).Select(a=>a.pid).First();
                                }
                                else
                                {
                                    prop.pid = long.Parse(CommHaddle.GetRecordID(kind.CoID));                                
                                }
                                SkuPropLst.Add(prop);
                            }  
                            if(SkuPropLst.Count>0)
                            {
                                conn.Execute(AddSkuPropSql(),SkuPropLst,Trans);
                            }                      
                        }   
                        // var Kind = conn.QueryFirst<CustomKind>("SELECT * FROM customkind WHERE CoID=@CoID AND ID=@KindID AND IsDelete=0", new { CoID = kind.CoID, KindID = kind.ID });
                        string cname = "customkind" + kind.CoID + kind.ID;
                        CacheBase.Set(cname, kind);//新增缓存
                        Trans.Commit();
                        CoreUser.LogComm.InsertUserLog("新增" + kind.Type, "CustomKind", kind.KindName, kind.Creator, kind.CoID, DateTime.Now);
                    }
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    result.s = -1;
                    result.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Dispose();
                    conn.Close();
                }
                return result;
            }
        }
        #endregion

        #region 修改类目资料
        public static DataResult UptKind(CustomKind Kind, string CoID, string UserName)
        {
            var result = ExistKind(Kind.KindName, Kind.ParentID, Kind.ID, int.Parse(CoID));//判断类目名称是否已存在
            var cname = "customkind" + CoID.ToString() + Kind.ID;
            string contents = string.Empty;
            if (result.s == 1)
            {
                using (var conn = new MySqlConnection(DbBase.CommConnectString))
                {
                    try
                    {
                        var res = GetKind(Kind.ID, CoID);//原类目资料
                        var OldKind = res.d as CustomKindData;
                        // res = GetKind(Kind.ParentID, CoID);//上级类目资料
                        // var pkind = res.d as CustomKind;
                        // string fullname = pkind.FullName + "=>" + Kind.KindName;
                        if (OldKind.KindName != Kind.KindName)
                        {
                            contents = contents + "类目名称:" + OldKind.KindName + "=>" + Kind.KindName + ";";
                        }
                        if (OldKind.Order != Kind.Order)
                        {
                            contents = contents + "类目排序:" + OldKind.Order + "=>" + Kind.Order + ";";
                        }
                        // if (OldKind.FullName != Kind.FullName)
                        // {
                        //     contents = contents + "类目全名:" + OldKind.FullName + "=>" + Kind.FullName + ";";
                        // }
                        if (OldKind.Enable != Kind.Enable)
                        {
                            contents = contents + "类目状态" + OldKind.Enable.ToString() + "=>" + Kind.Enable.ToString();
                        }
                        if (!string.IsNullOrEmpty(contents))
                        {
                            string sql = "update customkind Set KindName=@KindName,`Order`=@Order,Enable=@Enable,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE ID=@ID";
                            var p = new DynamicParameters();
                            p.Add("@KindName", Kind.KindName);
                            // p.Add("@FullName", fullname);
                            p.Add("@Order", Kind.Order);
                            p.Add("@Enable", Kind.Enable);
                            p.Add("@Modifier", UserName);
                            p.Add("@ModifyDate", DateTime.Now);
                            p.Add("@ID", Kind.ID);
                            int count = conn.Execute(sql, p);
                            if (count < 0)
                            {
                                result.s = -3003;
                            }
                            else
                            {
                                CacheBase.Remove(cname);//清除缓存
                                var kind = conn.QueryFirst<CustomKind>("select * from customkind WHERE ID=@ID", new { ID = Kind.ID });
                                CacheBase.Set(cname, kind);//添加缓存
                                CoreUser.LogComm.InsertUserLog("修改" + "商品类目", "CustomKind", contents, UserName, int.Parse(CoID), DateTime.Now);//操作记录
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        result.s = -1;
                        result.d = e.Message;
                    }
                }
            }
            return result;
        }
        #endregion

        #region 检查商品类目是否存在
        public static DataResult ExistKind(string KindName, int ParentID, int ID, int CoID)
        {
            int count = 0;
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    StringBuilder querystr = new StringBuilder();
                    querystr.Append("select count(ID) from customkind where CoID = @CoID and ParentID=@ParentID and KindName = @KindName");
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    p.Add("@KindName", KindName);
                    p.Add("@ParentID", ParentID);
                    if (ID > 0)
                    {
                        querystr.Append(" and ID !=@ID");
                        p.Add("@ID", ID);
                    }
                    count = conn.QueryFirst<int>(querystr.ToString(), p);
                    if (count > 0)
                    {
                        res.s = -1;
                        res.d = "类目名称已存在";
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                return res;
            }
        }
        #endregion

        #region 删除商品类目
        public static DataResult DelKind(List<int> IDLst, int CoID, string UserName)
        {
            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string sql = "update customkind Set IsDelete=@IsDelete,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE ID in @IDLst";
                    var p = new DynamicParameters();
                    p.Add("@IsDelete", 1);
                    p.Add("@Modifier", UserName);
                    p.Add("@ModifyDate", DateTime.Now);
                    p.Add("@IDLst", IDLst);
                    int count = conn.Execute(sql, p);
                    if (count > 0)
                    {
                        foreach (var ID in IDLst)
                        {
                            var cname = "customkind" + CoID + ID;
                            CacheBase.Remove(cname);
                        }
                        string contents = "删除类目=>" + string.Join(",", IDLst);
                        CoreUser.LogComm.InsertUserLog("删除类目资料", "CustomKind", contents, UserName, CoID, DateTime.Now);
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion   


        #region 状态(停用|启用)
        public static DataResult UptKindEnable(List<int> IDLst, string CoID, string UserName, bool Enable)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                conn.Open();
                var Trans = conn.BeginTransaction();
                try
                {
                    string contents = string.Empty;
                    string uptsql = @"update customkind set Enable = @Enable,Modifier=@Modifier,ModifyDate=@ModifyDate where ID in @IDLst";
                    var p = new DynamicParameters();
                    p.Add("@Enable", Enable);
                    p.Add("@Modifier", UserName);
                    p.Add("@ModifyDate", DateTime.Now);
                    p.Add("@IDLst", IDLst);
                    int count = conn.Execute(uptsql, p, Trans);
                    if (count < 0)
                    {
                        res.s = -3003;
                    }
                    else
                    {
                        if (Enable)
                        {
                            contents = "类目状态启用：";
                            res.s = 3001;
                        }
                        else
                        {
                            contents = "类目状态停用：";
                            res.s = 3002;
                        }
                        contents += string.Join(",", IDLst.ToArray());
                        CoreUser.LogComm.InsertUserLog("修改类目状态", "CustomKind", contents, UserName, int.Parse(CoID), DateTime.Now);
                        if (res.s > 0)
                        {
                            Trans.Commit();
                        }
                    }
                }
                catch (Exception e)
                {
                    Trans.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    Trans.Dispose();
                    conn.Close();
                }
            }

            return res;
        }
        #endregion  

        #region 获取商品类目列表
        public static DataResult GetCommKindLst(CusKindParam IParam)
        {
            var result = new DataResult(1, null);
            string sql = @"SELECT * FROM customkind WHERE Type=@Type AND CoID=@CoID AND ParentID = @ParentID AND Enable=1 AND IsDelete=0";
            var p = new DynamicParameters();
            p.Add("@Type", IParam.Type);
            p.Add("@CoID", IParam.CoID);
            p.Add("@ParentID", IParam.ParentID);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    var Children = conn.Query<CustomKindData>(sql, p).AsList();
                    result.d = Children;
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion

        #region 新增标准商品类目
        public static DataResult InsertKindProps(CustomKind IParam)
        {
            var result = new DataResult(1, null);
            string sql = "SELECT * FROM item_cates_standard WHERE id=@ID";
            var conn = new MySqlConnection(DbBase.CommConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                var Kind_standard = conn.QueryFirst<Item_cates_standard>(sql, new { ID = IParam.ID });
                if (Kind_standard != null)
                {
                    string querysql = "SELECT count(ID) FROM customkind WHERE KindName=@name and CoID=@CoID and ParentID=@ParentID and IsDelete=0";
                    int count = conn.QueryFirst<int>(querysql, new { name = Kind_standard.name, CoID = IParam.CoID, ParentID = IParam.ParentID });
                    if (count > 0)
                    {
                        result.s = -1;
                        result.d = "商品类目已存在！";
                    }
                    else
                    {
                        var NewKind = new CustomKind();
                        NewKind.CoID = IParam.CoID;
                        NewKind.KindName = Kind_standard.name;
                        NewKind.Type = IParam.Type;
                        NewKind.ParentID = IParam.ParentID;
                        NewKind.Enable = IParam.Enable;
                        NewKind.Creator = IParam.Creator;
                        NewKind.CreateDate = IParam.CreateDate;
                        NewKind.tb_cid = Kind_standard.tb_cid;
                        conn.Execute(AddKindSql(), NewKind, Trans);
                        int kindid = conn.QueryFirst<int>("select LAST_INSERT_ID()", Trans);//新增商品类目
                                                                                            //新增商品类目属性
                        result = TmallHaddle.itemProps(Kind_standard.tb_cid.ToString());
                        if (result.s == 1)
                        {
                            dynamic item_sku_props = result.d;
                            var ItemPropLst = new List<Customkind_props>();//商品类目属性
                            var ItemPropValLst = new List<Customkind_props_value>();//商品类目属性之可选属性值
                            var SkuPropLst = new List<Customkind_skuprops>();//Sku属性（颜色|尺码）
                            var SkuPropValLst = new List<Customkind_skuprops_value>();
                            var OldSkuValLst = conn.Query<skuprops_value_data>("SELECT pid,vid FROM Customkind_skuprops_value WHERE CoID=@CoID", new { CoID = IParam.CoID });
                            // var SizeLst = new List<CoreSize>();//尺码熟悉
                            int order = 1;
                            foreach (var item in item_sku_props["item_props"])
                            {
                                var prop = new Customkind_props();
                                if (item["is_material"] != null)
                                {
                                    prop.is_material = item["is_material"];
                                }
                                if (item["is_allow_alias"] != null)
                                {
                                    prop.is_allow_alias = item["is_allow_alias"];
                                }
                                if (item["is_enum_prop"] != null)
                                {
                                    prop.is_enum_prop = item["is_enum_prop"];
                                }
                                if (item["is_input_prop"] != null)
                                {
                                    prop.is_input_prop = item["is_input_prop"];
                                }
                                if (item["is_key_prop"] != null)
                                {
                                    prop.is_key_prop = item["is_key_prop"];
                                }
                                if (item["is_sale_prop"] != null)
                                {
                                    prop.is_sale_prop = item["is_sale_prop"];
                                }
                                if (item["must"] != null)
                                {
                                    prop.must = item["must"];
                                }
                                if (item["multi"] != null)
                                {
                                    prop.multi = item["multi"];
                                }
                                int ValOrder = 1;
                                // if(item["material_do"]!=null&& item["material_do"]["materials"] != null)
                                if (item["prop_values"] != null && item["prop_values"]["prop_value"] != null)
                                {
                                    foreach (var v in item["prop_values"]["prop_value"])
                                    {
                                        var PropVal = new Customkind_props_value();
                                        PropVal.vid = v["vid"];
                                        PropVal.name = v["name"];
                                        PropVal.kindid = kindid;
                                        PropVal.pid = item["pid"];
                                        PropVal.tb_cid = Kind_standard.tb_cid;
                                        PropVal.Creator = IParam.Creator;
                                        PropVal.CreateDate = IParam.CreateDate;
                                        PropVal.CoID = IParam.CoID;
                                        PropVal.Order = ValOrder;
                                        ValOrder++;
                                        ItemPropValLst.Add(PropVal);
                                    }
                                    // prop.values = Newtonsoft.Json.JsonConvert.SerializeObject(item["prop_values"]);
                                }
                                prop.pid = item["pid"];
                                prop.name = item["name"];
                                prop.kindid = kindid;
                                prop.tb_cid = Kind_standard.tb_cid;
                                prop.Creator = IParam.Creator;
                                prop.CreateDate = IParam.CreateDate;
                                prop.CoID = IParam.CoID;
                                prop.Order = order;
                                order++;
                                ItemPropLst.Add(prop);
                            }

                            foreach (var item in item_sku_props["sku_props"])
                            {
                                var skupid = item["pid"];
                                var prop = new Customkind_skuprops();
                                prop.CoID = IParam.CoID;
                                prop.kindid = kindid;
                                prop.name = item["name"];
                                prop.pid = skupid;
                                prop.tb_cid = Kind_standard.tb_cid;
                                prop.is_color_prop = item["is_color_prop"];
                                prop.Creator = IParam.Creator;
                                prop.CreateDate = IParam.CreateDate;
                                SkuPropLst.Add(prop);
                                if (item["prop_values"] != null && item["prop_values"]["prop_value"] != null)
                                {
                                    foreach (var v in item["prop_values"]["prop_value"])
                                    {
                                        long vid = v["vid"];
                                        if (OldSkuValLst.Where(a => a.vid == vid && a.pid == prop.pid).Count() == 0)
                                        {
                                            var val = new Customkind_skuprops_value();
                                            val.vid = vid;
                                            val.pid = skupid;
                                            val.name = v["name"];
                                            val.Creator = IParam.Creator;
                                            val.CreateDate = IParam.CreateDate;
                                            val.CoID = IParam.CoID;
                                            SkuPropValLst.Add(val);
                                        }
                                    }
                                }
                                // if (item["is_color_prop"] != null && item["is_color_prop"] == true)//新增颜色属性
                                // {
                                //     foreach (var col in item["prop_values"]["prop_value"])
                                //     {
                                //         var color = new CoreColor();
                                //         color.colorid = col["vid"];
                                //         color.vid = col["vid"];
                                //         color.name = col["name"];
                                //         color.CoID = IParam.CoID;
                                //         color.Creator = IParam.Creator;
                                //         color.CreateDate = IParam.CreateDate;
                                //         color.pid = skupid;
                                //         color.tb_cid = Kind_standard.tb_cid;
                                //         color.kindid = kindid;
                                //         ColorLst.Add(color);
                                //     }
                                // }
                                // else
                                // {
                                //     foreach (var col in item["prop_values"]["prop_value"])
                                //     {
                                //         var size = new CoreSize();
                                //         size.sizeid = col["vid"];
                                //         size.vid = col["vid"];
                                //         size.name = col["name"];
                                //         size.CoID = IParam.CoID;
                                //         size.Creator = IParam.Creator;
                                //         size.CreateDate = IParam.CreateDate;
                                //         size.pid = skupid;
                                //         size.tb_cid = Kind_standard.tb_cid;
                                //         size.kindid = kindid;
                                //         SizeLst.Add(size);
                                //     }
                                // }
                            }
                            if (ItemPropLst.Count > 0)
                            {
                                conn.Execute(AddKindPorpSql(), ItemPropLst, Trans);
                            }
                            if(SkuPropLst.Count>0)
                            {
                                conn.Execute(AddSkuPropSql(),SkuPropLst,Trans);
                            }
                            if (SkuPropValLst.Count > 0)
                            {
                                conn.Execute(AddSkuPropValSql(),SkuPropValLst,Trans);
                            }
                            // if (ColorLst.Count > 0)
                            // {
                            //     conn.Execute(AddColorSql(), ColorLst, Trans);
                            // }
                            // if (SizeLst.Count > 0)
                            // {
                            //     conn.Execute(AddSizeSql(), SizeLst, Trans);
                            // }
                            if (ItemPropValLst.Count > 0)
                            {
                                conn.Execute(AddKindPropValueSql(), ItemPropValLst, Trans);
                                string uptvalsql = @"UPDATE customkind_props_value,
                                                    customkind_props
                                                    SET customkind_props_value.propid = customkind_props.id
                                                    WHERE
                                                        customkind_props_value.kindid = customkind_props.kindid
                                                    AND customkind_props_value.pid = customkind_props.pid
                                                    AND customkind_props_value.kindid = @kindid";
                                conn.Execute(uptvalsql, new { kindid = kindid }, Trans);
                            }
                            Trans.Commit();
                            CoreUser.LogComm.InsertUserLog("新增商品类目", "Customkind", "新增标准类目" + Kind_standard.name, IParam.Creator, IParam.CoID, DateTime.Now);
                            result.d = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trans.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                Trans.Dispose();
                conn.Dispose();
                conn.Close();
            }
            return result;
        }
        #endregion

        #region 导入淘宝自定义类目
        public static DataResult InsertTmaoKind(int CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CommConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                //获取用户自定义类目
                var NewKindLst = new List<CustomKind>();
                var UptKindLst = new List<CustomKind>();
                result = CoreData.CoreApi.TmallHaddle.GetSellercatsList("南极人羽绒旗舰店");
                var TmaoDataLst = result.d as List<cat_item>;
                //转译成待新增商品类目
                var res = AddTmaoKind(TmaoDataLst, CoID, UserName);
                var KindLst = res.d as List<CustomKind>;
                //判断类目是否已存在
                var CidLst = KindLst.Select(a => a.cid).AsList();
                var OldLst = conn.Query<CustomKind>("SELECT * FROM customkind WHERE CoID=@CoID AND cid in @CidLst", new { CoID = CoID, CidLst = CidLst }).AsList();
                //已存在的类目更新
                if (OldLst.Count > 0)
                {
                    foreach (var Old in OldLst)
                    {
                        CustomKind uptkind = KindLst.First(a => a.cid == Old.cid);
                        if (uptkind != null)
                        {
                            uptkind.ID = Old.ID;
                            uptkind.Modifier = UserName;
                            uptkind.ModifyDate = DateTime.Now.ToString();
                        }
                    }
                }

                NewKindLst = KindLst.Where(a => a.ID == 0).AsList();
                UptKindLst = KindLst.Where(a => a.ID > 0).AsList();
                if (NewKindLst.Count > 0)
                {
                    conn.Execute(AddKindSql(), NewKindLst);
                }

                if (UptKindLst.Count > 0)
                {
                    string uptsql = @"UPDATE customkind
                                SET KindName =@KindName,
                                    `Order` =@Order,
                                    cid =@cid,
                                    parent_cid =@parent_cid,
                                    Modifier =@Modifier,
                                    ModifyDate =@ModifyDate,
                                    IsDelete=0
                                WHERE
                                    ID =@ID";
                    conn.Execute(uptsql, UptKindLst);
                }

                string uptsql2 = @"UPDATE customkind,
                                    customkind AS m
                                    SET customkind.ParentID = m.ID
                                    WHERE
                                        customkind.parent_cid = m.cid
                                    AND m.cid > 0
                                    AND customkind.parent_cid > 0
                                    AND m.cid in @CidLst";
                conn.Execute(uptsql2, new { CidLst = KindLst.Select(a => a.cid).AsList() });
                Trans.Commit();
                CoreUser.LogComm.InsertUserLog("新增商品类目", "Customkind", "导入淘宝商品自定义类目", UserName, CoID, DateTime.Now);
                result.d = "";

            }
            catch (Exception e)
            {
                Trans.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                Trans.Dispose();
                conn.Dispose();
                conn.Close();
            }
            return result;
        }
        public static DataResult AddTmaoKind(List<cat_item> TmaoDataLst, int CoID, string UserName)
        {
            var result = new DataResult(1, null);
            var KindLst = new List<CustomKind>();
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    foreach (var Tmao in TmaoDataLst)
                    {
                        var kind = new CustomKind();
                        kind.CoID = CoID;
                        kind.KindName = Tmao.name;
                        kind.cid = Tmao.cid;
                        kind.parent_cid = Tmao.parent_cid;
                        kind.Order = Tmao.sort_order;
                        kind.Creator = UserName;
                        kind.CreateDate = DateTime.Now.ToString();
                        KindLst.Add(kind);
                        if (Tmao.children != null && Tmao.children.Count > 0)
                        {
                            var res = AddTmaoKind(Tmao.children, CoID, UserName);
                            var ChildLst = res.d as List<CustomKind>;
                            KindLst.AddRange(ChildLst);
                        }
                        result.d = KindLst;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }


            }
            return result;
        }
        #endregion


        #region 获取复制目标类目
        public static DataResult GetCopyToKindLst(string CoID, string Enable)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string sql = @"SELECT ID,KindName,ParentID FROM customkind WHERE CoID=@CoID AND IsDelete=0";
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    if (!string.IsNullOrEmpty(Enable) && Enable.ToUpper() != "ALL")
                    {
                        sql = sql + " AND Enable=@Enable";
                        p.Add("@Enable", Enable.ToUpper() == "TRUE" ? true : false);
                    }
                    var NameLst = conn.Query<CustomKindname>(sql, p).AsList();
                    res.d = NameLst;

                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
            }
            return res;
        }
        #endregion



        #region 获取标准类目列表
        public static DataResult GetKindNameLst(string CoID, int ParentID, string Enable)
        {
            var result = new DataResult(1, null);
            var KindLst = new List<CustomKindname>();
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string sql = @"SELECT ID,KindName,Enable FROM customkind WHERE CoID=@CoID AND IsDelete=0 AND ParentID=@ParentID";
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    p.Add("@ParentID", ParentID);
                    // if (!string.IsNullOrEmpty(Enable) && Enable.ToUpper() != "ALL")
                    // {
                    //     sql = sql + " AND Enable=@Enable";
                    //     p.Add("@Enable", Enable.ToUpper() == "TRUE" ? true : false);
                    // }
                    var ParentLst = conn.Query<CustomKindname>(sql, p).AsList();
                    KindLst.AddRange(ParentLst);
                    foreach (var kind in ParentLst)
                    {
                        var res = GetKindNameLst(CoID, kind.ID, Enable);
                        if (res.s == 1)
                        {
                            kind.Children = res.d as List<CustomKindname>;
                        }
                        else
                            break;
                    }
                    result.d = ParentLst;
                    conn.Close();
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }
        #endregion     

        #region 新增商品类目
        public static string AddKindSql()
        {
            string sql = @"INSERT INTO customkind(
                                        Type,
                                        KindName,
                                        FullName,
                                        Enable,
                                        `Order`,
                                        ParentID,
                                        CoID,
                                        tb_cid,
                                        cid,
                                        parent_cid,
                                        pic_url,
                                        mode,
                                        norm,
                                        Creator,
                                        CreateDate
                                        ) VALUES(
                                        @Type,
                                        @KindName,
                                        @FullName,
                                        @Enable,
                                        @Order,
                                        @ParentID,
                                        @CoID,
                                        @tb_cid,
                                        @cid,
                                        @parent_cid,
                                        @pic_url,
                                        @mode,
                                        @norm,
                                        @Creator,
                                        @CreateDate
                                        ) ";
            return sql;
        }
        #endregion

        #region 新增商品属性
        public static string AddKindPorpSql()
        {
            string sql = @" INSERT INTO customkind_props(
                                    `name`,
                                    kindid,
                                    pid,
                                    is_allow_alias,
                                    is_enum_prop,
                                    is_input_prop,
                                    is_key_prop,
                                    is_sale_prop,
                                    must,
                                    multi,
                                    tb_cid,
                                    `values`,
                                    Enable,
                                    Creator,
                                    CreateDate,
                                    CoID ) 
                            VALUES (
                                    @name,
                                    @kindid,
                                    @pid,
                                    @is_allow_alias,
                                    @is_enum_prop,
                                    @is_input_prop,
                                    @is_key_prop,
                                    @is_sale_prop,
                                    @must,
                                    @multi,
                                    @tb_cid,
                                    @values,
                                    @Enable,
                                    @Creator,
                                    @CreateDate,
                                    @CoID ) ";
            return sql;
        }
        #endregion
        #region 新增类目属性可选值
        public static string AddKindPropValueSql()
        {
            string sql = @"INSERT INTO customkind_props_value (
                                        `name`,
                                        propid,
                                        kindid,
                                        vid,
                                        pid,
                                        tb_cid,
                                        `Order`,
                                        `Enable`,
                                        ParentID,
                                        Creator,
                                        CreateDate,
                                        Modifier,
                                        ModifyDate,
                                        CoID
                                    )
                                    VALUES
                                        (
                                            @name,
                                            @propid,
                                            @kindid,
                                            @vid,
                                            @pid,
                                            @tb_cid,
                                            @ORDER,
                                            @ENABLE,
                                            @ParentID,
                                            @Creator,
                                            @CreateDate,
                                            @Modifier,
                                            @ModifyDate,
                                            @CoID
                                        ) ";
            return sql;
        }
        #endregion
        #region 新增商品类目颜色
        public static string AddColorSql()
        {
            string sql = @" INSERT INTO corecolor(
                                        colorid,
                                        vid,
                                        `name`,
                                        kindid,
                                        pid,
                                        tb_cid,
                                        CoID,
                                        Creator,
                                        CreateDate)
                                    VALUES (
                                        @colorid,
                                        @vid,
                                        @name,
                                        @kindid,
                                        @pid,
                                        @tb_cid,
                                        @CoID,
                                        @Creator,
                                        @CreateDate
                                        )";
            return sql;
        }
        #endregion
        #region 新增商品类目尺寸
        public static string AddSizeSql()
        {
            string sql = @" INSERT INTO coresize(
                                        sizeid,
                                        `name`,
                                        kindid,
                                        pid,
                                        tb_cid,
                                        vid,
                                        CoID,
                                        Creator,
                                        CreateDate)
                                    VALUES (
                                        @sizeid,
                                        @name,
                                        @kindid,
                                        @pid,
                                        @tb_cid,
                                        @vid,
                                        @CoID,
                                        @Creator,
                                        @CreateDate
                                        )";
            return sql;
        }
        #endregion

        #region 新增Sku属性
         public static string AddSkuPropSql()
        {
            string sql = @" INSERT INTO customkind_skuprops(
                                        kindid,
                                        `name`,
                                        pid,
                                        tb_cid,
                                        is_color_prop,
                                        `Order`,
                                        `Enable`,
                                        Creator,
                                        CreateDate,
                                        CoID
                                    ) VALUES (
                                        @kindid,
                                        @name,
                                        @pid,
                                        @tb_cid,
                                        @is_color_prop,
                                        @Order,
                                        @Enable,
                                        @Creator,
                                        @CreateDate,
                                        @CoID              
                                        )";
            return sql;
        }
        public static string AddSkuPropValSql()
        {
            string sql = @" INSERT INTO customkind_skuprops_value(                                        
                                        `name`,
                                        mapping,
                                        pid,
                                        vid,    
                                        Creator,
                                        CreateDate,
                                        CoID
                                    ) VALUES (
                                        @name,
                                        @mapping,
                                        @pid,
                                        @vid,
                                        @Creator,
                                        @CreateDate,
                                        @CoID              
                                        )";
            return sql;
        }
        #endregion

    }
}