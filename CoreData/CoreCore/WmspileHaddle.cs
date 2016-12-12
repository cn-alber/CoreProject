using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreModels;
using CoreModels.XyCore;
using Dapper;
using MySql.Data.MySqlClient;

namespace CoreData.CoreCore
{
    public static class WmspileHaddle{
        public static DataResult getPileList(string CoID,string wareid,string area="",string row="",string col="",string storey="",string cell=""){
            var res = new DataResult(1, null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sql = @"SELECT ID,SkuID,Skuautoid,PCode,`Enable`,Area,`Row`,Col,Storey,Cell, `Order` FROM wmspile WHERE CoID = "+CoID+" AND IsDelete = FALSE AND WarehouseID ="+wareid+" ";
                    string whereSql = "";
                    if(!string.IsNullOrEmpty(area)){
                        whereSql += " AND Area = '"+area+"' ";
                    }
                    if(!string.IsNullOrEmpty(row)){
                        whereSql += " AND `Row` = '"+row+"' ";
                    }
                    if(!string.IsNullOrEmpty(col)){
                        whereSql += " AND Col = '"+col+"' ";
                    }
                    if(!string.IsNullOrEmpty(storey)){
                        whereSql += " AND Storey = '"+storey+"' ";
                    }
                    if(!string.IsNullOrEmpty(cell)){
                        whereSql += " AND Cell = '"+cell+"' ";
                    }                    
                    var relist =  conn.Query<Pilelist>(sql+whereSql+" ORDER BY `Order` ").AsList();
                    var list = conn.Query<Pilelist>(sql+" ORDER BY `Order` ").AsList();
                    
                    var subs = list.GroupBy(i=>i.Area).Select(g => new Sub{ parent = g.FirstOrDefault().Area , name="区域"}).ToList();
                    foreach(var rowsub in subs ){
                        if(!string.IsNullOrEmpty(rowsub.parent)){
                            rowsub.children = list.Where(i=>i.Area == rowsub.parent ).GroupBy(i=>i.Row).Select(g => new Sub{ parent = g.FirstOrDefault().Row, name="行"}).ToList();
                            foreach(var colsub in rowsub.children){
                                if(!string.IsNullOrEmpty(colsub.parent)){
                                    colsub.children = list.Where(i=>i.Row == colsub.parent ).GroupBy(i=>i.Col).Select(g => new Sub{ parent = g.FirstOrDefault().Col, name="列"}).ToList();
                                    foreach(var storeysub in colsub.children){
                                        if(!string.IsNullOrEmpty(storeysub.parent)){
                                            storeysub.children = list.Where(i=>i.Col == storeysub.parent ).GroupBy(i=>i.Storey).Select(g => new Sub{ parent = g.FirstOrDefault().Storey, name="层"}).ToList();
                                            if(!string.IsNullOrEmpty(storeysub.children[0].parent.Replace(" ",""))){
                                                foreach(var cellsub in storeysub.children){
                                                    cellsub.children = list.Where(i=>i.Storey == cellsub.parent ).GroupBy(i=>i.Cell).Select(g => new Sub{ parent = g.FirstOrDefault().Cell, name="格子"}).ToList();
                                                }   
                                            } else {
                                                storeysub.children =null;
                                            }     
                                        }                                                                                                 
                                    }
                                }                                
                            }
                        }                        
                    }                    
                    res.d=new {
                        submenu = subs,
                        list = relist
                    };                                  
                }
                catch
                {
                    conn.Dispose();
                }
            }

            return res;
        }

