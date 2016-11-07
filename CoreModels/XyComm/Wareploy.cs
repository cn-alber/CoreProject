namespace CoreModels.XyComm
{
    public class WarePloy
    {
        #region Model
		private int _id;
		private string _name;
		private int? _level;
		private int? _wid=0;
		private string _wname;
		private string _province;
		private string _shopid;
		private string _did;
		private string _containgoods;
		private string _removegoods;
		private string _containskus;
		private string _removeskus;
		private int? _minnum;
		private int? _maxnum;
		private int? _payment;
		private int? _coid;
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 策略名
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 优先级
		/// </summary>
		public int? Level
		{
			set{ _level=value;}
			get{return _level;}
		}
		/// <summary>
		/// 指定仓库
		/// </summary>
		public int? Wid
		{
			set{ _wid=value;}
			get{return _wid;}
		}
		/// <summary>
		/// 仓库名
		/// </summary>
		public string Wname
		{
			set{ _wname=value;}
			get{return _wname;}
		}
		/// <summary>
		/// 限定省份
		/// </summary>
		public string Province
		{
			set{ _province=value;}
			get{return _province;}
		}
		/// <summary>
		/// 限定店铺
		/// </summary>
		public string Shopid
		{
			set{ _shopid=value;}
			get{return _shopid;}
		}
		/// <summary>
		/// 分销商ID
		/// </summary>
		public string Did
		{
			set{ _did=value;}
			get{return _did;}
		}
		/// <summary>
		/// 包含商品货号
		/// </summary>
		public string ContainGoods
		{
			set{ _containgoods=value;}
			get{return _containgoods;}
		}
		/// <summary>
		/// 排除的商品货号
		/// </summary>
		public string RemoveGoods
		{
			set{ _removegoods=value;}
			get{return _removegoods;}
		}
		/// <summary>
		/// 包含商品编码
		/// </summary>
		public string ContainSkus
		{
			set{ _containskus=value;}
			get{return _containskus;}
		}
		/// <summary>
		/// 排除商品编码
		/// </summary>
		public string RemoveSkus
		{
			set{ _removeskus=value;}
			get{return _removeskus;}
		}
		/// <summary>
		/// 限定商品最小数量
		/// </summary>
		public int? MinNum
		{
			set{ _minnum=value;}
			get{return _minnum;}
		}
		/// <summary>
		/// 限定商品最大数量
		/// </summary>
		public int? MaxNum
		{
			set{ _maxnum=value;}
			get{return _maxnum;}
		}
		/// <summary>
		/// 限定付款方式
		/// </summary>
		public int? Payment
		{
			set{ _payment=value;}
			get{return _payment;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? CoID
		{
			set{ _coid=value;}
			get{return _coid;}
		}
		#endregion Model

        
    }

	public class WarePloyRequest{
		public int ID{get;set;}
        public int CoID{get;set;}
    	public string Name{get;set;} 
        public int Level{get;set;}
    	public int Wid{get;set;}
        public string Wname {get;set;}
        public string[] Province {get;set;}                                   
		public string[]	Shopid {get;set;}
        public string[] Did {get;set;}
        public string[] ContainGoods {get;set;}
        public string[] RemoveGoods {get;set;}
        public string[] ContainSkus {get;set;}
        public string[] RemoveSkus {get;set;}
        public int MinNum {get;set;}
        public int MaxNum {get;set;}
        public int Payment{get;set;}

	}





}