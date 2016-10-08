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
        ///获取菜单列表(避免与上面部分代码冲突)
        ///</summary>
        public static DataResult GetRefreshList(string roleid, string coid, string uname, string uid)
        {
            var s = 1;
            var cname = "refresh" + coid + roleid;

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

                    // var child = conn.Query<Refresh>("select id,name,CASE NewIcon  WHEN NewIconPre IS NOT NULL  THEN CONCAT(NewIcon,',','') ELSE CONCAT(NewIconPre,',','fa') END AS icons ,NewUrl as path,ParentID from menus where viewpowerid in (" +
                    //                              r.ViewList + ") order by ParentID,sortindex").AsList();
                    var child = conn.Query<Refresh>("select id,name,NewIcon, NewIconPre,NewUrl as path,ParentID from menus where NewUrl != '' and viewpowerid in (" +
                                r.ViewList + ") order by ParentID,sortindex").AsList();

                    foreach (var c in child)
                    {
                        if (!string.IsNullOrEmpty(c.NewIconPre))
                        {
                            c.icon = new string[] { c.NewIcon, c.NewIconPre };
                        }
                        else
                        {
                            c.icon = new string[] { c.NewIcon, "" };
                        }

                    }

                    // if (child.Count == 0)
                    // {
                    //     return null;
                    // }
                    var pidarray = (from c in child select c.parentID).Distinct().ToArray();
                    var pid = string.Join(",", pidarray);

                    //parent = conn.Query<Refresh>("select id,name,CASE NewIcon  WHEN NewIconPre IS NOT NULL  THEN CONCAT(NewIcon,',','') ELSE CONCAT(NewIconPre,',','fa') END AS icons ,NewUrl as path,ParentID from menus where id in (" + pid + ") order by sortindex").AsList();
                    parent = conn.Query<Refresh>("select id,name,NewIcon , NewIconPre ,NewUrl as path,ParentID from menus where id in (" + pid + ") order by sortindex").AsList();
                    foreach (var p in parent)
                    {
                        p.type = 2;
                        p.icon = new string[] { p.NewIcon, p.NewIconPre };
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
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    StringBuilder querycount = new StringBuilder();
                    StringBuilder querysql = new StringBuilder();
                    var p = new DynamicParameters();
                    string countsql = @"
                                    SELECT
                                        count(`user`.ID)
                                    FROM
                                        `user`       
                                    WHERE `user`.CompanyID = @CoID AND IsDelete = 0
                                     ";
                    string sql = @"SELECT
                                        `user`.ID,
                                        `user`.Account,
                                        `user`.`Name`,
                                        `user`.`Enable`,
                                        `user`.Email,
                                        `user`.Gender,
                                        role.`Name` AS RoleName,
                                        `user`.CreateDate
                                    FROM
                                        `user`                                   
                                    LEFT OUTER JOIN role ON `user`.RoleID = role.ID
                                    WHERE `user`.CompanyID = @CoID AND IsDelete = 0";
                    querycount.Append(countsql);
                    querysql.Append(sql);
                    p.Add("@CoID", IParam.CoID);

                    if (!string.IsNullOrEmpty(IParam.Enable) && IParam.Enable.ToUpper() != "ALL")//是否启用
                    {
                        querycount.Append(" AND `user`.Enable = @Enable");
                        querysql.Append(" AND `user`.Enable = @Enable");
                        p.Add("@Enable", IParam.Enable.ToUpper() == "TRUE" ? true : false);
                    }
                    if (!string.IsNullOrEmpty(IParam.Filter))
                    {
                        if(IParam.FilterType == 1)
                        {
                            querycount.Append(" AND Account like @Filter");
                            querysql.Append(" AND Account like @Filter");
                        }
                        else
                        {
                            querycount.Append(" AND `user`.Name like @Filter");   
                            querysql.Append(" AND `user`.Name like @Filter");   
                        }                        
                        p.Add("@Filter", "%" + IParam.Filter + "%");
                    }
                    if (!string.IsNullOrEmpty(IParam.SortField) && !string.IsNullOrEmpty(IParam.SortDirection))//排序
                    {
                        querycount.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                        querysql.Append(" ORDER BY " + IParam.SortField + " " + IParam.SortDirection);
                    }
                    var DataCount= CoreData.DbBase.UserDB.QueryFirst<int>(querycount.ToString(), p);
                    if (DataCount == 0)
                    {
                        res.s = -3001;
                    }
                    else
                    {
                        us.DataCount = DataCount;
                        decimal pagecnt = Math.Ceiling(decimal.Parse(us.DataCount.ToString()) / decimal.Parse(IParam.PageSize.ToString()));
                        us.PageCount = Convert.ToInt32(pagecnt);
                        int dataindex = (IParam.PageIndex - 1) * IParam.PageSize;
                        querysql.Append(" LIMIT @ls , @le");
                        p.Add("@ls", dataindex);
                        p.Add("@le", IParam.PageSize);
                        var UserLst = CoreData.DbBase.UserDB.Query<UserQuery>(querysql.ToString(), p).AsList();
                        us.UserLst = UserLst;
                        res.d = us;
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
                }
            }
            return res;
        }
        #endregion

        #region 单笔用户资料 - 编辑|查询
        public static DataResult GetUserEdit(string UserID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    string querysql = @"SELECT
                                            u.*,
                                            r. NAME AS RoleName
                                        FROM
                                            `user` u
                                        INNER JOIN role r ON u.RoleID = r.ID
                                        WHERE
                                            u.ID = @UserID AND IsDelete = 0";
                    var p = new { UserID = UserID };
                    var us = DbBase.UserDB.QueryFirst<UserEdit>(querysql, p);
                    if (us == null)
                    {
                        res.s = -3001;
                    }
                    us.PassWord = string.Empty;//密码不带回资料
                    res.d = us;
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion

        #region 从缓存读取数据
        public static DataResult GetUserCache(int CoID, string ID)
        {
            var res = new DataResult(1, null);
            string usname = "user" + CoID.ToString() + ID;
            var us = CacheBase.Get<UserEdit>(usname);//读取缓存
            if (us == null)
            {
                try
                {
                    string querysql = @"SELECT
                                            u.*, 
                                            r. NAME AS RoleName
                                        FROM
                                            `user` u                                        
                                        INNER JOIN role r ON u.RoleID = r.ID
                                        WHERE
                                            u.ID = @ID and u.CompanyID=@CoID AND IsDelete = 0";
                    var p = new { ID = ID, CoID = CoID };
                    us = DbBase.UserDB.QueryFirst<UserEdit>(querysql, p);
                    if (us == null)
                    {
                        res.s = -3001;
                    }
                    res.d = us;
                    CacheBase.Set(usname, us);//新增缓存
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

        #region 启用|停用用户
        public static DataResult UptUserEnable(List<int> IDLst, string CoID, string UserName, bool Enable)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                conn.Open();
                var TransUser = conn.BeginTransaction();
                try
                {
                    //删除缓存                    
                    foreach (var item in IDLst)
                    {
                        CacheBase.Remove("user" + CoID + item);
                    }
                    string contents = string.Empty;
                    string uptsql = @"update user set Enable = @Enable where ID in @ID";
                    var args = new { ID = IDLst, Enable = Enable };

                    int count = conn.Execute(uptsql, args, TransUser);
                    if (count < 0)
                    {
                        res.s = -3003;
                    }
                    else
                    {
                        if (Enable)
                        {
                            contents = "用户状态启用：";
                            res.s = 3001;
                        }
                        else
                        {
                            contents = "用户状态停用：";                            
                            res.s = 3002;
                        }
                        contents += string.Join(",", IDLst.ToArray());
                        CoreUser.LogComm.InsertUserLogTran(TransUser, "修改用户状态", "User", contents, UserName, CoID, DateTime.Now);
                        string querysql = @"SELECT
                                            u.*, b. NAME AS CompanyName,
                                            r. NAME AS RoleName
                                        FROM
                                            `user` u
                                        INNER JOIN company b ON u.CompanyID = b.ID
                                        INNER JOIN role r ON u.RoleID = r.ID
                                        WHERE
                                            u.ID in @ID AND IsDelete = 0";
                        var p = new { ID = IDLst };
                        var userLst = DbBase.UserDB.Query<UserEdit>(querysql, p, TransUser).ToList();
                        if (userLst.Count() == 0)
                        {
                            res.s = -3001;
                        }
                       // res.d = contents;
                        //添加缓存
                        foreach (var item in userLst)
                        {
                            CacheBase.Set("user" + CoID + item.ID, item);
                        }
                        if (res.s == 1)
                        {
                            TransUser.Commit();
                        }
                    }
                }
                catch (Exception e)
                {
                    TransUser.Rollback();
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                    conn.Close();
                }
            }
            return res;
        }
        #endregion

        #region 检查用户账号是否存在
        public static DataResult ExistUser(string Account, int ID, int CoID)
        {
            int count = 0;
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    string query = string.Empty;
                    // object param = null;
                    StringBuilder querystr = new StringBuilder();
                    querystr.Append("select * from user where CompanyID = @CoID and Account = @Account");
                    var p = new DynamicParameters();
                    p.Add("@CoID", CoID);
                    p.Add("@Account", Account);
                    if (ID > 0)
                    {
                        querystr.Append(" and ID !=@ID");
                        p.Add("@ID", ID);
                    }
                    count = conn.Query(querystr.ToString(), p).Count();
                    if (count > 0)
                    {
                        res.s = -1;
                        res.d = "账号已存在";
                    }
                    return res;
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return res;
        }
        #endregion

        #region 新增用户
        public static DataResult SaveInsertUser(UserEdit user, int CoID, string UserName)
        {
            var result = ExistUser(user.Account, 0, CoID);
            if (result.s == 1)
            {
                result = AddUser(user, CoID, UserName);
            }
            return result;
        }

        public static DataResult AddUser(UserEdit user, int CoID, string UserName)
        {
            var usname = "user" + CoID + user.ID;
            var result = new DataResult(1, null);
            string sqlCommandText = @"INSERT INTO `user`
                        (Account,
                        `Name`,
                        `PassWord`,
                        `Enable`,
                        Email,
                        Gender,
                        Mobile,
                        QQ,
                        CompanyID,
                        RoleID,
                        Creator,
                        CreateDate) VALUES(
                        @Account,
                        @Name,
                        @PassWord,
                        @Enable,
                        @Email,
                        @Gender,
                        @Mobile,
                        @QQ,
                        @CompanyID,
                        @RoleID,
                        @Creator,
                        @CreateDate
                        )";
            var us = new UserEdit();
            us.Account = user.Account;
           // us.SecretID = user.SecretID;
            us.Name = user.Name;
            us.PassWord = user.PassWord;
            us.Enable = user.Enable;
            us.Email = user.Email;
            us.Gender = user.Gender;
            us.Mobile = user.Mobile;
            us.QQ = user.QQ;
            us.CompanyID = CoID;
            us.RoleID = user.RoleID;
            us.Creator = UserName;
            us.CreateDate = DateTime.Now.ToString();
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            UserDBconn.Open();
            var TransUser = UserDBconn.BeginTransaction();
            try
            {
                int count = UserDBconn.Execute(sqlCommandText, us, TransUser);
                if (count < 0)
                {
                    result.s = -3002;
                }
                else
                {
                    CoreUser.LogComm.InsertUserLogTran(TransUser, "新增用户资料", "User", user.Name, UserName, CoID.ToString(), DateTime.Now);
                    CacheBase.Set<UserEdit>(usname, user);//缓存
                }
                if (result.s == 1)
                {
                    TransUser.Commit();
                }
            }
            catch (Exception e)
            {
                TransUser.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransUser.Dispose();
                UserDBconn.Close();
            }
            return result;
        }
        #endregion


        #region 修改用户
        public static DataResult SaveUpdateUser(UserEdit user, int CoID, string UserName)
        {
            var sname = "user" + CoID + user.ID;
            string contents = string.Empty;
            var result = ExistUser(user.Account, user.ID, CoID);
            if (result.s == 1)
            {
                var res = GetUserEdit(user.ID.ToString());
                var userOld = res.d as UserEdit;
                //删除原有缓存
                CacheBase.Remove(sname);
                if (userOld.Account != user.Account)
                {
                    contents = contents + "账号:" + userOld.Account + "=>" + user.Account + ";";
                }
                if (userOld.Name != user.Name)
                {
                    contents = contents + "名称:" + userOld.Name + "=>" + user.Name + ";";
                }
                // if (!string.IsNullOrEmpty(user.PassWord) && userOld.PassWord != user.PassWord)
                // {
                //     contents = contents + "密码:" + userOld.PassWord + "=>" + user.PassWord + ";";
                // }
                if (userOld.Enable != user.Enable)
                {
                    contents = contents + "用户状态:" + userOld.Enable + "=>" + user.Enable + ";";
                }
                if (userOld.Email != user.Email)
                {
                    contents = contents + "邮箱:" + userOld.Email + "=>" + user.Email + ";";
                }
                if (userOld.Gender != user.Gender)
                {
                    contents = contents + "性别:" + userOld.Gender + "=>" + user.Gender + ";";
                }
                if (userOld.Mobile != user.Mobile)
                {
                    contents = contents + "联系电话:" + userOld.Mobile + "=>" + user.Mobile + ";";
                }
                if (userOld.QQ != user.QQ)
                {
                    contents = contents + "QQ:" + userOld.QQ + "=>" + user.QQ + ";";
                }
                if (userOld.RoleID != user.RoleID)
                {
                    contents = contents + "角色:" + userOld.RoleID + "." + userOld.RoleName + "=>" + user.RoleID + "." + user.RoleName + ";";
                }

                var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
                UserDBconn.Open();
                var TransUser = UserDBconn.BeginTransaction();
                try
                {
                    
                    //  `PassWord` = @PassWord,
                    //     `Enable` = @Enable,
                    string str = @"UPDATE user
                    SET Account = @Account,
                        `Name` = @Name,                       
                        Email = @Email,
                        Gender = @Gender,
                        Mobile = @Mobile,
                        QQ = @QQ,
                        CompanyID = @CompanyID,
                        RoleID = @RoleID
                    WHERE ID = @ID               
                    ";
                    int count = UserDBconn.Execute(str, user, TransUser);
                    if (count <= 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLogTran(TransUser, "修改用户资料", "User", contents, UserName, CoID.ToString(), DateTime.Now);
                        CacheBase.Set<UserEdit>(sname, user);//缓存
                        TransUser.Commit();
                    }
                }
                catch (Exception e)
                {
                    TransUser.Rollback();
                    result.s = -1;
                    result.d = e.Message;
                }
                finally
                {
                    TransUser.Dispose();
                    UserDBconn.Close();
                }
                CacheBase.Set(sname, user);
            }
            return result;
        }
        #endregion

        #region 删除用户
        public static DataResult DeleteUserAccount(List<int> IDLst, int IsDelete, int CoID, string UserName)
        {
            var result = new DataResult(1, null);
            foreach (var u in IDLst)
            {
                var sname = "user" + CoID + u;
                CacheBase.Remove(sname);
            }
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            UserDBconn.Open();
            var TransUser = UserDBconn.BeginTransaction();
            try
            {
                var sql = string.Empty;
                var p = new DynamicParameters();
                // if (IsDelete == 1)
                // {
                //     sql = "delete from user where CompanyID = @CoID and ID in @IDLst";
                // }
                // else
                // {//软删除
                    sql = "update user set IsDelete = @IsDelete,Deleter=@Deleter,DeleteDate=@DeleteDate where CompanyID = @CoID and ID in @IDLst";
                    p.Add("@IsDelete", 1);
                    p.Add("@Deleter", UserName);
                    p.Add("@DeleteDate",DateTime.Now);
                // }

                p.Add("@CoID", CoID);
                p.Add("@IDLst", IDLst);

                int count = DbBase.UserDB.Execute(sql, p, TransUser);
                if (count > 0)
                {
                    string contents = "删除用户=>" + string.Join(",", IDLst);
                    LogComm.InsertUserLogTran(TransUser, "删除用户资料", "User", contents, UserName, CoID.ToString(), DateTime.Now);
                }
                TransUser.Commit();
            }
            catch (Exception e)
            {
                TransUser.Rollback();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransUser.Dispose();
                UserDBconn.Dispose();
            }
            return result;
        }
        #endregion

        #region 修改用户密码
          public static DataResult ModifyPwd(string uid, string newp)
        {

            var reslut = new DataResult(2001, null);
            using (var conn = new MySqlConnection(DbBase.UserConnectString))
            {
                try
                {
                    string sql = "UPDATE `user` SET `user`.`PassWord`= '" + newp + "' WHERE `user`.ID = " + uid;
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
        #endregion
    }
}