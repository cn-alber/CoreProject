using CoreModels;
using CoreModels.XyUser;
using Dapper;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CoreData.CoreUser
{
    public static class CompanyHaddle
    {
        ///<summary>
        ///查询公司资料List
        ///</summary>
        public static DataResult GetCompanyList(CompanyParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select id,name,enable,address,remark,creator,createdate from company where 1 = 1";
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
            var res = new CompanyData();
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{    
                    var u = conn.Query<CompanyMulti>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<CompanyMulti>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Com = u;
                    if (count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        result.d = res;
                    }               
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
        ///<summary>
        ///查询单笔公司资料
        ///</summary>
        public static DataResult GetCompanyEdit(int ID)
        {
            var result = new DataResult(1,null);        
            var parent = CacheBase.Get<CompanySingle>("company" + ID.ToString());  
            if (parent == null)
            {
                using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                    try{
                        string wheresql = "select id,name,enable,address,email,contacts,telphone,mobile,remark from company where id ='" + ID.ToString() + "'" ;
                        var u = conn.Query<CompanySingle>(wheresql).AsList();
                        if (u.Count == 0)
                        {
                            result.s = -3001;
                            result.d = null;
                        }
                        else
                        {
                            CacheBase.Set<CompanySingle>("company" + ID.ToString(), u[0]);
                            result.d = u;
                        }
                    }catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                }                                           
            }
            else
            {
                result.d = parent;
            }                            
            return result;
        }
        ///<summary>
        ///检查公司资料是否已经存在
        ///</summary>
        public static DataResult IsComExist(string name)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string wheresql = "select id,name,enable,address,email,contacts,telphone,mobile,remark from company where name ='" + name + "'" ;
                    var u = conn.Query<CompanySingle>(wheresql).AsList();            
                    if (u.Count > 0)
                    {
                        result.d = true;                 
                    }
                    else
                    {
                        result.d = false;   
                    }              
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }

            } 
            return  result;
        }
        ///<summary>
        ///公司启用停用设置
        ///</summary>
        public static DataResult UpdateComEnable(Dictionary<int,string> IDsDic,string Company,string UserName,bool Enable)
        {
            var result = new DataResult(1,null);   
            string contents = string.Empty;
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string uptsql = @"update company set enable = @Enable where id in @ID";
                    var args = new {ID = IDsDic.Keys.AsList(),Enable = Enable};          
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
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
                    result.d = contents;           
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return  result;
        }
        ///<summary>
        ///公司基本资料保存
        ///</summary>
        public static DataResult SaveCompany(string modifyFlag,CompanySingle com,string UserName,string Company)
        {
            var result = new DataResult(1,null);        
            if (modifyFlag == "new")
            {
                var iresult = InsertCompany(com,UserName,Company);
                result.s = iresult.s;
                result.d = iresult.d;
            }
            else
            {
                var mresult = UpdateCompany(com,UserName,Company);
                result.s = mresult.s;
                result.d = mresult.d;
            }
            return result;
        }      
        public static DataResult InsertCompany(CompanySingle com,string UserName,string Company)
        {
            var result = new DataResult(1,"资料新增成功!");   
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string sqlCommandText = @"INSERT INTO company(name,enable,address,email,contacts,telphone,mobile,remark,creator) VALUES(
                            @Name,
                            @Enable,
                            @Address,
                            @Email,
                            @Contacts,
                            @Telphone,
                            @Mobile,
                            @Remark,
                            @UName
                        )";
                    var args = new {Name = com.name,Enable=com.enable,Address = com.address,Email = com.email,Contacts = com.contacts,
                                    Telphone = com.telphone,Mobile = com.mobile,Remark = com.remark,UName = UserName};
                    int count =conn.Execute(sqlCommandText,args);
                    if(count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        LogComm.InsertUserLog("新增公司资料", "company", "新增公司" + com.name ,UserName, Company, DateTime.Now);
                        string wheresql = "select id,name,enable,address,email,contacts,telphone,mobile,remark from company where name ='" + com.name + "'" ;
                        var u = conn.Query<CompanySingle>(wheresql).AsList();
                        if (u.Count > 0)
                        {
                            CacheBase.Set<CompanySingle>("company" + u[0].id.ToString(), u[0]);
                        }
                    }        
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return  result;
        }
        public static DataResult UpdateCompany(CompanySingle com,string UserName,string Company)
        {
            var result = new DataResult(1,"资料更新成功!");  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string wheresql = "select id,name,enable,address,email,contacts,telphone,mobile,remark from company where id =" + com.id;
                    var u = conn.Query<CompanySingle>(wheresql).AsList();
                    if(com.name != u[0].name)
                    {
                        contents = contents + "公司名称" + ":" +u[0].name + "=>" + com.name + ";";
                    }
                    if(com.enable != u[0].enable)
                    {
                        contents = contents + "启用状态" + ":" +u[0].enable + "=>" + com.enable + ";";
                    }
                    if(com.address != u[0].address)
                    {
                        contents = contents + "公司地址" + ":" +u[0].address + "=>" + com.address + ";";
                    }
                    if(com.email != u[0].email)
                    {
                        contents = contents + "公司邮箱" + ":" +u[0].email + "=>" + com.email + ";";
                    }
                    if(com.contacts != u[0].contacts)
                    {
                        contents = contents + "公司联络人" + ":" +u[0].contacts + "=>" + com.contacts + ";";
                    }
                    if(com.telphone != u[0].telphone)
                    {
                        contents = contents + "固定电话" + ":" +u[0].telphone + "=>" + com.telphone + ";";
                    }
                    if(com.mobile != u[0].mobile)
                    {
                        contents = contents + "移动电话" + ":" +u[0].mobile + "=>" + com.mobile + ";";
                    }
                    if(com.remark != u[0].remark)
                    {
                        contents = contents + "备注" + ":" +u[0].remark + "=>" + com.remark + ";";
                    }
                    string uptsql = @"update company set name = @Name,enable = @Enable,address = @Address,email=@Email,contacts=@Contacts,telphone=@Telphone,mobile=@Mobile,remark=@Remark where id = @ID";
                    var args = new {Name = com.name,Enable=com.enable,Address = com.address,Email = com.email,Contacts = com.contacts,
                                    Telphone = com.telphone,Mobile = com.mobile,Remark = com.remark,ID = com.id};
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        LogComm.InsertUserLog("修改公司资料", "company", contents, UserName, Company, DateTime.Now);               
                        CacheBase.Set<CompanySingle>("company" + com.id.ToString(), com);
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return  result;
        }
    }
}