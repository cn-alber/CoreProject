using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyUser;
using Dapper;
namespace CoreData.CoreUser
{
    public static class CompanyHaddle
    {
        ///<summary>
        ///获取全部的公司资料
        ///</summary>
        public static DataResult GetCompanyAll(int CoID,string nameFilter, string enable)
        {
            var s = 0;
            string wheresql = "";
            bool flag = false;
            if(CoID != 1)
            {
                wheresql = wheresql + "where id = " + CoID;
                flag = true;
            }
            bool enableF = false;
            if(enable != "all")
            {
                if(enable == "true")   
                {
                    enableF = true;
                }
                if(flag == false)
                {
                    wheresql = "where enable =" + enableF;
                }
                else
                {
                    wheresql = wheresql + " and enable =" + enableF;
                }
                flag = true;
            }
            if(nameFilter != "" && nameFilter != null)
            {
               if(flag == false)
                {
                    wheresql = wheresql + "where name like '%"+ nameFilter +"%' or address like '%" + nameFilter + "%' or remark like '%" + nameFilter + "%'";
                }
                else
                {
                    wheresql = wheresql + " and (name like '%"+ nameFilter +"%' or address like '%" + nameFilter + "%' or remark like '%" + nameFilter + "%')";
                }
                flag = true;
            }
            wheresql = "select * from company " + wheresql + " limit 0,10";
            var u = DbBase.UserDB.Query<Company>(wheresql).AsList();
            /*bool enableF = false;
            if(enable == "true")   
            {
                enableF = true;
            }
            var u = DbBase.UserDB.Query<Company>("select * from company limit 0,10").AsList();
            if(CoID == 1)
            {
                if(enable == "all")
                {
                    if(nameFilter == null || nameFilter == "")
                    {
                        u = DbBase.UserDB.Query<Company>("select * from company limit 0,10").AsList();
                    }
                    else
                    {
                        u = DbBase.UserDB.Query<Company>("select * from company where name like '%"+ nameFilter +"%'or address like '%" + nameFilter + "%' or remark like '%" + nameFilter + "%' limit 0,10").AsList();
                    }
                }
                else
                {
                    if(nameFilter == null || nameFilter == "")
                    {
                        u = DbBase.UserDB.Query<Company>("select * from company where enable = @ena limit 0,10",new {ena = enableF}).AsList();
                    }
                    else
                    {
                        u = DbBase.UserDB.Query<Company>("select * from company where enable = @ena and (name like '%"+ nameFilter +"%'or address like '%" + nameFilter + "%' or remark like '%" + nameFilter + "%') limit 0,10", new { ena = enableF }).AsList();
                    }
                }
            }
            else
            {
                if(enable == "all")
                {
                    if(nameFilter == null || nameFilter == "")
                    {
                        u = DbBase.UserDB.Query<Company>("select * from company where id = @id limit 0,10", new { id = CoID }).AsList();
                    }
                    else
                    {
                        u = DbBase.UserDB.Query<Company>("select * from company where id = @id and (name like '%"+ nameFilter +"%' or address like '%" + nameFilter + "%' or remark like '%" + nameFilter + "%') limit 0,10", new { id = CoID }).AsList();
                    }
                }
                else
                {
                    if(nameFilter == null || nameFilter == "")
                    {
                        u = DbBase.UserDB.Query<Company>("select * from company where id = @id and enable = @ena limit 0,10",new {id = CoID,ena = enableF}).AsList();
                    }
                    else
                    {
                        u = DbBase.UserDB.Query<Company>("select * from company where id = @id and enable = @ena and (name like '%"+ nameFilter +"%' or address like '%" + nameFilter + "%' or remark like '%" + nameFilter + "%') limit 0,10", new {id = CoID, ena = enableF}).AsList();
                    }
                }
            }
            */
            return new DataResult(s,u);
        }
    }
}