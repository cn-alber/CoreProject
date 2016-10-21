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
            string sqlCount = "select count(id) from supplycompany where 1 = 1";
            string sqlCommand = "select * from supplycompany where 1 = 1";
            string wheresql = string.Empty;
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
                    int count = conn.QueryFirst<int>(sqlCount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<ScoCompany>(sqlCommand + wheresql).AsList();

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
        ///公司启用停用设置
        ///</summary>
        public static DataResult UpdateScoComEnable(List<int> id,int CoID,string UserName,bool Enable)
        {
            var result = new DataResult(1,null);   
            string contents = string.Empty;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string uptsql = @"update supplycompany set enable = @Enable where id in @ID and coid = @Coid";
                    var args = new {ID = id,Enable = Enable,Coid = CoID};          
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                        return  result;
                    }
                    if(Enable)
                    {
                        contents = "客户状态启用";
                    }
                    else
                    {
                        contents = "客户状态停用";
                    }
                    CoreUser.LogComm.InsertUserLog("修改客户资料", "supplycompany", contents, UserName, CoID, DateTime.Now);
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
        public static DataResult GetScoCompanyEdit(int ID,int CoID)
        {
            var result = new DataResult(1,null);   
            string scoName = "scocompany" + ID.ToString();     
            var parent = CacheBase.Get<ScoCompany>(scoName);  
            if (parent == null)
            {
                using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                    try{
                        string wheresql = "select * from supplycompany where id =" + ID + " and coid =" + CoID;
                        var u = conn.Query<ScoCompany>(wheresql).AsList();
                        if (u.Count == 0)
                        {
                            result.s = -3001;
                            result.d = null;
                        }
                        else
                        {
                            CacheBase.Set<ScoCompany>(scoName, u[0]);
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
        ///检查客户资料是否已经存在
        ///</summary>
        public static DataResult IsScoComExist(string name,int coid)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select count(1) from supplycompany where sconame ='" + name + "' and coid =" + coid ;
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
        ///客户基本资料新增
        ///</summary>
        public static DataResult InsertScoCompany(int CoID,ScoCompany com,string UserName)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    if(com.sconame == null)
                    {
                        result.s = -1;
                        result.d = "名称必须有值!";
                        return  result;
                    }
                    string sqlCommandText = @"INSERT INTO supplycompany(sconame,scosimple,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark,coid,creator,modifier) VALUES(
                            @Sconame,
                            @Scosimple,
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
                            @Coid,
                            @Creator,
                            @Modifier
                        )";
                    com.coid = CoID;
                    com.createdate = DateTime.Now;
                    com.creator = UserName;
                    com.modifier = UserName;
                    com.modifydate = DateTime.Now;
                    int count =conn.Execute(sqlCommandText,com);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return  result;
                    }
                    int rtn = conn.QueryFirst<int>("select LAST_INSERT_ID()");
                    result.d = rtn;
                    CoreUser.LogComm.InsertUserLog("新增客户资料", "supplycompany", "新增客户" + com.sconame ,UserName, CoID, DateTime.Now);
                    com.id = rtn;
                    CacheBase.Set<ScoCompany>("scocompany" + rtn, com);  
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
        public static DataResult UpdateScoCompany(int CoID,ScoCompany com,string UserName)
        {
            var result = new DataResult(1,null);  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select * from supplycompany where id =" + com.id + " and coid = " + CoID;
                    var u = conn.Query<ScoCompany>(wheresql).AsList();
                    var supply = u[0] as ScoCompany;
                    if(com.sconame != null)
                    {
                        if(com.sconame != u[0].sconame)
                        {
                            contents = contents + "客户名称" + ":" +u[0].sconame + "=>" + com.sconame + ";";
                            supply.sconame = com.sconame;
                        }
                    }
                    if(com.scosimple != null)
                    {
                        if(com.scosimple != u[0].scosimple)
                        {
                            contents = contents + "客户简称" + ":" +u[0].scosimple + "=>" + com.scosimple + ";";
                            supply.scosimple = com.scosimple;
                        }
                    }
                    if(com.scocode != null)
                    {
                        if(com.scocode != u[0].scocode)
                        {
                            contents = contents + "客户编号" + ":" +u[0].scocode + "=>" + com.scocode + ";";
                            supply.scocode = com.scocode;
                        }
                    }
                    if(com.address != null)
                    {
                        if(com.address != u[0].address)
                        {
                            contents = contents + "客户地址" + ":" +u[0].address + "=>" + com.address + ";";
                            supply.address = com.address;
                        }
                    }
                    if(com.country != null)
                    {
                        if(com.country != u[0].country)
                        {
                            contents = contents + "国家" + ":" +u[0].country + "=>" + com.country + ";";
                            supply.country = com.country;
                        }
                    }
                    if(com.contactor != null)
                    {
                        if(com.contactor != u[0].contactor)
                        {
                            contents = contents + "联系人" + ":" +u[0].contactor + "=>" + com.contactor + ";";
                            supply.contactor = com.contactor;
                        }
                    }
                    if(com.tel != null)
                    {
                        if(com.tel != u[0].tel)
                        {
                            contents = contents + "固定电话" + ":" +u[0].tel + "=>" + com.tel + ";";
                            supply.tel = com.tel;
                        }
                    }
                    if(com.phone != null)
                    {
                        if(com.phone != u[0].phone)
                        {
                            contents = contents + "移动电话" + ":" +u[0].phone + "=>" + com.phone + ";";
                            supply.phone = com.phone;
                        }
                    }
                    if(com.fax != null)
                    {
                        if(com.fax != u[0].fax)
                        {
                            contents = contents + "传真" + ":" +u[0].fax + "=>" + com.fax + ";";
                            supply.fax = com.fax;
                        }
                    }
                    if(com.url != null)
                    {
                        if(com.url != u[0].url)
                        {
                            contents = contents + "主页" + ":" +u[0].url + "=>" + com.url + ";";
                            supply.url = com.url;
                        }
                    }
                    if(com.bank != null)
                    {
                        if(com.bank != u[0].bank)
                        {
                            contents = contents + "开户银行" + ":" +u[0].bank + "=>" + com.bank + ";";
                            supply.bank = com.bank;
                        }
                    }
                    if(com.email != null)
                    {
                        if(com.email != u[0].email)
                        {
                            contents = contents + "客户邮箱" + ":" +u[0].email + "=>" + com.email + ";";
                            supply.email = com.email;
                        }
                    }
                    if(com.typelist != null)
                    {
                        if(com.typelist != u[0].typelist)
                        {
                            contents = contents + "公司类型" + ":" +u[0].typelist + "=>" + com.typelist + ";";
                            supply.typelist = com.typelist;
                        }
                    }
                    if(com.bankid != null)
                    {
                        if(com.bankid != u[0].bankid)
                        {
                            contents = contents + "开户账号" + ":" +u[0].bankid + "=>" + com.bankid + ";";
                            supply.bankid = com.bankid;
                        }
                    }
                    if(com.taxid != null)
                    {
                        if(com.taxid != u[0].taxid)
                        {
                            contents = contents + "税号" + ":" +u[0].taxid + "=>" + com.taxid + ";";
                            supply.taxid = com.taxid;
                        }
                    }
                    if(com.remark != null)
                    {
                        if(com.remark != u[0].remark)
                        {
                            contents = contents + "备注" + ":" +u[0].remark + "=>" + com.remark + ";";
                            supply.remark = com.remark;
                        }
                    }
                    supply.modifier = UserName;
                    supply.modifydate = DateTime.Now;
                    string uptsql = @"update supplycompany set sconame = @Sconame,scosimple = @Scosimple,scocode = @Scocode,address = @Address,country = @Country,contactor = @Contactor,
                                        tel = @Tel,phone = @Phone,fax = @Fax,url = @Url,bank = @Bank,email = @Email,typelist = @Typelist,bankid = @Bankid,taxid = @Taxid,remark = @Remark,
                                        modifier = @Modifier,modifydate = @Modifydate where id = @ID and coid = @Coid";
                    int count = conn.Execute(uptsql,supply);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLog("修改客户资料", "supplycompany", contents, UserName, CoID, DateTime.Now);               
                        CacheBase.Set<ScoCompany>("scocompany" + supply.id.ToString(), supply);
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
            wheresql = "select * from supplycompany " + wheresql ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<ScoCompany>(wheresql).AsList();
                    result.d = u;       
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