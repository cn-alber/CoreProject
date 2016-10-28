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
                    int count = conn.Execute(propsql, p,Trans);
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
                var DelIDLst = OldLst.Where(a => !a.IsDelete&&!IParam.ValLst.Contains(a.name)).AsList().Select(b => b.id).AsList();
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
                var UptIDLst = OldLst.Where(a => a.IsDelete&&IParam.ValLst.Contains(a.name)).AsList().Select(b => b.id).AsList();
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
                var args = new { ID = IDLst, Enable = Enable,Modifier=UserName,ModifyDate=DateTime.Now.ToString()};
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

        #endregion

    }
}