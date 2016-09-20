using System.Collections.Generic;
//using System.Linq;
using CoreModels;
using CoreModels.XyUser;
using Dapper;
using System;
namespace CoreData.CoreUser
{
    public static class CompanyHaddle
    {
        ///<summary>
        ///查询公司资料
        ///</summary>
        public static DataResult GetCompanyAll(int CoID,string nameFilter, string enable,int pageIndex,int numPerPage)
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
                    wheresql = wheresql + "where name like '%"+ nameFilter +"%'";
                }
                else
                {
                    wheresql = wheresql + " and name like '%"+ nameFilter +"%'";
                }
                flag = true;
            }
            wheresql = "select name,enable,address,typelist,remark,creator,createdate from company " + wheresql ;//+ " limit 0,10";
            var u = DbBase.UserDB.Query<Company>(wheresql).AsList();
            int count = u.Count;
            decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(numPerPage.ToString()));

            int dataindex = (pageIndex - 1)*numPerPage;
            wheresql = wheresql + "limit " + dataindex.ToString() + " ," + numPerPage.ToString();
            u = DbBase.UserDB.Query<Company>(wheresql).AsList();
            var cp = new CompanyParm();
            cp.datacnt = count;
            cp.pagecnt = pagecnt;
            cp.com = u;
            return new DataResult(s,cp);
        }
    }

    public class CompanyParm
    {
        public int datacnt {get;set;}
        public decimal pagecnt{get;set;}
        public List<Company> com {get;set;}
    }
}