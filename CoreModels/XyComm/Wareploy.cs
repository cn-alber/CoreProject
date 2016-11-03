namespace CoreModels.XyComm
{
    public class WarePloy
    {
        #region Model
		private int _id;
		private string _name;
		private int? _level;
		private int? _wid;
		private int? _province;
		private int? _shopid;
		private int? _did;
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
		/// 限定省份
		/// </summary>
		public int? Province
		{
			set{ _province=value;}
			get{return _province;}
		}
		/// <summary>
		/// 限定店铺
		/// </summary>
		public int? Shopid
		{
			set{ _shopid=value;}
			get{return _shopid;}
		}
		/// <summary>
		/// 分销商ID
		/// </summary>
		public int? Did
		{
			set{ _did=value;}
			get{return _did;}
		}
		/// <summary>
		/// 包含商品编码
		/// </summary>
		public string ContainGoods
		{
			set{ _containgoods=value;}
			get{return _containgoods;}
		}
		/// <summary>
		/// 排除的商品编码
		/// </summary>
		public string RemoveGoods
		{
			set{ _removegoods=value;}
			get{return _removegoods;}
		}
		/// <summary>
		/// 包含商品的款式编码
		/// </summary>
		public string ContainSkus
		{
			set{ _containskus=value;}
			get{return _containskus;}
		}
		/// <summary>
		/// 排除商品的款式编码
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


}