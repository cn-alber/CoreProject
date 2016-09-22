using System;
namespace CoreModels.XyUser
{
    public class Log
    {
        #region Model
        private int _ID;
        private string _Name;
        private string _LogType;
        private string _Contents;
        private string _UserName;
        private string _Compnay;
        private DateTime _LogDate = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set
            {                
                _ID = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }
        /// <summary>
        /// 修改类型
        /// </summary>
        public string LogType
        {
            get { return _LogType; }
            set
            {
                _LogType = value;
            }
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string Contents
        {
            get { return _Contents; }
            set
            {
                _Contents = value;
            }
        }
        /// <summary>
        /// 操作人
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
            }
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Compnay
        {
            get { return _Compnay; }
            set
            {
                _Compnay = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LogDate
        {
            get { return _LogDate; }
            set
            {
                _LogDate = value;
            }
        }
        #endregion

    }

}