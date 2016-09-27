using MySql.Data.MySqlClient;

namespace CoreData
{
    public static class DbBase
    {
        public static MySqlConnection UserDB = new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xyuser;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        public static MySqlConnection GoodsDB = new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xygoods;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        public static MySqlConnection CoreDB = new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xycore;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        public static MySqlConnection CommDB = new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xycomm;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        public static MySqlConnection MsgDB = new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xymessage;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");

        public static readonly string UserConnectString = "server=xieyuntestout.mysql.rds.aliyuncs.com;database=xyuser;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None";
        public static readonly string GoodsConnectString = "server=xieyuntestout.mysql.rds.aliyuncs.com;database=xygoods;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None";
        public static readonly string CoreConnectString = "server=xieyuntestout.mysql.rds.aliyuncs.com;database=xycore;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None";
        public static readonly string CommConnectString = "server=xieyuntestout.mysql.rds.aliyuncs.com;database=xycomm;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None";
        public static readonly string MsgConnectString = "server=xieyuntestout.mysql.rds.aliyuncs.com;database=xymessage;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None";
        // public static MySqlConnection UserDB = new MySqlConnection("server=xieyuntest.mysql.rds.aliyuncs.com;database=xyuser;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        // public static MySqlConnection GoodsDB = new MySqlConnection("server=xieyuntest.mysql.rds.aliyuncs.com;database=xygoods;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        // public static MySqlConnection CoreDB = new MySqlConnection("server=xieyuntest.mysql.rds.aliyuncs.com;database=xycore;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        // public static MySqlConnection CommDB = new MySqlConnection("server=xieyuntest.mysql.rds.aliyuncs.com;database=xycomm;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        // public static MySqlConnection MsgDB = new MySqlConnection("server=xieyuntest.mysql.rds.aliyuncs.com;database=xymessage;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
    }

    public class DapperDbBase
    {
        public MySqlConnection UserDB()
        {
            return new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xyuser;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None");
        }
    }

    // public interface IDapperDbBase
    // {
    //     MySqlConnection UserDB();
    // }
}