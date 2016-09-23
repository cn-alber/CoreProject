using CoreModels;
using CoreModels.XyUser;
using Dapper;
using System;
using System.Collections.Generic;

namespace CoreData.CoreUser
{
    public static class CompanyHaddle
    {
        ///<summary>
        ///查询公司资料List
        ///</summary>
        public static DataResult GetCompanyList(CompanyParm cp)
        {
            var s = 1;
            string wheresql = "where 1 = 1";
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and id = " + cp.CoID;
            }
            if(!string.IsNullOrEmpty(cp.Enable) && cp.Enable.ToUpper()!="ALL")//是否启用
            {
                wheresql = wheresql + " AND enable = "+ (cp.Enable.ToUpper()=="TRUE"?true:false);
            }
            if(!string.IsNullOrEmpty(cp.Filter))//过滤条件
            {
               wheresql = wheresql + " and name like '%"+ cp.Filter +"%'";
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            wheresql = "select id,name,enable,address,typelist,remark,creator,createdate from company " + wheresql ;//+ " limit 0,10";
            //计算应抓取资料的笔数
            var u = DbBase.UserDB.Query<CompanyMulti>(wheresql).AsList();
            int count = u.Count;
            decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

            int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
            wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
            u = DbBase.UserDB.Query<CompanyMulti>(wheresql).AsList();

            cp.Datacnt = count;
            cp.Pagecnt = pagecnt;
            cp.Com = u;
            if (count == 0)
            {
                s = 3001;
            }
            return new DataResult(s,cp);
        }
        ///<summary>
        ///查询单笔公司资料
        ///</summary>
        public static DataResult GetCompanyEdit(int ID)
        {
            var s = 1;            
            string wheresql = "select id,name,enable,address,email,typelist,contacts,telphone,mobile,remark from company where id ='" + ID.ToString() + "'" ;//+ " limit 0,10";
            var u = DbBase.UserDB.Query<CompanySingle>(wheresql).AsList();
            if (u.Count == 0)
            {
                s = 3001;
            }
            return new DataResult(s,u);
        }
        // ///<summary>
        // ///检查公司资料是否已经存在
        // ///</summary>
        // public static DataResult IsComExist(string name)
        // {
        //     var s = 1;            
        //     string wheresql = "select name,enable,address,email,typelist,contacts,telphone,mobile,remark from company where name ='" + name + "'" ;//+ " limit 0,10";
        //     var u = DbBase.UserDB.Query<Company>(wheresql).AsList();
        //     bool flag = false;
        //     if(u.Count > 0)
        //     {
        //         flag = true;
        //     }
        //     return new DataResult(s,flag);
        // }
        ///<summary>
        ///公司启用停用设置
        ///</summary>
        public static DataResult UpdateComEnable(Dictionary<int,string> IDsDic,string Company,string UserName,bool Enable)
        {
            var s = 1;
            string contents = string.Empty;            
            string uptsql = @"update company set enable = @Enable where id in @ID";
            var args = new {ID = IDsDic.Keys.AsList(),Enable = Enable};          

            int count = DbBase.UserDB.Execute(uptsql,args);
            if(count<=0)
            {
                s=3003;
            }
            else
            {
                if(Enable)
               {
                   contents = "公司状态启用：";
               }
               else
               {
                   contents = "公司状态停用：";
               }
               contents+= string.Join(",", IDsDic.Values.AsList().ToArray());
               LogComm.InsertUserLog("修改公司资料", "company", contents, UserName, Company, DateTime.Now);
            }
            
            return new DataResult(s,contents);
        }

        ///<summary>
        ///公司基本资料保存
        ///</summary>
        public static DataResult UpdateCompany(string modifyFlag,string name)
        {
            // var s = 0;     
            // if (modifyFlag == "new")
            // {
            //     var result = IsComExist(name);
            //     if (bool.Parse(result.d.ToString()) == true)
            //     {
            //         return new DataResult(s,"该公司名称的资料已经存在，不允许新增");
            //     }
            //     //DBTrans a = DbBase.UserDB.BeginTransaction(IsolationLevel)
            //     //DbBase.UserDB.QuerySingle IsolationLevel.ReadUncommitted
            //     //DbTransaction a = 
            // }
            // else
            // {

            // }
            // var s = 0;            
            // string wheresql = "update company set enable = " + enable + " where id in (" + id + ")" ;//+ " limit 0,10";
            // var u = DbBase.UserDB.Query<Company>(wheresql).AsList();
            
            // return new DataResult(s,"公司:[" + name + "]被" + (enable ? "启用" : "禁用"));
            return new DataResult(0,"");
        }

        
    }

    
}