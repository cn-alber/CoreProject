using System.Collections.Generic;

namespace CoreModels.XyUser
{
    public class Menus
    {
        #region Model
        private int _id;
        private string _name;
        private bool _enable = false;
        private string _iconfont;
        private string _newicon;
        private string _newiconpre;
        private string _navigateurl;
        private string _newurl;
        private string _imageurl;
        private string _remark;
        private int? _sortindex;
        private int _parentid = 0;
        private int _viewpowerid = 0;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Enable
        {
            set { _enable = value; }
            get { return _enable; }
        }
        /// <summary>
        /// 文字图标
        /// </summary>
        public string IconFont
        {
            set { _iconfont = value; }
            get { return _iconfont; }
        }
        /// <summary>
        /// 新系统文字图标
        /// </summary>
        public string NewIcon
        {
            set { _newicon = value; }
            get { return _newicon; }
        }
        /// <summary>
        /// fontawesome图标
        /// </summary>
        public string NewIconPre
        {
            set { _newiconpre = value; }
            get { return _newiconpre; }
        }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public string NavigateUrl
        {
            set { _navigateurl = value; }
            get { return _navigateurl; }
        }
        /// <summary>
        /// 新系统链接
        /// </summary>
        public string NewUrl
        {
            set { _newurl = value; }
            get { return _newurl; }
        }
        /// <summary>
        /// 图标地址
        /// </summary>
        public string ImageUrl
        {
            set { _imageurl = value; }
            get { return _imageurl; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 顺序
        /// </summary>
        public int? SortIndex
        {
            set { _sortindex = value; }
            get { return _sortindex; }
        }
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentID
        {
            set { _parentid = value; }
            get { return _parentid; }
        }
        /// <summary>
        /// 浏览权限ID
        /// </summary>
        public int ViewPowerID
        {
            set { _viewpowerid = value; }
            get { return _viewpowerid; }
        }
        #endregion Model
    }

    public class Menu
    {
     
        public int ID { get; set; }
        public string Name { get; set; }
        public string NewIcon { get; set; }
        public string NewIconPre { get; set; }
        public string NavigateUrl { get; set; }
        public int ParentID { get; set; }
        public List<Menu> Data { get; set; }
    }


    public class Refresh
    {
        
        public int type{get;set;}
        public int id { get; set; }
        public string name { get; set; }
        public string icons { get; set; }
        public string[] icon {get;set;}
        public string path { get; set; }
        public int parentID { get; set; }
        public List<Refresh> data { get; set; }
    }



}
//     public class MenuList
//     {
//         public string Name { get; set; }
//         public List<Menu> Parent { get; set; }
//         public List<Menu> Child { get; set; }
//     }
// }