using CoreModels;
using CoreModels.XyUser;
using Dapper;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CoreData.CoreUser
{
    public static class BusinessHaddle
    {
        ///<summary>
        ///查询资料
        ///</summary>
        public static DataResult GetBusiness(int CoID)
        {
            var result = new DataResult(1,null);        
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string wheresql = "select ismergeorder,isautosetexpress,isignoresku,isautogoodsreviewed,isupdateskuall,isupdatepresalesku,isskulock,ispresaleskulock,ischeckfirst,"+
                                       "isjustcheckex,isautosendafftercheck,isneedkg,isautoremarks,isexceptions,cabinetheight,cabinetnumber,ispositionaccurate,goodsuniquecode,"+
                                       "isgoodsrule,isbeyondcount,pickingmethod,tempnominus,mixedpicking from business where coid = " + CoID;
                    var u = conn.Query<Business>(wheresql).AsList();
                    if (u.Count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
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
        ///新增资料
        ///</summary>
        public static DataResult InsertBusiness(Business bu,string UserName,string Company,int CoID)
        {
            var result = new DataResult(1,"资料新增成功!");   
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    // string sqlCommandText = @"INSERT INTO business(ismergeorder,isautosetexpress,isignoresku,isautogoodsreviewed,isupdateskuall,isupdatepresalesku,isskulock,ispresaleskulock,
                    //                             ischeckfirst,isjustcheckex,isautosendafftercheck,isneedkg,isautoremarks,isexceptions,cabinetheight,cabinetnumber,ispositionaccurate,goodsuniquecode,
                    //                             isgoodsrule,isbeyondcount,pickingmethod,tempnominus,mixedpicking) 
                    //                         VALUES(
                    //     )";
                    // var args = new {Name = com.name,Enable=com.enable,Address = com.address,Email = com.email,Contacts = com.contacts,
                    //                 Telphone = com.telphone,Mobile = com.mobile,Remark = com.remark,UName = UserName};
                    // int count =conn.Execute(sqlCommandText,args);
                    // if(count < 0)
                    // {
                    //     result.s = -3003;
                    // }
                    // else
                    // {
                    //     LogComm.InsertUserLog("新增公司资料", "company", "新增公司" + com.name ,UserName, Company, DateTime.Now);
                    //     string wheresql = "select id,name,enable,address,email,contacts,telphone,mobile,remark from company where name ='" + com.name + "'" ;
                    //     var u = conn.Query<CompanySingle>(wheresql).AsList();
                    //     if (u.Count > 0)
                    //     {
                    //         CacheBase.Set<CompanySingle>("company" + u[0].id.ToString(), u[0]);
                    //     }
                    // }        
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