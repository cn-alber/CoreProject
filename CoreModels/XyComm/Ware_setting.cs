namespace CoreModels.XyComm
{
    public class ware_setting{
       #region Model
		private int _id;
		private int? _coid;
		private bool _locksku;
		private bool _ispositionaccurate;
		private bool _synchrosku;
		private int? _isbeyondcount;
		private bool _orderstore;
		private int? _pickingmethod;
		private bool _goodsuniquecode;
		private int? _codepre;
		private bool _singlegoods;
		private int? _intervalchar;
		private bool _sendonpicking;
		private int? _cabinetheight;
		private int? _cabinetnum;
		private bool _limitsender;
		private int? _isgoodsrule;
		private bool _segmentpicking;
		private bool _autolossc;
		private bool _autodelivery;
		private bool _mixedpicking;
		private bool _pickingnominus;
		private int? _reducestock;
		private int? _locktime;
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? CoID
		{
			set{ _coid=value;}
			get{return _coid;}
		}
		/// <summary>
		/// 特殊订单锁定库存 1：锁定 0：不锁定
		/// </summary>
		public bool LockSku
		{
			set{ _locksku=value;}
			get{return _locksku;}
		}
		/// <summary>
		/// 仓位精确库存
		/// </summary>
		public bool IsPositionAccurate
		{
			set{ _ispositionaccurate=value;}
			get{return _ispositionaccurate;}
		}
		/// <summary>
		/// 从商品维护导入商品信息时，默认禁止同步库存
		/// </summary>
		public bool SynchroSku
		{
			set{ _synchrosku=value;}
			get{return _synchrosku;}
		}
		/// <summary>
		/// 采购入库超入处理：1允许入库 2不允许 3 预警提示
		/// </summary>
		public int? IsBeyondCount
		{
			set{ _isbeyondcount=value;}
			get{return _isbeyondcount;}
		}
		/// <summary>
		/// 允许直接登记采购入库单并审核入库
		/// </summary>
		public bool OrderStore
		{
			set{ _orderstore=value;}
			get{return _orderstore;}
		}
		/// <summary>
		/// 拣货方式 "1" 手执拣货  "2" 纸质拣货
		/// </summary>
		public int? PickingMethod
		{
			set{ _pickingmethod=value;}
			get{return _pickingmethod;}
		}
		/// <summary>
		/// 商品唯一码
		/// </summary>
		public bool GoodsUniqueCode
		{
			set{ _goodsuniquecode=value;}
			get{return _goodsuniquecode;}
		}
		/// <summary>
		/// 唯一码前缀 1 商品编码 2 重新数字编码
		/// </summary>
		public int? CodePre
		{
			set{ _codepre=value;}
			get{return _codepre;}
		}
		/// <summary>
		/// 一单一货
		/// </summary>
		public bool SingleGoods
		{
			set{ _singlegoods=value;}
			get{return _singlegoods;}
		}
		/// <summary>
		/// 商品编码间隔符 1 - 2 /  3 .  4 无
		/// </summary>
		public int? IntervalChar
		{
			set{ _intervalchar=value;}
			get{return _intervalchar;}
		}
		/// <summary>
		/// 手执拣货一单多货，边捡边播
		/// </summary>
		public bool SendOnPicking
		{
			set{ _sendonpicking=value;}
			get{return _sendonpicking;}
		}
		/// <summary>
		/// 播种柜层高
		/// </summary>
		public int? CabinetHeight
		{
			set{ _cabinetheight=value;}
			get{return _cabinetheight;}
		}
		/// <summary>
		/// 播种柜总格
		/// </summary>
		public int? CabinetNum
		{
			set{ _cabinetnum=value;}
			get{return _cabinetnum;}
		}
		/// <summary>
		/// 限定由拣货人员播种
		/// </summary>
		public bool LimitSender
		{
			set{ _limitsender=value;}
			get{return _limitsender;}
		}
		/// <summary>
		/// 仓位货物置放规则 1 一仓多货 2 一仓一货
		/// </summary>
		public int? IsGoodsRule
		{
			set{ _isgoodsrule=value;}
			get{return _isgoodsrule;}
		}
		/// <summary>
		/// 分段拣货 0未开通 1 开通
		/// </summary>
		public bool SegmentPicking
		{
			set{ _segmentpicking=value;}
			get{return _segmentpicking;}
		}
		/// <summary>
		/// 零拣区找不到商品时自动盘亏当前仓位
		/// </summary>
		public bool AutoLossc
		{
			set{ _autolossc=value;}
			get{return _autolossc;}
		}
		/// <summary>
		/// 大单拣货成时自动出库 0 关闭 1开通
		/// </summary>
		public bool AutoDelivery
		{
			set{ _autodelivery=value;}
			get{return _autodelivery;}
		}
		/// <summary>
		/// 混合拣货 0 关闭 1开通
		/// </summary>
		public bool MixedPicking
		{
			set{ _mixedpicking=value;}
			get{return _mixedpicking;}
		}
		/// <summary>
		/// 拣货暂存位禁止负库存
		/// </summary>
		public bool PickingNoMinus
		{
			set{ _pickingnominus=value;}
			get{return _pickingnominus;}
		}
		/// <summary>
		/// 打单界面模块 - 直接发货：减库存方式 1:减拣货暂存位库存  2: 减仓位库存
		/// </summary>
		public int? ReduceStock
		{
			set{ _reducestock=value;}
			get{return _reducestock;}
		}
		/// <summary>
		/// 一单一货连打发货锁定订单时间（秒）
		/// </summary>
		public int? LockTime
		{
			set{ _locktime=value;}
			get{return _locktime;}
		}
		#endregion Model


    }



}