         public static DataResult getWmsPileList(string CoID,string area="",string row="",string col="",string storey="",string cell=""){
            var res = new DataResult(1, null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sql = @"SELECT ID,SkuID,Skuautoid,PCode,`Enable`,Area,`Row`,Col,Storey,Cell FROM wmspile WHERE CoID = 1 AND IsDelete = FALSE ";
                    string whereSql = "";
                    if(!string.IsNullOrEmpty(area)){
                        whereSql += " AND Area = '"+area+"' ";
                    }
                    if(!string.IsNullOrEmpty(row)){
                        whereSql += " AND `Row` = '"+row+"' ";
                    }
                    if(!string.IsNullOrEmpty(col)){
                        whereSql += " AND Col = '"+col+"' ";
                    }
                    if(!string.IsNullOrEmpty(storey)){
                        whereSql += " AND Storey = '"+storey+"' ";
                    }
                    if(!string.IsNullOrEmpty(cell)){
                        whereSql += " AND Cell = '"+cell+"' ";
                    }                    
                    var list = conn.Query<Pilelist>(sql+whereSql+" ORDER BY wmspile.`Order` ASC ").AsList();
                         
                    res.d=new {
                        list = list
                    };                                  
                }
                catch
                {
                    conn.Dispose();
                }
            }

