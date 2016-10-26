using CoreModels;
using CoreModels.XyUser;
using Dapper;
using System;
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
            var res = new BusinessData();
            var bu = new Business();
            bu.ismergeorder = true;
            bu.isautosetexpress = true;
            bu.isignoresku = false;
            bu.isautogoodsreviewed = false;
            bu.isupdateskuall = false;
            bu.isupdatepresalesku = false;
            bu.isskulock = true;
            bu.ispresaleskulock = true;
            bu.ischeckfirst = false;
            bu.isjustcheckex = true;
            bu.isautosendafftercheck = true;
            bu.isneedkg = false;
            bu.isautoremarks = true;
            bu.isexceptions = true;
            bu.ispositionaccurate = true;
            bu.goodsuniquecode = true;
            bu.isgoodsrule = true;;
            bu.isbeyondcount = true;
            bu.pickingmethod = true;
            bu.tempnominus = false;
            bu.mixedpicking = false; 
            res.businessInitData = bu;
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string wheresql = "select * from business where coid = " + CoID;
                    var u = conn.Query<Business>(wheresql).AsList();
                    res.businessData = u[0] as Business;
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
        ///更新资料
        ///</summary>
        public static DataResult UpdateBusiness(Business bu,string UserName,int CoID)
        {
            var result = new DataResult(1,"资料更新成功!");  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{
                    string wheresql = "select id,ismergeorder,isautosetexpress,isignoresku,isautogoodsreviewed,isupdateskuall,isupdatepresalesku,isskulock,ispresaleskulock,ischeckfirst,"+
                                       "isjustcheckex,isautosendafftercheck,isneedkg,isautoremarks,isexceptions,cabinetheight,cabinetnumber,ispositionaccurate,goodsuniquecode,"+
                                       "isgoodsrule,isbeyondcount,pickingmethod,tempnominus,mixedpicking from business where coid = " + CoID;
                    var u = conn.Query<Business>(wheresql).AsList();
                    if(bu.ismergeorder != u[0].ismergeorder)
                    {
                        contents = contents + "订单是否自动标记等待合并" + ":" +u[0].ismergeorder + "=>" + bu.ismergeorder + ";";
                    }
                    if(bu.isautosetexpress != u[0].isautosetexpress)
                    {
                        contents = contents + "订单审核是否自动计算快递" + ":" +u[0].isautosetexpress + "=>" + bu.isautosetexpress + ";";
                    }
                    if(bu.isignoresku != u[0].isignoresku)
                    {
                        contents = contents + "不限制库存发货" + ":" +u[0].isignoresku + "=>" + bu.isignoresku + ";";
                    }
                    if(bu.isautogoodsreviewed != u[0].isautogoodsreviewed)
                    {
                        contents = contents + "订单审核自动检查缺货" + ":" +u[0].isautogoodsreviewed + "=>" + bu.isautogoodsreviewed + ";";
                    }
                    if(bu.isupdateskuall != u[0].isupdateskuall)
                    {
                        contents = contents + "上传库存包含在途" + ":" +u[0].isupdateskuall + "=>" + bu.isupdateskuall + ";";
                    }
                    if(bu.isupdatepresalesku != u[0].isupdatepresalesku)
                    {
                        contents = contents + "允许上传预售商品的库存" + ":" +u[0].isupdatepresalesku + "=>" + bu.isupdatepresalesku + ";";
                    }
                    if(bu.isskulock != u[0].isskulock)
                    {
                        contents = contents + "库存锁" + ":" +u[0].isskulock + "=>" + bu.isskulock + ";";
                    }
                    if(bu.ispresaleskulock != u[0].ispresaleskulock)
                    {
                        contents = contents + "预售锁定库存" + ":" +u[0].ispresaleskulock + "=>" + bu.ispresaleskulock + ";";
                    }
                    if(bu.ischeckfirst != u[0].ischeckfirst)
                    {
                        contents = contents + "先验货才发货" + ":" +u[0].ischeckfirst + "=>" + bu.ischeckfirst + ";";
                    }
                    if(bu.isjustcheckex != u[0].isjustcheckex)
                    {
                        contents = contents + "验货出库只检验快递单号" + ":" +u[0].isjustcheckex + "=>" + bu.isjustcheckex + ";";
                    }
                    if(bu.isautosendafftercheck != u[0].isautosendafftercheck)
                    {
                        contents = contents + "验货完成自动发货" + ":" +u[0].isautosendafftercheck + "=>" + bu.isautosendafftercheck + ";";
                    }
                    if(bu.isneedkg != u[0].isneedkg)
                    {
                        contents = contents + "发货前称重" + ":" +u[0].isneedkg + "=>" + bu.isneedkg + ";";
                    }
                    if(bu.isautoremarks != u[0].isautoremarks)
                    {
                        contents = contents + "售后收货自动上传备注" + ":" +u[0].isautoremarks + "=>" + bu.isautoremarks + ";";
                    }
                    if(bu.isexceptions != u[0].isexceptions)
                    {
                        contents = contents + "售后换货订单自动生成" + ":" +u[0].isexceptions + "=>" + bu.isexceptions + ";";
                    }
                    if(bu.cabinetheight != u[0].cabinetheight)
                    {
                        contents = contents + "分拣柜层高设置" + ":" +u[0].cabinetheight + "=>" + bu.cabinetheight + ";";
                    }
                    if(bu.cabinetnumber != u[0].cabinetnumber)
                    {
                        contents = contents + "分拣柜总格数" + ":" +u[0].cabinetnumber + "=>" + bu.cabinetnumber + ";";
                    }
                    if(bu.ispositionaccurate != u[0].ispositionaccurate)
                    {
                        contents = contents + "仓位精确库存" + ":" +u[0].ispositionaccurate + "=>" + bu.ispositionaccurate + ";";
                    }
                    if(bu.goodsuniquecode != u[0].goodsuniquecode)
                    {
                        contents = contents + "商品唯一码" + ":" +u[0].goodsuniquecode + "=>" + bu.goodsuniquecode + ";";
                    }
                    if(bu.isgoodsrule != u[0].isgoodsrule)
                    {
                        contents = contents + "仓位货物置放规则" + ":" +u[0].isgoodsrule + "=>" + bu.isgoodsrule + ";";
                    }
                    if(bu.isbeyondcount != u[0].isbeyondcount)
                    {
                        contents = contents + "采购入库超入处理" + ":" +u[0].isbeyondcount + "=>" + bu.isbeyondcount + ";";
                    }
                    if(bu.pickingmethod != u[0].pickingmethod)
                    {
                        contents = contents + "拣货方式" + ":" +u[0].pickingmethod + "=>" + bu.pickingmethod + ";";
                    }
                    if(bu.tempnominus != u[0].tempnominus)
                    {
                        contents = contents + "拣货暂存禁止负库存" + ":" +u[0].tempnominus + "=>" + bu.tempnominus + ";";
                    }
                    if(bu.mixedpicking != u[0].mixedpicking)
                    {
                        contents = contents + "混合拣货" + ":" +u[0].mixedpicking + "=>" + bu.mixedpicking + ";";
                    }
                    string uptsql = @"update business set ismergeorder = @Ismergeorder,isautosetexpress = @Isautosetexpress,isignoresku = @Isignoresku,isautogoodsreviewed = @Isautogoodsreviewed,
                                        isupdateskuall = @Isupdateskuall,isupdatepresalesku = @Isupdatepresalesku,isskulock= @Isskulock,ispresaleskulock = @Ispresaleskulock,ischeckfirst = @Ischeckfirst,
                                        isjustcheckex = @Isjustcheckex,isautosendafftercheck = @Isautosendafftercheck,isneedkg = @Isneedkg,isautoremarks = @Isautoremarks,isexceptions = @Isexceptions,
                                        cabinetheight = @Cabinetheight,cabinetnumber = @Cabinetnumber,ispositionaccurate = @Ispositionaccurate,goodsuniquecode = @Goodsuniquecode,isgoodsrule = @Isgoodsrule,
                                        isbeyondcount = @Isbeyondcount,pickingmethod = @Pickingmethod,tempnominus = @Tempnominus,mixedpicking = @Mixedpicking where coid = @CoID";
                    var args = new {Ismergeorder = bu.ismergeorder,Isautosetexpress = bu.isautosetexpress,Isignoresku = bu.isignoresku,Isautogoodsreviewed = bu.isautogoodsreviewed,
                                    Isupdateskuall = bu.isupdateskuall,Isupdatepresalesku = bu.isupdatepresalesku,Isskulock = bu.isskulock,Ispresaleskulock = bu.ispresaleskulock,
                                    Ischeckfirst = bu.ischeckfirst,Isjustcheckex = bu.isjustcheckex,Isautosendafftercheck = bu.isautosendafftercheck,Isneedkg = bu.isneedkg,
                                    Isautoremarks = bu.isautoremarks,Isexceptions = bu.isexceptions,Cabinetheight = bu.cabinetheight,Cabinetnumber = bu.cabinetnumber,
                                    Ispositionaccurate = bu.ispositionaccurate,Goodsuniquecode = bu.goodsuniquecode,Isgoodsrule = bu.isgoodsrule,Isbeyondcount = bu.isbeyondcount,
                                    Pickingmethod = bu.pickingmethod,Tempnominus = bu.tempnominus,Mixedpicking = bu.mixedpicking,CoID = CoID};
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        LogComm.InsertUserLog("修改业务流程资料", "business", contents, UserName, CoID, DateTime.Now);               
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