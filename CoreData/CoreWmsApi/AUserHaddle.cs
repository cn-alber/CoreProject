using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyUser;
using CoreModels.XyComm;
using CoreModels.WmsApi;
using Dapper;
using MySql.Data.MySqlClient;
using System;

namespace CoreData.CoreWmsApi
{
    public static class AUserHaddle
    {
        public static DataResult GetAUser(AUserParam IParam)
        {
            var res = new DataResult(1, null);
            // var Lst = new List<AUser>();
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    var p = new DynamicParameters();
                    string sql = @"SELECT
                            `user`.Account,
                            `user`.`Name`,
                            `user`.`PassWord`,
                            `user`.`Enable`,
                            `user`.CompanyID,
                            `user`.RoleID
                            FROM
                            `user`
                            WHERE Account = @Account AND IsDelete=0";
                    p.Add("@Account", IParam.Account);
                    var Lst = conn.Query<AUser>(sql, p).AsList();
                    if (Lst.Count == 0)
                    {
                        res.s = -2001;//"账号不存在"
                    }
                    else if (!Lst[0].Enable)
                    {
                        res.s = -2005;//账户被停用
                    }
                    else if (!Lst[0].PassWord.Equals(IParam.Password))
                    {
                        res.s = -2002;//密码错误
                    }
                    else
                    {
                        int roleid = Lst[0].RoleID;
                        var WhID = conn.QueryFirst<int>("SELECT role.WhID FROM role WHERE role.ID=@RoleID AND IsWms=1", new { RoleID = roleid });
                        if (WhID <= 0)
                        {
                            res.s = -2021;//账号未开启物流权限
                        }
                        else
                        {
                            string wsql = @"SELECT
                                                warehouse.ID,
                                                warehouse.WarehouseName,
                                                warehouse.Type
                                            FROM
                                                warehouse
                                            WHERE
                                                ID = @WhID
                                            UNION
                                                SELECT
                                                    warehouse.ID,
                                                    warehouse.WarehouseName,
                                                    warehouse.Type
                                                FROM
                                                    warehouse
                                                WHERE
                                                    warehouse.ParentID = (
                                                        SELECT
                                                            warehouse.ID
                                                        FROM
                                                            warehouse
                                                        WHERE
                                                            ID = @WhID
                                                    )";
                            var AWhLst = DbBase.CommDB.Query<AWarehouse>(wsql, new { WhID = WhID }).AsList();
                            Lst[0].AWhLst = AWhLst;
                            var IPLst = DbBase.CommDB.Query<Printer>("SELECT PrintType,IPAddress FROM printer WHERE CoID=@CoID AND IsDefault=1 AND Enabled=1", new { CoID = Lst[0].CompanyID }).AsList();
                            foreach (var i in IPLst)
                            {
                                switch (i.PrintType)
                                {
                                    case 1:
                                        Lst[0].IPAddress = i.IPAddress;
                                        break;
                                    case 2:
                                        Lst[0].ExpressIP = i.IPAddress;
                                        break;
                                    case 3:
                                        Lst[0].BarIp = i.IPAddress;
                                        break;
                                }
                            }
                            res.d=Lst;
                        }
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                    DbBase.CommDB.Close();
                }

                return res;
            }

        }
    }
}