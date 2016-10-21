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
            var res = new CompanyData();
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>("select count(1) from company " + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = "select * from company " + wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<Company>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Com = u;
                    result.d = res;              
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
            var parent = CacheBase.Get<Company>("company" + ID.ToString());  
            if (parent == null)
            {
                using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                    try{
                        string wheresql = "select * from company where id ='" + ID.ToString() + "'" ;
                        var u = conn.Query<Company>(wheresql).AsList();
                        if (u.Count > 0)
                        {
                            CacheBase.Set<Company>("company" + ID.ToString(), u[0]);
                            result.d = u[0];
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
                    string wheresql = "select count(1) from company where name ='" + name + "'" ;
                    int u = conn.QueryFirst<int>(wheresql);            
                    if (u > 0)
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
        public static DataResult UpdateComEnable(List<int> id,int CoID,string UserName,bool Enable)
        {
            var result = new DataResult(1,null);   
            string contents = string.Empty;
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string uptsql = @"update company set enable = @Enable where id in @ID";
                    var args = new {ID = id,Enable = Enable};          
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                        return  result;
                    }
                    if(Enable == true)
                    {
                        contents = "公司状态启用";
                    }
                    else
                    {
                        contents = "公司状态停用";
                    }
                    LogComm.InsertUserLog("修改公司资料", "company", contents, UserName, CoID, DateTime.Now);
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
        ///公司基本资料新增
        ///</summary>   
        public static DataResult InsertCompany(Company com,string UserName,int CoID,UserEdit user)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string sqlCommandText = @"INSERT INTO company(name,address,email,contacts,telphone,mobile,remark,creator,modifier) VALUES(
                            @Name,@Address,@Email,@Contacts,@Telphone,@Mobile,@Remark,@Creator,@Modifier)";
                    com.creator = UserName;
                    com.createdate = DateTime.Now;
                    com.modifier = UserName;
                    com.modifydate = DateTime.Now;
                    int count =conn.Execute(sqlCommandText,com);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return  result;
                    }
                    int rtn = conn.QueryFirst<int>("select LAST_INSERT_ID()");
                    sqlCommandText = @"INSERT INTO user(account,name,password,email,gender,mobile,qq,companyid,creator) VALUES(
                        @Account,@Name,@Password,@Email,@Gender,@Mobile,@Qq,@Companyid,@Creator)";
                    user.CompanyID = rtn;
                    user.Creator = UserName;
                    count =conn.Execute(sqlCommandText,user);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    result.d = rtn;
                    LogComm.InsertUserLog("新增公司资料", "company", "新增公司" + com.name ,UserName, CoID, DateTime.Now);
                    com.id = rtn;
                    CacheBase.Set<Company>("company" + rtn.ToString(), com);   
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
        public static DataResult UpdateCompany(Company com,string UserName,int CoID)
        {
            var result = new DataResult(1,null);  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string wheresql = "select * from company where id =" + com.id;
                    var u = conn.Query<Company>(wheresql).AsList();
                    Company comupdate = u[0] as Company;
                    if(com.name != null)
                    {
                        if(com.name != u[0].name)
                        {
                            contents = contents + "公司名称" + ":" +u[0].name + "=>" + com.name + ";";
                            comupdate.name = com.name;
                        }
                    }
                    if(com.address != null)
                    {
                        if(com.address != u[0].address)
                        {
                            contents = contents + "公司地址" + ":" +u[0].address + "=>" + com.address + ";";
                            comupdate.address = com.address;
                        }
                    }
                    if(com.email != null)
                    {
                        if(com.email != u[0].email)
                        {
                            contents = contents + "公司邮箱" + ":" +u[0].email + "=>" + com.email + ";";
                            comupdate.email = com.email;
                        }
                    }
                    if(com.contacts != null)
                    {
                        if(com.contacts != u[0].contacts)
                        {
                            contents = contents + "公司联络人" + ":" +u[0].contacts + "=>" + com.contacts + ";";
                            comupdate.contacts = com.contacts;
                        }
                    }
                    if(com.telphone != null)
                    {
                        if(com.telphone != u[0].telphone)
                        {
                            contents = contents + "固定电话" + ":" +u[0].telphone + "=>" + com.telphone + ";";
                            comupdate.telphone = com.telphone;
                        }
                    }
                    if(com.mobile != null)
                    {
                        if(com.mobile != u[0].mobile)
                        {
                            contents = contents + "移动电话" + ":" +u[0].mobile + "=>" + com.mobile + ";";
                            comupdate.mobile = com.mobile;
                        }
                    }
                    if(com.remark != null)
                    {
                        if(com.remark != u[0].remark)
                        {
                            contents = contents + "备注" + ":" +u[0].remark + "=>" + com.remark + ";";
                            comupdate.remark = com.remark;
                        }
                    }
                    comupdate.modifier = UserName;
                    comupdate.modifydate = DateTime.Now;
                    string uptsql = @"update company set name = @Name,address = @Address,email=@Email,contacts=@Contacts,telphone=@Telphone,
                                        mobile=@Mobile,remark=@Remark,modifier = @Modifier,modifydate = @Modifydate where id = @ID";
                    int count = conn.Execute(uptsql,comupdate);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        LogComm.InsertUserLog("修改公司资料", "company", contents, UserName, CoID, DateTime.Now);               
                        CacheBase.Set<Company>("company" + com.id.ToString(), comupdate);
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