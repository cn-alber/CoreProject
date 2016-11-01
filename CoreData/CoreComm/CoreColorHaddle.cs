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
    public static class CoreColorHaddle
    {
        #region 根据商品类目获取颜色列表
        public static DataResult GetKindColorLst(int KindID, string CoID)
        {
            var res = new DataResult(1, null);
            string sql = @"SELECT id,colorid,name FROM corecolor WHERE CoID=@CoID AND kindid = @KindID AND IsDelete=0";
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    var ColorLst = conn.Query<ColorData>(sql, new { KindID = KindID, CoID = CoID }).AsList();
                    res.d = ColorLst;
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