            return res;
        }

        



        public static int getNum(string letter){
            byte[] array = System.Text.Encoding.ASCII.GetBytes(letter);
            return (int)(array[0]);
        }

        public static DataResult insertpile(PileInsert pile,string uname, string CoID){
            var res = new DataResult(1, null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    var row = new List<string>();
                    var col = new List<string>();
                    var storey = new List<string>();
                    var cell = new List<string>();
                    var area = new List<string>(){pile.area};
                    if(char.IsNumber(pile.row[0].ToCharArray()[0]) && char.IsNumber(pile.row[1].ToCharArray()[0])){
                        for(int i = Convert.ToInt16(pile.row[0]);i<Convert.ToInt16(pile.row[1])+1;i++){
                            row.Add(i.ToString());
                        }
                    } else {
                        for(int i = getNum(pile.row[0]);i<getNum(pile.row[1])+1;i++){
                            row.Add(Convert.ToChar(i).ToString());
                        }                       
                    }
                    if(char.IsNumber(pile.col[0].ToCharArray()[0]) && char.IsNumber(pile.col[1].ToCharArray()[0])){
                        for(int i = Convert.ToInt16(pile.col[0]);i<Convert.ToInt16(pile.col[1])+1;i++){
                            col.Add(i.ToString());
                        }

                    } else {
                        for(int i = getNum(pile.col[0]);i<getNum(pile.col[1])+1;i++){
                            col.Add(Convert.ToChar(i).ToString());
                        }
                    }
                    if(pile.storey != null){
                        if(char.IsNumber(pile.storey[0].ToCharArray()[0]) && char.IsNumber(pile.storey[1].ToCharArray()[0])){
                            for(int i = Convert.ToInt16(pile.storey[0]);i<Convert.ToInt16(pile.storey[1])+1;i++){
                                storey.Add(i.ToString());
                            }
                        } else {
                            for(int i = getNum(pile.storey[0]);i<getNum(pile.storey[1])+1;i++){
                                storey.Add(Convert.ToChar(i).ToString());
                            }
                        }
                    }

                    if (pile.cell!=null){
                        if(char.IsNumber(pile.cell[0].ToCharArray()[0]) && char.IsNumber(pile.cell[1].ToCharArray()[0])){
                            for(int i = Convert.ToInt16(pile.cell[0]);i<Convert.ToInt16(pile.cell[1])+1;i++){
                                cell.Add(i.ToString());
                            }
                        } else {
                            for(int i = getNum(pile.cell[0]);i<getNum(pile.cell[1])+1;i++){
                                cell.Add(Convert.ToChar(i).ToString());
                            }
                        }
                    }
                    

                    //把需要进行笛卡尔积计算的集合放入到 List<List<string>>对象中  
                    List<List<string>> dimvalue = new List<List<string>>(); 
                    dimvalue.Add(area); 
                    dimvalue.Add(row);  
                    dimvalue.Add(col);  
                    if(storey.Count != 0 ){
                        dimvalue.Add(storey);
                    }
                    if(cell.Count != 0 ){
                        dimvalue.Add(cell);
                    }

                    //建立结果容器  List<string> result  
                    List<string> result = new List<string>();  
                    //传递给计算方法中计算  
                    Descartes(dimvalue, 0, result, string.Empty);  
                    StringBuilder sql = new StringBuilder("");
                    string maxOrder = "SELECT MAX(`Order`) FROM wmspile";
                    int oIndex = conn.Query<int>(maxOrder).AsList()[0]; 
                    oIndex++;  
                    //遍历查询结果  
                    foreach (string s in result)  
                    {  
                        var sp =s.Split('-');
                        var count = sp.Length;
                        var cellparam = count>4 ? sp[4] : " ";
                        var storeyparam = count>3 ? sp[3] : " ";
                        sql.Append(@"INSERT wmspile SET 
                                        wmspile.PCode = '"+s+@"',
                                        wmspile.Area='"+sp[0]+@"',
                                        wmspile.`Row`='"+sp[1]+@"',
                                        wmspile.Cell='"+cellparam+@"',
                                        wmspile.CoID='"+CoID+@"',
                                        wmspile.Col='"+sp[2]+@"',
                                        wmspile.Storey='"+storeyparam+@"',
                                        wmspile.CreateDate=NOW(),
                                        wmspile.PCType=1,
                                        wmspile.Type='"+pile.Type+@"',
                                        wmspile.Creator='"+uname+@"',
                                        wmspile.`Enable`=TRUE,
                                        wmspile.`Order` = "+(oIndex++)+@",
                                        wmspile.WarehouseID="+pile.WarehouseID+@",
                                        wmspile.WarehouseName='"+pile.WarehouseName+"';");
                    }  

                    res.d = conn.Execute(sql.ToString());                             
                }
                catch(Exception ex)
                {
                    res.s = -1;
                    if (ex.Message.IndexOf("Duplicate entry")>-1){
                        var pcode = ex.Message.Split('\'')[1];
                        res.d = "仓位："+ pcode.Substring(0,pcode.Length - 3) + " 已存在";
                    }else {
                        res.d = ex.Message;
                    }
                    
                    conn.Dispose();
                }
            }

            return res;
        }

        public static DataResult deletepile(List<string> ids, string CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var sql = "UPDATE  wmspile SET IsDelete=TRUE WHERE ID in(" + string.Join(",", ids.ToArray()) + ") AND CoID = " + CoID;
                    var rnt = conn.Execute(sql);
                    if (rnt > 0)
                    {
                        res.s = 1;
                    }
                    else
                    {
                        res.s = -1;
                    }
                }
                catch (Exception ex)
                {
                    res.s = -1;
                    res.d = ex.Message;
                    conn.Dispose();
                }
            }

            return res;
        }
        
        public static DataResult pileGetOrder(string CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var sql = "SELECT DISTINCT(wmspile.`Row`) FROM wmspile WHERE wmspile.`Row` <> '' AND CoID = @CoID ORDER BY wmspile.`Order` ASC";
                    var list = conn.Query<string>(sql,new {
                        CoID = CoID
                    }).AsList();
                    res.d = string.Join(",", list.ToArray());
                }
                catch (Exception ex)
                {
                    res.s = -1;
                    res.d = ex.Message;
                    conn.Dispose();
                }
            }

            return res;
        }

        public static DataResult pileAndSku(string CoID,int page,int pageSize)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var sql = "SELECT ID,Skuautoid,PCode from wmspile WHERE Skuautoid <>0 AND CoID="+CoID+ " limit "+(page-1)*0 +","+page*pageSize;
                    var total = conn.Query<decimal>("SELECT COUNT(ID) from wmspile WHERE Skuautoid <>0 AND CoID= "+CoID).AsList()[0];
                    var pageCount = Math.Ceiling(total/pageSize);

                    var pList1 = conn.Query<plist1>(sql).AsList();
                    var ids = from n in pList1
                        select n.Skuautoid;                    
                    sql = "SELECT ID, SkuID, SkuName, Img,GoodsCode from coresku WHERE ID in("+string.Join(",", ids.ToArray())+")";
                    var pList2 = conn.Query<plist2>(sql).AsList();
                    var plist = new List<pileAndSku>();
                    foreach(var p in pList1){
                        if(p.Skuautoid !=0) {
                             var t = pList2.Where(i => i.id == p.Skuautoid).First();                            
                            plist.Add(new pileAndSku(){
                                ID = p.id,
                                Skuautoid = p.Skuautoid,
                                PCode = p.PCode,
                                SkuID = t.SkuID,
                                SkuName = t.SkuName,
                                Img = t.Img,
                                GoodsCode = t.GoodsCode
                            });
                        }                       
                    }
                    if(page ==1){
                        res.d = new {
                            list = plist,
                            page = page,
                            pageSize = pageSize,
                            pageCount = pageCount,
                            total = total
                        };    
                    }else{
                        res.d = new {
                            list = plist,
                            page = page
                        };    
                    }
                    

                }
                catch (Exception ex)
                {
                    res.s = -1;
                    res.d = ex.Message;
                    conn.Dispose();
                }
            }

            return res;
        }

        public static DataResult pileOrder(string editOrder, string CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    Console.WriteLine(editOrder);
                    var areaArr = editOrder.Split(',');
                    var sql = new StringBuilder();
                    for(var i=0;i<areaArr.Length;i++){
                        sql.Append("UPDATE  wmspile SET `Order`="+i+" WHERE Area='"+areaArr[i]+"' AND CoID = "+CoID+";");
                    }
                    Console.WriteLine(sql);
                    // var sql = "UPDATE  wmspile SET `Order`=@oIndex WHERE ID =@id AND CoID = @CoID";
                    var rnt = conn.Execute(sql.ToString());
                    if (rnt > 0)
                    {
                        res.s = 1;
                    }
                    else
                    {
                        res.s = -1;
                    }
                }
                catch (Exception ex)
                {
                    res.s = -1;
                    res.d = ex.Message;
                    conn.Dispose();
                }
            }

            return res;
        }


        public static DataResult delPileSKu(List<string> ids, string CoID)
        {
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {

                    var sql = "UPDATE  wmspile SET Skuautoid=0 WHERE ID in("+string.Join(",", ids.ToArray())+") AND CoID = "+CoID+";";                 
                    var rnt = conn.Execute(sql.ToString());
                    if (rnt > 0)
                    {
                        res.s = 1;
                    }
                    else
                    {
                        res.s = -1;
                    }
                }
                catch (Exception ex)
                {
                    res.s = -1;
                    res.d = ex.Message;
                    conn.Dispose();
                }
            }

            return res;
        }

        public static DataResult insertPileSKu(string PCode, string SkuID,string CoID){
            var res = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CoreConnectString))
            {
                try
                {
                    var sql ="SELECT ID from coresku WHERE SkuID = @SkuID";
                    var dataR = conn.Query<int>(sql, new {SkuID = SkuID}).AsList();
                    if (dataR.Count == 0){
                        res.s = -1;
                        res.d = "商品编码不存在";
                    } else {
                        sql ="UPDATE wmspile SET Skuautoid = @Skuautoid,SkuID = @SkuID WHERE PCode=@PCode";
                        Console.WriteLine(sql);
                        var rnt = conn.Execute(sql, new {Skuautoid=dataR[0],PCode = PCode,SkuID = SkuID});
                        if (rnt == 0) {
                            res.s = -1;
                            res.d = "仓位不存在";
                        }
                    }
                }
                catch (Exception ex)
                {
                    res.s = -1;
                    res.d = ex.Message;
                    conn.Dispose();
                }
            }

            return res;

        }


        /// <summary>  
        /// 笛卡尔积  
        /// </summary>  
        private static string Descartes(List<List<string>> list, int count, List<string> result, string data)
        {
            data = string.IsNullOrEmpty(data) ? data : data+"-";
            string temp = data;
            //获取当前数组
            List<string> astr = list[count];
            //循环当前数组
            foreach (var item in astr)
            {
                if (count + 1 < list.Count)
                {
                    temp += Descartes(list, count + 1, result, data + item);
                }
                else
                {
                    result.Add(data  + item);
                }
            }
            return temp;
        }









    }
}