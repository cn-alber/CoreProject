using CoreModels;
using Dapper;
using System;
using CoreModels.XyComm;
using CoreData;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData.CoreCore;
using CoreModels.XyCore;
using static CoreModels.Enum.OrderE;
using System.Threading.Tasks;
using System.Text;

namespace CoreDate.CoreComm
{
    public static class PrinterHaddle
    {
        
         /// <summary>
		/// 打印机列表
		/// </summary>
        public static DataResult getPrinterLst(PrinterParam param){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{                
                    var sql = new StringBuilder();     
                    var totalSql = new StringBuilder();       
                    var p = new DynamicParameters();
                    sql.Append(@"SELECT 
                                    ID,
                                    `Name`,
                                    PrintType,
                                    PrintName,
                                    IPAddress,
                                    Enabled,
                                    IsDefault,
                                    PrinterPort 
                                FROM
                                    printer
                                WHERE CoID = @CoID AND IsDelete = 0 ");
                    totalSql.Append(@"SELECT COUNT(ID) FROM printer WHERE CoID = @CoID AND IsDelete = 0");
                    p.Add("@CoID", param.CoID);
                    if(param.Type > 0){
                        sql.Append(" AND PrintType = @Type ");
                        totalSql.Append(" AND PrintType = @Type ");
                        p.Add("@Type", param.Type);
                    }
                    if (!string.IsNullOrEmpty(param.Enabled) && param.Enabled.ToUpper() != "ALL")//是否启用
                    {
                        sql.Append(" AND Enabled = @Enabled ");
                        totalSql.Append(" AND Enabled = @Enabled ");
                        p.Add("@Enabled", param.Enabled.ToUpper() == "TRUE" ? true : false);
                    }
                    if(!string.IsNullOrEmpty(param.Filter)){
                        sql.Append(" AND `IPAddress` = @IPAddress ");
                        totalSql.Append(" AND `IPAddress` = @IPAddress ");
                        p.Add("@IPAddress", param.Filter);
                    }
                    var total = conn.Query<decimal>(totalSql.ToString(), p).AsList()[0];
                    var pageCount = Math.Ceiling(total/decimal.Parse(param.PageSize.ToString()));
                    var lst = conn.Query<PrinterQuery>(sql.ToString(), p).AsList();

                    if (param.PageIndex == 1) {
                        result.d  = new {
                            total = total,
                            pageCount = pageCount,
                            lst = lst,
                            pageIndex = param.PageIndex,
                            pageSize = param.PageSize
                        };
                    } else {
                        result.d  = new {
                            total = total,
                            pageCount = pageCount,
                            lst = lst
                        };
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }

        public static DataResult getPrintQuery(int id,string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string sql = @"SELECT 
                                    ID,
                                    Name,
                                    PrintType,
                                    PrintName,
                                    IPAddress,
                                    Enabled,
                                    IsDefault,
                                    PrinterPort
                                   FROM 
                                    printer 
                                   WHERE ID=@id AND CoID=@CoID";
                    var rnt = conn.Query<PrinterQuery>(sql,new {
                        id = id,
                        CoID = CoID
                    }).AsList();
                    if(rnt.Count > 0) {
                        result.d = rnt[0];
                    }else{
                        result.s = -1;
                        result.d = "错误的ID";
                    }

                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }



        public static DataResult creatPrinter(PrinterInsert printer){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string sql = @"INSERT INTO printer SET 
                                    `NAME` = @NAME, 
                                    `CoID` = @CoID,
                                    `PrintType` = @PrintType,
                                    `PrintName` = @PrintName,
                                    `IPAddress` = @IPAddress,
                                    `Enabled` = @Enabled,
                                    `PrinterPort` = @PrinterPort";
                    var rnt = conn.Execute(sql,printer);
                    if(rnt > 0){
                        result.d = 1;
                    } else {
                        result.d = -1;
                    }

                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }

        public static DataResult modifyPrinter(PrinterInsert printer){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string sql = @"UPDATE printer SET 
                                    `NAME` = @NAME, 
                                    `PrintType` = @PrintType,
                                    `PrintName` = @PrintName,
                                    `IPAddress` = @IPAddress,
                                    `Enabled` = @Enabled,
                                    `PrinterPort` = @PrinterPort
                                    WHERE 
                                     ID=@ID AND `CoID` = @CoID";
                    var rnt = conn.Execute(sql,printer);
                    if(rnt > 0){
                        result.d = 1;
                    } else {
                        result.s = -1;
                    }

                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }
        public static DataResult delPrinter(List<string> ids, string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string sql = @"UPDATE printer SET IsDelete=TRUE WHERE ID in ("+string.Join(",", ids.ToArray())+") AND CoID=@CoID";
                    var rnt = conn.Execute(sql,new {
                        CoID = CoID
                    });
                    if(rnt > 0){
                        result.s = 1;
                    } else {
                        result.s = -1;
                    }

                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }

        public static DataResult enabledPrinter(List<string> ids, string CoID,bool Enabled){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string sql = @"UPDATE printer SET Enabled=@Enabled WHERE ID in ("+string.Join(",", ids.ToArray())+") AND CoID=@CoID";
                    var rnt = conn.Execute(sql,new {
                        Enabled = Enabled,
                        CoID = CoID
                    });
                    if(rnt > 0){
                        result.s = 1;
                    } else {
                        result.s = -1;
                    }

                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }
        public static DataResult defPrinter(int id, string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
 
                    string sql = @"SELECT PrintType FROM printer WHERE id=@id AND CoID=@CoID";//@"UPDATE printer SET IsDefault=@IsDefault WHERE ID=@id AND CoID=@CoID";
                    var res = conn.Query<int>(sql,new {
                        id = id,             
                        CoID = CoID
                    }).AsList();
                    if(res.Count>0) {
                        var type = res[0];
                        sql=@"UPDATE printer SET IsDefault=FALSE WHERE PrintType=@type AND CoID=@CoID;
                              UPDATE printer SET IsDefault=TRUE WHERE ID=@id AND CoID=@CoID";
                        var rnt = conn.Execute(sql,new {
                            id = id,    
                            type = type,         
                            CoID = CoID
                        });      
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