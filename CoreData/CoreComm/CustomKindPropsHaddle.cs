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
                string sql = CustomKindHaddle.AddKindPorpSql();
                conn.Execute(sql, IParam, Trans);
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

    }
}