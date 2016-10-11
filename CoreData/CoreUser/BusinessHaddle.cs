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
                    string wheresql = "select id,ismergeorder,isautosetexpress,isignoresku,isautogoodsreviewed,isupdateskuall,isupdatepresalesku,isskulock,ispresaleskulock,ischeckfirst,"+
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
                    string sqlCommandText = @"INSERT INTO business(ismergeorder,isautosetexpress,isignoresku,isautogoodsreviewed,isupdateskuall,isupdatepresalesku,isskulock,ispresaleskulock,
                                                ischeckfirst,isjustcheckex,isautosendafftercheck,isneedkg,isautoremarks,isexceptions,cabinetheight,cabinetnumber,ispositionaccurate,goodsuniquecode,
                                                isgoodsrule,isbeyondcount,pickingmethod,tempnominus,mixedpicking,coid) 
                                            VALUES(@Ismergeorder,@Isautosetexpress,@Isignoresku,@Isautogoodsreviewed,@Isupdateskuall,@Isupdatepresalesku,@Isskulock,@Ispresaleskulock,
                                                @Ischeckfirst,@Isjustcheckex,@Isautosendafftercheck,@Isneedkg,@Isautoremarks,@Isexceptions,@Cabinetheight,@Cabinetnumber,@Ispositionaccurate,
                                                @Goodsuniquecode,@Isgoodsrule,@Isbeyondcount,@Pickingmethod,@Tempnominus,@Mixedpicking,@Coid)";
                    var args = new {Ismergeorder = bu.ismergeorder,Isautosetexpress = bu.isautosetexpress,Isignoresku = bu.isignoresku,Isautogoodsreviewed = bu.isautogoodsreviewed,
                                    Isupdateskuall = bu.isupdateskuall,Isupdatepresalesku = bu.isupdatepresalesku,Isskulock = bu.isskulock,Ispresaleskulock = bu.ispresaleskulock,
                                    Ischeckfirst = bu.ischeckfirst,Isjustcheckex = bu.isjustcheckex,Isautosendafftercheck = bu.isautosendafftercheck,Isneedkg = bu.isneedkg,
                                    Isautoremarks = bu.isautoremarks,Isexceptions = bu.isexceptions,Cabinetheight = bu.cabinetheight,Cabinetnumber = bu.cabinetnumber,
                                    Ispositionaccurate = bu.ispositionaccurate,Goodsuniquecode = bu.goodsuniquecode,Isgoodsrule = bu.isgoodsrule,Isbeyondcount = bu.isbeyondcount,
                                    Pickingmethod = bu.pickingmethod,Tempnominus = bu.tempnominus,Mixedpicking = bu.mixedpicking,Coid = CoID};
                    int count =conn.Execute(sqlCommandText,args);
                    if(count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        LogComm.InsertUserLog("新增业务流程资料", "business", "新增业务流程",UserName, Company, DateTime.Now);
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
        ///更新资料
        ///</summary>
        public static DataResult UpdateBusiness(Business bu,string UserName,string Company)
        {
            var result = new DataResult(1,"资料更新成功!");  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    // string wheresql = "select id,ismergeorder,isautosetexpress,isignoresku,isautogoodsreviewed,isupdateskuall,isupdatepresalesku,isskulock,ispresaleskulock,ischeckfirst,"+
                    //                    "isjustcheckex,isautosendafftercheck,isneedkg,isautoremarks,isexceptions,cabinetheight,cabinetnumber,ispositionaccurate,goodsuniquecode,"+
                    //                    "isgoodsrule,isbeyondcount,pickingmethod,tempnominus,mixedpicking from business where id = " + bu.id;
                    // var u = conn.Query<CompanySingle>(wheresql).AsList();
                    // if(com.name != u[0].name)
                    // {
                    //     contents = contents + "公司名称" + ":" +u[0].name + "=>" + com.name + ";";
                    // }
                    // if(com.enable != u[0].enable)
                    // {
                    //     contents = contents + "启用状态" + ":" +u[0].enable + "=>" + com.enable + ";";
                    // }
                    // if(com.address != u[0].address)
                    // {
                    //     contents = contents + "公司地址" + ":" +u[0].address + "=>" + com.address + ";";
                    // }
                    // if(com.email != u[0].email)
                    // {
                    //     contents = contents + "公司邮箱" + ":" +u[0].email + "=>" + com.email + ";";
                    // }
                    // if(com.contacts != u[0].contacts)
                    // {
                    //     contents = contents + "公司联络人" + ":" +u[0].contacts + "=>" + com.contacts + ";";
                    // }
                    // if(com.telphone != u[0].telphone)
                    // {
                    //     contents = contents + "固定电话" + ":" +u[0].telphone + "=>" + com.telphone + ";";
                    // }
                    // if(com.mobile != u[0].mobile)
                    // {
                    //     contents = contents + "移动电话" + ":" +u[0].mobile + "=>" + com.mobile + ";";
                    // }
                    // if(com.remark != u[0].remark)
                    // {
                    //     contents = contents + "备注" + ":" +u[0].remark + "=>" + com.remark + ";";
                    // }
                    // string uptsql = @"update company set name = @Name,enable = @Enable,address = @Address,email=@Email,contacts=@Contacts,telphone=@Telphone,mobile=@Mobile,remark=@Remark where id = @ID";
                    // var args = new {Name = com.name,Enable=com.enable,Address = com.address,Email = com.email,Contacts = com.contacts,
                    //                 Telphone = com.telphone,Mobile = com.mobile,Remark = com.remark,ID = com.id};
                    // int count = conn.Execute(uptsql,args);
                    // if(count < 0)
                    // {
                    //     result.s= -3003;
                    // }
                    // else
                    // {
                    //     LogComm.InsertUserLog("修改公司资料", "company", contents, UserName, Company, DateTime.Now);               
                    //     CacheBase.Set<CompanySingle>("company" + com.id.ToString(), com);
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