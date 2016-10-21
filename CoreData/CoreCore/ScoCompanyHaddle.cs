using CoreModels;
using CoreModels.XyCore;
using MySql.Data.MySqlClient;
using Dapper;
using System;
using System.Collections.Generic;

namespace CoreData.CoreCore
{
    public static class ScoCompanyHaddle
    {
        ///<summary>
        ///查询公司资料List
        ///</summary>
        public static DataResult GetScoCompanyList(ScoCompanyParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select id,scocode,enable,sconame,scosimple,typelist,remark,creator,createdate from supplycompany where 1 = 1";
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(!string.IsNullOrEmpty(cp.Enable) && cp.Enable.ToUpper()!="ALL")//是否启用
            {
                wheresql = wheresql + " AND enable = "+ (cp.Enable.ToUpper()=="TRUE"?true:false);
            }
            if(!string.IsNullOrEmpty(cp.Filter))//过滤条件
            {
               wheresql = wheresql + " and (sconame like '%"+ cp.Filter +"%' or scosimple like '%" + cp.Filter + "%')";
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new ScoCompanyData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<ScoCompanyMulti>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<ScoCompanyMulti>(wheresql).AsList();

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
        ///公司启用停用设置
        ///</summary>
        public static DataResult UpdateScoComEnable(List<int> id,string Company,string UserName,bool Enable)
        {
            var result = new DataResult(1,null);   
            string contents = string.Empty;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string uptsql = @"update supplycompany set enable = @Enable where id in @ID";
                    var args = new {ID = id,Enable = Enable};          
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        if(Enable)
                        {
                            contents = "客户状态启用";
                        }
                        else
                        {
                            contents = "客户状态停用";
                        }
                        int CoID = 1;
                        // contents+= string.Join(",", IDsDic.Values.AsList().ToArray());
                        CoreUser.LogComm.InsertUserLog("修改客户资料", "supplycompany", contents, UserName, CoID, DateTime.Now);
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
        ///查询单笔公司资料
        ///</summary>
        public static DataResult GetScoCompanyEdit(int ID)
        {
            var result = new DataResult(1,null);   
            string scoName = "scocompany" + ID.ToString();     
            var parent = CacheBase.Get<ScoCompanySingle>(scoName);  
            if (parent == null)
            {
                using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                    try{
                        string wheresql = "select id,sconame,scosimple,enable,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark from supplycompany where id ='" + ID.ToString() + "'" ;
                        var u = conn.Query<ScoCompanySingle>(wheresql).AsList();
                        if (u.Count == 0)
                        {
                            result.s = -3001;
                            result.d = null;
                        }
                        else
                        {
                            CacheBase.Set<ScoCompanySingle>(scoName, u[0]);
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
        ///检查客户资料是否已经存在
        ///</summary>
        public static DataResult IsScoComExist(string name,int coid)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select id,sconame,scosimple,enable,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark from supplycompany where sconame ='" + name + "' and coid =" + coid ;
                    var u = conn.Query<ScoCompanySingle>(wheresql).AsList();            
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
        ///客户基本资料新增
        ///</summary>
        public static DataResult InsertScoCompany(int CoID,ScoCompanySingle com,string UserName,string Company)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlCommandText = @"INSERT INTO supplycompany(sconame,scosimple,enable,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark,creator,coid) VALUES(
                            @Sconame,
                            @Scosimple,
                            @Enable,
                            @Scocode,
                            @Address,
                            @Country,
                            @Contactor,
                            @Tel,
                            @Phone,
                            @Fax,
                            @Url,
                            @Email,
                            @Typelist,
                            @Bank,
                            @Bankid,
                            @Taxid,
                            @Remark,
                            @UName,
                            @Coid
                        )";
                    var args = new {Sconame = com.sconame,Scosimple = com.scosimple,Enable=com.enable,Scocode = com.scocode,Address = com.address,Country = com.country,
                                    Contactor = com.contactor,Tel = com.tel,Phone = com.phone,Fax = com.fax,Url = com.url,Email = com.email,Typelist = com.typelist,
                                    Bank = com.bank,Bankid = com.bankid,Taxid = com.taxid,Remark = com.remark,UName = UserName,Coid = CoID};
                    int count =conn.Execute(sqlCommandText,args);
                    if(count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        int rtn = conn.QueryFirst<int>("select LAST_INSERT_ID()");
                        result.d = rtn;
                        CoreUser.LogComm.InsertUserLog("新增客户资料", "supplycompany", "新增客户" + com.sconame ,UserName, CoID, DateTime.Now);
                        string wheresql = "select id,sconame,scosimple,enable,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark from supplycompany where sconame ='" + com.sconame + "' and coid =" + CoID ;
                        var u = conn.Query<ScoCompanySingle>(wheresql).AsList();
                        if (u.Count > 0)
                        {
                            CacheBase.Set<ScoCompanySingle>("scocompany" + u[0].id.ToString(), u[0]);
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
        ///<summary>
        ///客户基本资料更新
        ///</summary>
        public static DataResult UpdateScoCompany(int CoID,ScoCompanySingle com,string UserName,string Company)
        {
            var result = new DataResult(1,null);  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select id,sconame,scosimple,enable,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark from supplycompany where id =" + com.id;
                    var u = conn.Query<ScoCompanySingle>(wheresql).AsList();
                    var p = new DynamicParameters();
                    string uptsql = @"update supplycompany set ";
                    if(com.sconame != u[0].sconame)
                    {
                        contents = contents + "客户名称" + ":" +u[0].sconame + "=>" + com.sconame + ";";
                    }
                    if(com.enable != u[0].enable)
                    {
                        contents = contents + "启用状态" + ":" +u[0].enable + "=>" + com.enable + ";";
                    }
                    if(com.scosimple != u[0].scosimple)
                    {
                        contents = contents + "客户简称" + ":" +u[0].scosimple + "=>" + com.scosimple + ";";
                    }
                    if(com.scocode != u[0].scocode)
                    {
                        contents = contents + "客户编号" + ":" +u[0].scocode + "=>" + com.scocode + ";";
                    }
                    if(com.address != u[0].address)
                    {
                        contents = contents + "客户地址" + ":" +u[0].address + "=>" + com.address + ";";
                    }
                    if(com.country != u[0].country)
                    {
                        contents = contents + "国家" + ":" +u[0].country + "=>" + com.country + ";";
                    }
                    if(com.contactor != u[0].contactor)
                    {
                        contents = contents + "联系人" + ":" +u[0].contactor + "=>" + com.contactor + ";";
                    }
                    if(com.tel != u[0].tel)
                    {
                        contents = contents + "固定电话" + ":" +u[0].tel + "=>" + com.tel + ";";
                    }
                    if(com.phone != u[0].phone)
                    {
                        contents = contents + "移动电话" + ":" +u[0].phone + "=>" + com.phone + ";";
                    }
                    if(com.fax != u[0].fax)
                    {
                        contents = contents + "传真" + ":" +u[0].fax + "=>" + com.fax + ";";
                    }
                    if(com.url != u[0].url)
                    {
                        contents = contents + "主页" + ":" +u[0].url + "=>" + com.url + ";";
                    }
                    if(com.bank != u[0].bank)
                    {
                        contents = contents + "开户银行" + ":" +u[0].bank + "=>" + com.bank + ";";
                    }
                    if(com.email != u[0].email)
                    {
                        contents = contents + "客户邮箱" + ":" +u[0].email + "=>" + com.email + ";";
                    }
                    if(com.typelist != u[0].typelist)
                    {
                        contents = contents + "公司类型" + ":" +u[0].typelist + "=>" + com.typelist + ";";
                    }
                    if(com.bankid != u[0].bankid)
                    {
                        contents = contents + "开户账号" + ":" +u[0].bankid + "=>" + com.bankid + ";";
                    }
                    if(com.taxid != u[0].taxid)
                    {
                        contents = contents + "税号" + ":" +u[0].taxid + "=>" + com.taxid + ";";
                    }
                    if(com.remark != u[0].remark)
                    {
                        contents = contents + "备注" + ":" +u[0].remark + "=>" + com.remark + ";";
                    }
                    if(com.sconame != null)
                    {
                        uptsql = uptsql + "sconame = @Sconame,";
                        p.Add("@Sconame", com.sconame);
                    }
                    // if(com.enable != u[0].enable)
                    // {
                    //     uptsql = uptsql + "enable = @Enable,";
                    //     p.Add("@Enable", com.enable);
                    // }
                    if(com.scosimple != null)
                    {
                        uptsql = uptsql + "scosimple = @Scosimple,";
                        p.Add("@Scosimple", com.scosimple);
                    }
                    if(com.scocode != null)
                    {
                        uptsql = uptsql + "scocode = @Scocode,";
                        p.Add("@Scocode", com.scocode);
                    }
                    if(com.address != null)
                    {
                        uptsql = uptsql + "address = @Address,";
                        p.Add("@Address", com.address);
                    }
                    if(com.country != null)
                    {
                        uptsql = uptsql + "country = @Country,";
                        p.Add("@Country", com.country);
                    }
                    if(com.contactor != null)
                    {
                        uptsql = uptsql + "contactor = @Contactor,";
                        p.Add("@Contactor", com.contactor);
                    }
                    if(com.tel != null)
                    {
                        uptsql = uptsql + "tel = @Tel,";
                        p.Add("@Tel", com.tel);
                    }
                    if(com.phone != null)
                    {
                        uptsql = uptsql + "phone = @Phone,";
                        p.Add("@Phone", com.phone);
                    }
                    if(com.fax != null)
                    {
                        uptsql = uptsql + "fax = @Fax,";
                        p.Add("@Fax", com.fax);
                    }
                    if(com.url != null)
                    {
                        uptsql = uptsql + "url = @Url,";
                        p.Add("@Url", com.url);
                    }
                    if(com.bank != null)
                    {
                        uptsql = uptsql + "bank = @Bank,";
                        p.Add("@Bank", com.bank);
                    }
                    if(com.email != null)
                    {
                        uptsql = uptsql + "email = @Email,";
                        p.Add("@Email", com.email);
                    }
                    if(com.typelist != null)
                    {
                        uptsql = uptsql + "typelist = @Typelist,";
                        p.Add("@Typelist", com.typelist);
                    }
                    if(com.bankid != null)
                    {
                        uptsql = uptsql + "bankid = @Bankid,";
                        p.Add("@Bankid", com.bankid);
                    }
                    if(com.taxid != null)
                    {
                        uptsql = uptsql + "taxid = @Taxid,";
                        p.Add("@Taxid", com.taxid);
                    }
                    if(com.remark != null)
                    {
                        uptsql = uptsql + "remark = @Remark,";
                        p.Add("@Remark", com.remark);
                    }
                    if(uptsql.Substring(uptsql.Length - 1, 1) == ",")
                    {
                        uptsql = uptsql.Substring(0,uptsql.Length - 1);
                        uptsql = uptsql + " where id = @ID";
                        p.Add("@ID",com.id);
                    }
                    else
                    {
                        result.s = 1;
                        result.d = null;
                        return result;
                    }
                    int count = conn.Execute(uptsql,p);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLog("修改客户资料", "supplycompany", contents, UserName, CoID, DateTime.Now);               
                        CacheBase.Set<ScoCompanySingle>("scocompany" + com.id.ToString(), com);
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
        ///查询全部的公司资料
        ///</summary>
        public static DataResult GetScoCompanyAll(int CoID)
        {
            var result = new DataResult(1,null);     
            string wheresql = "where enable = true";
            if(CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + CoID;
            }
            wheresql = "select id,scocode,enable,sconame,scosimple,typelist,remark,creator,createdate from supplycompany " + wheresql ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<ScoCompanyMulti>(wheresql).AsList();
                    if (u.Count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        result.d = u;
                    }               
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
    }
}