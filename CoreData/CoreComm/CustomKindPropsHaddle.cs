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
    public static class CustomKindPropsHaddle
    {
        #region 商品类目属性新增
        public static DataResult InsertProps(Customkind_props IParam)
        {
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CommConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                //类目属性
                string sql = CustomKindHaddle.AddKindPorpSql();
                IParam.pid = long.Parse(CommHaddle.GetRecordID(IParam.CoID));
                conn.Execute(sql, IParam, Trans);
                var propid = conn.QueryFirst<long>("select LAST_INSERT_ID()", Trans);//新增商品类目

                //类目属性可选值
                int Order = 0;
                var NewValLst = new List<Customkind_props_value>();
                foreach (var name in IParam.ValLst)
                {
                    var val = new Customkind_props_value();
                    val.propid = propid;//类目属性id
                    val.name = name;
                    val.kindid = IParam.kindid;//类目id
                    val.pid = IParam.pid;
                    val.Creator = IParam.Creator;
                    val.CreateDate = IParam.CreateDate;
                    val.CoID = IParam.CoID;
                    Order++;
                    val.Order = Order;
                    NewValLst.Add(val);
                }
                if (NewValLst.Count > 0)
                {
                    conn.Execute(CustomKindHaddle.AddKindPropValueSql(), NewValLst, Trans);
                }

                Trans.Commit();
                CoreUser.LogComm.InsertUserLog("新增类目属性", "Customkind_props", IParam.name, IParam.Creator, IParam.CoID, DateTime.Now);
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

        #region 商品类目属性修改
        public static DataResult UpdateProps(Customkind_props IParam)
        {
            string contents = string.Empty;
            var result = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CommConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                var NewValLst = new List<Customkind_props_value>();
                // var DelValLst = new List<Customkind_props_value>();                
                string sql = "SELECT * FROM customkind_props WHERE CoID=@CoID AND id=@ID";
                var Old = conn.QueryFirst<Customkind_props>(sql, new { CoID = IParam.CoID, ID = IParam.id });
                if (Old.name != IParam.name)
                {
                    contents = contents + "名称:" + Old.name + "=>" + IParam.name + ";";
                }
                if (Old.Order != IParam.Order)
                {
                    contents = contents + "排序:" + Old.Order + "=>" + IParam.Order + ";";
                }
                if (Old.is_input_prop != IParam.is_input_prop)
                {
                    contents = contents + "可输入:" + Old.is_input_prop + "=>" + IParam.is_input_prop + ";";
                }
                //更新类目属性
                if (!string.IsNullOrEmpty(contents))
                {
                    string propsql = "update customkind_props Set name=@name,`Order`=@Order,is_input_prop=@is_input_prop,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE id=@ID";
                    var p = new DynamicParameters();
                    p.Add("@name", IParam.name);
                    // p.Add("@FullName", fullname);
                    p.Add("@Order", IParam.Order);
                    p.Add("@is_input_prop", IParam.is_input_prop);
                    p.Add("@Modifier", IParam.Creator);
                    p.Add("@ModifyDate", DateTime.Now);
                    p.Add("@ID", IParam.id);
                    int count = conn.Execute(propsql, p, Trans);
                }

                //属性可选值增删
                var NewNameLst = IParam.ValLst;
                string valsql = "SELECT * FROM Customkind_props_value WHERE propid = @propid AND CoID=@CoID";
                string ordersql = "SELECT Max(`Order`) FROM Customkind_props_value WHERE propid = @propid AND CoID=@CoID";
                var OldLst = conn.Query<Customkind_props_value>(valsql, new { propid = IParam.id, CoID = IParam.CoID }).AsList();
                if (OldLst.Count > 0)
                {
                    NewNameLst = NewNameLst.Where(a => !OldLst.Select(b => b.name).Contains(a)).AsList();//新增的属性值
                }
                var Order = conn.QueryFirst<int>(ordersql, new { propid = IParam.id, CoID = IParam.CoID });
                foreach (var name in NewNameLst)
                {
                    var val = new Customkind_props_value();
                    val.name = name;
                    val.kindid = IParam.kindid;//类目id
                    val.propid = IParam.id;//类目属性id
                    val.Creator = IParam.Creator;
                    val.CreateDate = IParam.CreateDate;
                    val.CoID = IParam.CoID;
                    Order++;
                    val.Order = Order;
                    NewValLst.Add(val);
                }
                if (NewValLst.Count > 0)
                {
                    conn.Execute(CustomKindHaddle.AddKindPropValueSql(), NewValLst, Trans);
                    contents = contents + "可选值新增:(" + string.Join(",", NewNameLst.ToArray()) + ")";
                }
                //更新属性可选值
                var DelIDLst = OldLst.Where(a => !a.IsDelete && !IParam.ValLst.Contains(a.name)).AsList().Select(b => b.id).AsList();
                if (DelIDLst.Count > 0)
                {
                    string DelSql = @"UPDATE customkind_props_value
                                    SET IsDelete = 1,
                                    Modifier =@Modifier,
                                    ModifyDate =@ModifyDate
                                    WHERE
                                        CoID =@CoID
                                    AND propid =@propid
                                    AND id in @DelIDLst ";
                    conn.Execute(DelSql, new { CoID = IParam.CoID, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate, propid = IParam.id, DelIDLst = DelIDLst }, Trans);
                    contents = contents + "可选值移除:(" + string.Join(",", NewNameLst.ToArray()) + ")";
                }
                var UptIDLst = OldLst.Where(a => a.IsDelete && IParam.ValLst.Contains(a.name)).AsList().Select(b => b.id).AsList();
                if (UptIDLst.Count > 0)
                {
                    string UptSql = @"UPDATE customkind_props_value
                                    SET IsDelete = 0,
                                    Modifier =@Modifier,
                                    ModifyDate =@ModifyDate
                                    WHERE
                                        CoID =@CoID
                                    AND propid =@propid
                                    AND id in @UptIDLst ";
                    conn.Execute(UptSql, new { CoID = IParam.CoID, Modifier = IParam.Creator, ModifyDate = IParam.CreateDate, propid = IParam.id, UptIDLst = UptIDLst }, Trans);
                    contents = contents + "可选值恢复:(" + string.Join(",", NewNameLst.ToArray()) + ")";
                }
                Trans.Commit();
                CoreUser.LogComm.InsertUserLog("修改类目属性", "Customkind_props", contents, IParam.Creator, IParam.CoID, DateTime.Now);


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

        #region 商品类目属性停用|启用
        public static DataResult UptPropsEnable(List<int> IDLst, string CoID, string UserName, bool Enable)
        {
            var res = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CommConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string contents = string.Empty;
                string uptsql = @"update customkind_props 
                                    set Enable = @Enable,
                                        Modifier =@Modifier,
                                        ModifyDate =@ModifyDate
                                  where ID in @ID";
                var args = new { ID = IDLst, Enable = Enable, Modifier = UserName, ModifyDate = DateTime.Now.ToString() };
                int count = conn.Execute(uptsql, args, Trans);
                if (count < 0)
                {
                    res.s = -3003;
                }
                else
                {
                    if (Enable)
                    {
                        contents = "商品类目属性启用：";
                        res.s = 3001;
                    }
                    else
                    {
                        contents = "商品类目属性停用：";
                        res.s = 3002;
                    }
                    Trans.Commit();
                    contents += string.Join(",", IDLst.ToArray());
                    CoreUser.LogComm.InsertUserLogTran(Trans, "修改类目属性状态", "Customkind_props", contents, UserName, int.Parse(CoID), DateTime.Now);
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
                conn.Dispose();
                conn.Close();
            }
            return res;
        }

        #endregion



        #region 商品类目属性可选值复制
        public static DataResult CopyProps(List<int> IDLst, List<int> KindIDLst, int Type, string CoID, string UserName)
        {
            var res = new DataResult(1, null);
            var conn = new MySqlConnection(DbBase.CommConnectString);
            conn.Open();
            var Trans = conn.BeginTransaction();
            try
            {
                string sql = @"SELECT * FROM customkind_props WHERE id in @IDLst AND CoID=@CoID AND IsDelete=0";
                string valsql = @"SELECT * FROM customkind_props_value WHERE propid in @IDLst AND CoID=@CoID AND IsDelete=0";
                var FrmProps = conn.Query<Customkind_props>(sql, new { CoID = CoID, IDLst = IDLst }).AsList();
                var FrmPropsValLst = conn.Query<Customkind_props_value>(valsql, new { CoID = CoID, IDLst = IDLst }).AsList();
                var AllPToLst = conn.Query<Customkind_props>("SELECT * FROM customkind_props WHERE kindid in @KindIDLst AND CoID=@CoID", new { KindIDLst = KindIDLst, CoID = CoID }).AsList();//目的类目的属性
                var AllPToValLst = conn.Query<Customkind_props_value>("SELECT * FROM customkind_props_value WHERE kindid in @KindIDLst AND CoID=@CoID", new { KindIDLst = KindIDLst, CoID = CoID }).AsList();////目的类目的属性可选值
                var NewPropLst = new List<Customkind_props>();//新增属性
                var NewValLst = new List<Customkind_props_value>();//新增属性可选值
                var UptPropLst = new List<Customkind_props>();//修改属性
                var UptValIDLst = new List<int>();//修改属性可选值
                var DelValIDLst = new List<int>();
                foreach (var KindID in KindIDLst)//目的类目ID
                {
                    var PToLst = AllPToLst.Where(a => a.kindid == KindID).AsList();
                    #region 复制属性——不存在相同pid——新增属性
                    var FrmpAddLst = FrmProps.Where(a => !PToLst.Select(b => b.pid).Contains(a.pid)).AsList();//新增属性的值                    
                    foreach (var p in FrmpAddLst)
                    {
                        var prop = new Customkind_props();
                        prop.kindid = KindID;//类目id
                        prop.name = p.name;
                        prop.multi = p.multi;
                        prop.must = p.must;
                        prop.is_allow_alias = p.is_allow_alias;
                        prop.is_enum_prop = p.is_enum_prop;
                        prop.is_input_prop = p.is_input_prop;
                        prop.is_key_prop = p.is_key_prop;
                        prop.is_sale_prop = p.is_sale_prop;
                        prop.pid = p.pid;//pid
                        prop.tb_cid = p.tb_cid;
                        prop.ParentID = p.id;//记录来源属性
                        prop.Order = p.Order;
                        prop.Enable = p.Enable;
                        prop.CoID = int.Parse(CoID);
                        prop.Creator = UserName;
                        prop.CreateDate = DateTime.Now.ToString();
                        // prop.
                        var ValLst = FrmPropsValLst
                                    .Where(a => a.CoID == p.CoID && a.propid == p.id)
                                    .Select(b => new Customkind_props_value
                                    {
                                        kindid = KindID,
                                        name = b.name,
                                        vid = b.vid,
                                        tb_cid = b.tb_cid,
                                        pid = b.pid,
                                        Order = b.Order,
                                        Enable = b.Enable,
                                        ParentID = p.id,    //记录来源属性                                                  
                                        CoID = b.CoID,
                                        Creator = UserName,
                                        CreateDate = DateTime.Now.ToString()
                                    }).AsList();
                        NewPropLst.Add(prop);
                        NewValLst.AddRange(ValLst);
                    }
                    #endregion
                    #region 复制属性——存在相同pid——更新属性
                    var FrmpUptLst = FrmProps.Where(a => PToLst.Select(b => b.pid).Contains(a.pid)).AsList();//修改属性的值   
                    if (FrmpUptLst.Count > 0)
                    {
                        //更新属性
                        var PropToLst = (from f in FrmpUptLst
                                         join t in PToLst
                                         on new { pid = f.pid } equals new { pid = t.pid }
                                         group new { f, t } by new
                                         {
                                             t.id,
                                             t.pid,
                                             t.CoID,
                                             f.Enable,
                                             f.name,
                                             f.multi,
                                             f.must,
                                             f.is_allow_alias,
                                             f.is_enum_prop,
                                             f.is_input_prop,
                                             f.is_key_prop,
                                             f.is_sale_prop,
                                             f.tb_cid,
                                             f.Order
                                         } into s
                                         select new Customkind_props
                                         {
                                             id = s.Key.id,
                                             CoID = s.Key.CoID,
                                             pid = s.Key.pid,
                                             name = s.Key.name,
                                             Enable = s.Key.Enable,
                                             multi = s.Key.multi,
                                             must = s.Key.must,
                                             is_allow_alias = s.Key.is_allow_alias,
                                             is_enum_prop = s.Key.is_enum_prop,
                                             is_input_prop = s.Key.is_input_prop,
                                             is_key_prop = s.Key.is_key_prop,
                                             is_sale_prop = s.Key.is_sale_prop,
                                             tb_cid = s.Key.tb_cid,
                                             Order = s.Key.Order,
                                             Modifier = UserName,
                                             ModifyDate = DateTime.Now.ToString()
                                         }).ToList();
                        //更新属性可选值
                        var FrmValLst = FrmPropsValLst.Where(a => PropToLst.Select(b => b.pid).Contains(a.pid)).AsList();
                        var ToValLst = AllPToValLst.Where(a => a.kindid == KindID && PropToLst.Select(b => b.pid).Contains(a.pid)).AsList();
                        var UptIDLst = ToValLst.Where(a => a.IsDelete && FrmValLst.Select(b => b.name).Contains(a.name)).AsList().Select(c => c.id).AsList();
                        var DelIDLst = ToValLst.Where(a => !a.IsDelete && !FrmValLst.Select(b => b.name).Contains(a.name)).AsList().Select(c => c.id).AsList();
                        UptValIDLst.AddRange(UptIDLst);
                        DelValIDLst.AddRange(DelIDLst);
                        var ValLst = FrmValLst.Where(a => !ToValLst.Select(b => b.name + b.pid).Contains(a.name + a.pid))
                                    .Select(c => new Customkind_props_value
                                    {
                                        kindid = KindID,
                                        name = c.name,
                                        vid = c.vid,
                                        tb_cid = c.tb_cid,
                                        pid = c.pid,
                                        Order = c.Order,
                                        Enable = c.Enable,
                                        ParentID = c.propid,    //记录来源属性                                                  
                                        CoID = c.CoID,
                                        Creator = UserName,
                                        CreateDate = DateTime.Now.ToString()
                                    }).AsList();
                        NewValLst.AddRange(ValLst);
                        UptPropLst.AddRange(PropToLst);
                    }
                    #endregion
                }
                //新增类目属性
                if (NewPropLst.Count > 0)
                {
                    conn.Execute(CustomKindHaddle.AddKindPorpSql(), NewPropLst, Trans);
                }
                //修改类目属性
                if (UptPropLst.Count > 0)
                {
                    string propsql = "UPDATE customkind_props SET name=@name,`Order`=@Order,is_input_prop=@is_input_prop,Enable=@Enable,Modifier=@Modifier,ModifyDate=@ModifyDate,IsDelete=0 WHERE id=@id";
                    conn.Execute(propsql, UptPropLst, Trans);
                }
                //删除类目属性
                if (Type == 2)
                {
                    var DelPropIDLst = AllPToLst.Where(a => !FrmProps.Select(b => b.pid).Contains(a.pid)).AsList().Select(c => c.id).AsList();
                    string Delsql = "UPDATE customkind_props SET IsDelete=1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE id in @IDLst";
                    conn.Execute(Delsql, new { IDLst = DelPropIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                }
                //新增属性可选值
                if (NewValLst.Count > 0)
                {
                    int i = conn.Execute(CustomKindHaddle.AddKindPropValueSql(), NewValLst, Trans);
                    string uptvalsql = @"UPDATE customkind_props_value,
                                            customkind_props
                                        SET customkind_props_value.propid = customkind_props.id
                                        WHERE
                                            customkind_props_value.kindid = customkind_props.kindid
                                        AND customkind_props_value.pid = customkind_props.pid
                                        AND customkind_props_value.kindid in @KindIDLst";
                    conn.Execute(uptvalsql, new { KindIDLst = KindIDLst }, Trans);
                }
                //修改属性可选值
                if (UptValIDLst.Count > 0)
                {
                    string UptValSql = "UPDATE customkind_props_value SET IsDelete=0,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE id in @IDLst";
                    conn.Execute(UptValSql, new { IDLst = UptValIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                }
                //删除属性可选值
                if (DelValIDLst.Count > 0)
                {
                    string DelValSql = "UPDATE customkind_props_value SET IsDelete=1,Modifier=@Modifier,ModifyDate=@ModifyDate WHERE id in @IDLst";
                    conn.Execute(DelValSql, new { IDLst = DelValIDLst, Modifier = UserName, ModifyDate = DateTime.Now.ToString() }, Trans);
                }
                Trans.Commit();
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
                conn.Dispose();
                conn.Close();
            }
            return res;
        }

        #endregion

        #region 商品資料新增 - 根據類目獲取商品類目屬性
        public static DataResult GetItemPropsByKindID(string ID, string CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {

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

    }
}