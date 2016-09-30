using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyUser;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace CoreData.CoreUser
{
    public static class UserHaddle
    {

        ///<summary>
        ///获取登陆信息
        ///</summary>
        public static DataResult GetUserInfo(string account, string password)
        {
            var s = 1;

            var u = DbBase.UserDB.Query<User>("select id, password, name, companyid, roleid, enable, islocked from user where account = @acc", new { acc = account }).AsList();
            if (u.Count == 0)
            {
                s = -2001;
            }
            else if (!u[0].Enable)
            {
                s = -2005;
            }
            else if (!u[0].PassWord.Equals(password))
            {
                s = -2002;
            }

            return new DataResult(s, s == 1 ? u[0] : null);
        }

        ///<summary>
        ///获取授权列表
        ///</summary>
        public static DataResult GetRole(string roleid, string coid)
        {
            var s = 1;
            var cname = "role" + coid + roleid;
            var cu = new Role();

            cu = CacheBase.Get<Role>(cname);
            if (cu == null)
            {
                var u = DbBase.UserDB.Query<Role>("select * from role where id = @rid and companyid = @coid", new { rid = roleid, coid = coid }).AsList();
                if (u.Count == 0)
                {
                    s = -2003;
                }
                else
                {
                    cu = u[0];
                    CacheBase.Set<Role>(cname, cu);
                }
            }

            return new DataResult(s, cu);
        }

        public static User GetUser(string uid)
        {
            var u = DbBase.UserDB.Query<User>("select id, password, name, companyid, roleid, enable, islocked from user where ID = @uid", new { uid = uid }).First();
            return u;
        }



        ///<summary>
        ///获取菜单列表
        ///</summary>
        // public static DataResult GetMenuList(string roleid, string coid)
        // {
        //     var s = 1;
        //     var cname = "menus" + coid + roleid;

        //     //获取菜单缓存
        //     // var parent = CacheBase.Get<List<Menu>>(cname);
        //     // if (parent == null)
        //     // {
        //         var parent = GetMenu(roleid, coid);
        //         if (parent == null)
        //         {
        //             s = -2004;
        //         }
        //         else
        //         {
        //             //无缓存，添加缓存
        //             //CacheBase.Set<List<Menu>>(cname, parent);
        //         }
        //     //}
        //     return new DataResult(s, s == 1 ? parent : null);
        // }

        // ///<summary>
        // ///获取菜单列表数据
        // ///</summary>
        // public static List<Menu> GetMenu(string roleid, string coid)
        // {
        //     var parent = new List<Menu>();
        //     using (var conn = new MySqlConnection(DbBase.UserConnectString))
        //     {
        //         try
        //         {
        //             //获取权限列表
        //             var role = GetRole(roleid, coid);
        //             if (role.s > 1) return null;
        //             var r = role.d as Role;
        //             //"select name,NewIcon,NewIconPre,NavigateUrl,ParentID from menus where viewpowerid in (" + r.ViewList + ") order by ParentID,sortindex"
        //             string sql = "select menus.id, menus.`Name` as `name`,NewUrl as router,SortIndex as `order`, menus.Remark, ParentID ,power.Title as access from menus "+
        //                         "LEFT JOIN power on power.ID = menus.ViewPowerID where viewpowerid in (" + r.ViewList + ") order by ParentID,sortindex"; 

        //             var child = conn.Query<Menu>(sql).AsList();
        //             if (child.Count == 0)
        //             {
        //                 return null;
        //             }
        //             var pidarray = (from c in child select c.parentid).Distinct().ToArray();
        //             var pid = string.Join(",", pidarray);
        //             //"select id,name,NewIcon,NewIconPre,NavigateUrl,ParentID from menus where id in (" + pid + ") order by sortindex"
        //             sql = "select menus.id, menus.`Name` as `name`,NewUrl as router,SortIndex as `order`, menus.Remark, ParentID ,power.Title as access from menus "+
        //                         "LEFT JOIN power on power.ID = menus.ViewPowerID where menus.id in (" + pid + ") order by sortindex"; 
                    
        //             parent = conn.Query<Menu>(sql).AsList();

        //             foreach (var p in parent)
        //             {
        //                 p.children = (from c in child where c.parentid == p.id select c).ToList();
        //             }
        //         }
        //         catch
        //         {
        //             conn.Dispose();
        //             return null;
        //         }
        //     }
        //     return parent;
        // }


        ///<summary>
        ///获取菜单列表(避免与上面部分代码冲突)
        ///</summary>
        public static DataResult GetRefreshList(string roleid, string coid, string uname, string uid)
        {
            var s = 1;
            var cname = "Refresh" + coid + roleid;

            //获取菜单缓存
            CacheBase.Remove(cname);
            var parent = CacheBase.Get<List<Refresh>>(cname);
            var parentRefresh = new List<Refresh>();

            if (parent == null)
            {
                parent = GetRefresh(roleid, coid);
                if (parent == null)
                {
                    s = -2004;
                }
                else
                {

                    CacheBase.Set<List<Refresh>>(cname, parent);
                    //return new DataResult(s, reslut);                    
                }
            }
            //var parent = GetRefresh(roleid, coid);
            //CacheBase.Set<List<Refresh>>(cname, parent);

            var reslut = new
            {
                isLocked = false,
                permissionMenus = parent,
                user = new
                {
                    name = uname,
                    avatar = "/path/avatarx80.png",
                    uid = uid,
                    key = "",
                    sign_time = "",
                }
            };
            return new DataResult(s, s == 1 ? reslut : null);
        }

        ///<summary>
        ///获取菜单列表数据
        ///</summary>
        public static List<Refresh> GetRefresh(string roleid, string coid)
        {
            var parent = new List<Refresh>();
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    //获取权限列表
                    var role = GetRole(roleid, coid);
                    if (role.s > 1) return null;
                    var r = role.d as Role;

                    var child = conn.Query<Refresh>("select id,name,CASE NewIcon  WHEN NewIconPre IS NOT NULL  THEN CONCAT(NewIcon,',','') ELSE CONCAT(NewIconPre,',','fa') END AS icons ,NewUrl as path,ParentID from menus where viewpowerid in (" +
                                                 r.ViewList + ") order by ParentID,sortindex").AsList();
                    foreach (var c in child)
                    {
                        c.icon = c.icons.Split(',');
                    }

                    if (child.Count == 0)
                    {
                        return null;
                    }
                    var pidarray = (from c in child select c.parentID).Distinct().ToArray();
                    var pid = string.Join(",", pidarray);
                    parent = conn.Query<Refresh>("select id,name,CASE NewIcon  WHEN NewIconPre IS NOT NULL  THEN CONCAT(NewIcon,',','') ELSE CONCAT(NewIconPre,',','fa') END AS icons ,NewUrl as path,ParentID from menus where id in (" + pid + ") order by sortindex").AsList();

                    foreach (var p in parent)
                    {
                        p.type = 2;
                        p.icon = p.icons.Split(',');
                        p.data = (from c in child where c.parentID == p.id select c).ToList();
                    }
                }
                catch
                {
                    conn.Dispose();
                    return null;
                }
            }
            return parent;
        }

        public static DataResult lockuser(string uid)
        {
            int s = 1;
            try
            {
                int rnt = DbBase.UserDB.Execute("UPDATE `user` SET `user`.IsLocked = 1 WHERE `user`.ID = " + uid);
                if (rnt < 1) s = -1;

            }
            catch { s = -1; }

            return new DataResult(s, null);
        }

        public static DataResult unlockuser(string uid, string password)
        {
            int s = 1;
            try
            {
                int rnt = DbBase.UserDB.Execute("UPDATE `user` SET `user`.IsLocked = 0 WHERE `user`.ID = @uid AND `user`.`PassWord` = @p ", new { uid = int.Parse(uid), p = password });
                if (rnt == 0) s = -2002;
            }
            catch { s = -2002; }
            return new DataResult(s, null);
        }

        public static DataResult editPwd(string uid, string oldp, string newp)
        {

            var reslut = new DataResult(2001, null);
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {

                try
                {
                    string sql = "UPDATE `user` SET `user`.`PassWord`= '" + newp + "' WHERE `user`.ID = " + uid + " AND `user`.`PassWord` = '" + oldp + "';";
                    int rnt = DbBase.UserDB.Execute(sql);
                    if (rnt == 0) reslut.s = -2002;
                }
                catch (Exception ex)
                {
                    reslut.s = -1;
                    reslut.d = ex.Message;
                    conn.Dispose();
                }
            }



            return reslut;
        }



        #region 用户管理 - 资料查询
        public static DataResult GetUserLst(UserParam IParam)
        {
            var res = new DataResult(1, null);
            var us = new UserData();
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string sql = @"SELECT
                                        `user`.ID,
                                        `user`.Account,
                                        `user`.`Name`,
                                        `user`.`Enable`,
                                        `user`.Email,
                                        `user`.Gender,
                                        company.`Name` AS 'CompanyName',
                                        role.`Name` AS 'RoleName',
                                        `user`.CreateDate
                                    FROM
                                        `user`,
                                        company,
                                        role
                                    WHERE
                                        `user`.CompanyID = company.ID
                                    AND `user`.RoleID = role.ID
                                    AND `user`.CompanyID = @CoID";
                    querysql.Append(sql);
                    if (IParam.CoID != 1)
                    {
                        querysql.Append(" AND CoID = @CoID");
                        p.Add("@CoID", IParam.CoID);
                    }
                    if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
                    {
                        querysql.Append(" AND Enable = @Enable");
                        p.Add("@Enable", IParam.Enable == "true" ? true : false);
                    }
                    if (!string.IsNullOrEmpty(IParam.Filter))
                    {
                        querysql.Append(" AND (SkuID like @Filter or SkuName like @Filter or Norm like @Filter)");
                        p.Add("@Filter", "'%" + IParam.Filter + "'");
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var UserLst = CoreData.DbBase.UserDB.Query<UserQuery>(querysql.ToString(), p).AsList();
                    if (UserLst.Count == 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        us.DataCount = UserLst.Count;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(us.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        us.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        UserLst = CoreData.DbBase.UserDB.Query<UserQuery>(querysql.ToString(), p).AsList();
                        us.UserLst = UserLst;
                        res.d = us;
                    }
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion


    }
}