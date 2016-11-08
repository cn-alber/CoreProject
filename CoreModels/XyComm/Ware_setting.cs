namespace CoreModels.XyComm
{
    public class ware_setting
    {
        #region Model
		private int _id;
		private int? _coid;
		private int? _locksku=0;
		private int? _ispositionaccurate=0;
		private int? _synchrosku=0;
		private int? _isbeyondcount=1;
		private int? _orderstore=0;
		private int? _pickingmethod=1;
		private int? _goodsuniquecode=0;
		private int? _codepre=1;
		private int? _singlegoods=0;
		private int? _intervalchar=4;
		private int? _sendonpicking=0;
		private int? _pickedautosend=0;
		private int? _cabinetheight=3;
		private int? _cabinetcolumn=0;
		private int? _cabinetnum=0;
		private int? _limitsender=0;
		private int? _sendusecount;
		private int? _isgoodsrule=2;
		private int? _segmentpicking=0;
		private int? _autolossc=0;
		private int? _autodelivery=0;
		private int? _mixedpicking=0;
		private int? _pickingnominus=0;
		private int? _reducestock=1;
		private int? _locktime=0;
		private int? _onemoreprint=0;
		private int? _onemoreonlyex=0;
		private bool _ismain= false;
		private bool _isfen= false;
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
		public int? LockSku
		{
			set{ _locksku=value;}
			get{return _locksku;}
		}
		/// <summary>
		/// 仓位精确库存
		/// </summary>
		public int? IsPositionAccurate
		{
			set{ _ispositionaccurate=value;}
			get{return _ispositionaccurate;}
		}
		/// <summary>
		/// 从商品维护导入商品信息时，默认禁止同步库存
		/// </summary>
		public int? SynchroSku
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
		public int? OrderStore
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
		public int? GoodsUniqueCode
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
		public int? SingleGoods
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
		public int? SendOnPicking
		{
			set{ _sendonpicking=value;}
			get{return _sendonpicking;}
		}
		/// <summary>
		/// 边拣边播，拣货完成自动出库
		/// </summary>
		public int? PickedAutoSend
		{
			set{ _pickedautosend=value;}
			get{return _pickedautosend;}
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
		/// 播种柜列数
		/// </summary>
		public int? CabinetColumn
		{
			set{ _cabinetcolumn=value;}
			get{return _cabinetcolumn;}
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
		public int? LimitSender
		{
			set{ _limitsender=value;}
			get{return _limitsender;}
		}
		/// <summary>
		/// 播种时时候需要输入数量 1：逐一扫描  2：输入数量
		/// </summary>
		public int? SendUseCount
		{
			set{ _sendusecount=value;}
			get{return _sendusecount;}
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
		public int? SegmentPicking
		{
			set{ _segmentpicking=value;}
			get{return _segmentpicking;}
		}
		/// <summary>
		/// 零拣区找不到商品时自动盘亏当前仓位
		/// </summary>
		public int? AutoLossc
		{
			set{ _autolossc=value;}
			get{return _autolossc;}
		}
		/// <summary>
		/// 大单拣货成时自动出库 0 关闭 1开通
		/// </summary>
		public int? AutoDelivery
		{
			set{ _autodelivery=value;}
			get{return _autodelivery;}
		}
		/// <summary>
		/// 混合拣货 0 关闭 1开通
		/// </summary>
		public int? MixedPicking
		{
			set{ _mixedpicking=value;}
			get{return _mixedpicking;}
		}
		/// <summary>
		/// 拣货暂存位禁止负库存
		/// </summary>
		public int? PickingNoMinus
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
		/// <summary>
		/// 一单多货打印拣货单时同时打印小订单
		/// </summary>
		public int? OneMorePrint
		{
			set{ _onemoreprint=value;}
			get{return _onemoreprint;}
		}
		/// <summary>
		/// 一单多货验货只需要扫描快递单号
		/// </summary>
		public int? OneMoreOnlyEx
		{
			set{ _onemoreonlyex=value;}
			get{return _onemoreonlyex;}
		}
		/// <summary>
		/// 是否是主仓
		/// </summary>
		public bool IsMain
		{
			set{ _ismain=value;}
			get{return _ismain;}
		}
		/// <summary>
		/// 是否是分仓
		/// </summary>
		public bool IsFen
		{
			set{ _isfen=value;}
			get{return _isfen;}
		}
		#endregion Model

    }

    public class ware_m_setting
    {
        public int ID
        {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? CoID
        {
            get; set;
        }
        /// <summary>
        /// 特殊订单锁定库存 1：锁定 0：不锁定
        /// </summary>
        public string LockSku
        {
            get; set;
        }
        /// <summary>
        /// 仓位精确库存
        /// </summary>
        public string IsPositionAccurate
        {
            get; set;
        }
        /// <summary>
        /// 从商品维护导入商品信息时，默认禁止同步库存
        /// </summary>
        public string SynchroSku
        {
            get; set;
        }
        /// <summary>
        /// 采购入库超入处理：1允许入库 2不允许 3 预警提示
        /// </summary>
        public string IsBeyondCount
        {
            get; set;
        }
        /// <summary>
        /// 允许直接登记采购入库单并审核入库
        /// </summary>
        public string OrderStore
        {
            get; set;
        }
        /// <summary>
        /// 拣货方式 "1" 手执拣货  "2" 纸质拣货
        /// </summary>
        public string PickingMethod
        {
            get; set;
        }
        /// <summary>
        /// 商品唯一码
        /// </summary>
        public string GoodsUniqueCode
        {
            get; set;
        }
        /// <summary>
        /// 唯一码前缀 1 商品编码 2 重新数字编码
        /// </summary>
        public string CodePre
        {
            get; set;
        }
        /// <summary>
        /// 一单一货
        /// </summary>
        public string SingleGoods
        {
            get; set;
        }
        /// <summary>
        /// 商品编码间隔符 1 - 2 /  3 .  4 无
        /// </summary>
        public string IntervalChar
        {
            get; set;
        }
        /// <summary>
        /// 手执拣货一单多货，边捡边播
        /// </summary>
        public string SendOnPicking
        {
            get; set;
        }
        /// <summary>
        /// 边拣边播，拣货完成自动出库
        /// </summary>
        public string PickedAutoSend
        {
            get; set;
        }
        /// <summary>
        /// 播种柜层高
        /// </summary>
        public int CabinetHeight
        {
            get; set;
        }
        /// <summary>
        /// 播种柜总格
        /// </summary>
        public int CabinetNum
        {
            get; set;
        }
		/// <summary>
		/// 播种柜列数
		/// </summary>
		public int CabinetColumn
		{
            get; set;
        }
        /// <summary>
        /// 限定由拣货人员播种
        /// </summary>
        public string LimitSender
        {
            get; set;
        }
        /// <summary>
        /// 播种时时候需要输入数量 1：逐一扫描  2：输入数量
        /// </summary>
        public string SendUseCount
        {
            get; set;
        }
        /// <summary>
        /// 仓位货物置放规则 1 一仓多货 2 一仓一货
        /// </summary>
        public string IsGoodsRule
        {
            get; set;
        }
        /// <summary>
        /// 分段拣货 0未开通 1 开通
        /// </summary>
        public string SegmentPicking
        {
            get; set;
        }
        /// <summary>
        /// 零拣区找不到商品时自动盘亏当前仓位
        /// </summary>
        public string AutoLossc
        {
            get; set;
        }
        /// <summary>
        /// 大单拣货成时自动出库 0 关闭 1开通
        /// </summary>
        public string AutoDelivery
        {
            get; set;
        }
        /// <summary>
        /// 混合拣货 0 关闭 1开通
        /// </summary>
        public string MixedPicking
        {
            get; set;
        }
        /// <summary>
        /// 拣货暂存位禁止负库存
        /// </summary>
        public string PickingNoMinus
        {
            get; set;
        }
        /// <summary>
        /// 打单界面模块 - 直接发货：减库存方式 1:减拣货暂存位库存  2: 减仓位库存
        /// </summary>
        public string ReduceStock
        {
            get; set;
        }
        /// <summary>
        /// 一单一货连打发货锁定订单时间（秒）
        /// </summary>
        public int LockTime
        {
            get; set;
        }
        /// <summary>
        /// 一单多货打印拣货单时同时打印小订单
        /// </summary>
        public string OneMorePrint
        {
            get; set;
        }
        /// <summary>
        /// 一单多货验货只需要扫描快递单号
        /// </summary>
        public string OneMoreOnlyEx
        {
            get; set;
        }
        /// <summary>
        /// 是否是主仓
        /// </summary>
        public bool IsMain
        {
            get; set;
        }
        /// <summary>
        /// 是否是分仓
        /// </summary>
        public bool IsFen
        {
            get; set;
        }

    }


    public class ware_f_setting
    {
        public int ID { get; set; }
        public int CoID { get; set; }
        public string LockSku { get; set; }
        public string IsPositionAccurate { get; set; }
        public string SynchroSku { get; set; }
        public string IsBeyondCount { get; set; }
        public string OrderStore { get; set; }
        public string PickingMethod { get; set; }
        public string SingleGoods { get; set; }
        public string IntervalChar { get; set; }
        public string SendOnPicking { get; set; }
        public string PickedAutoSend { get; set; }
        public int CabinetHeight { get; set; }
        public int CabinetNum { get; set; }
		public int CabinetColumn{get; set;}
        public string LimitSender { get; set; }
        public string SendUseCount { get; set; }
        public string IsGoodsRule { get; set; }
        public string SegmentPicking { get; set; }
        public string AutoLossc { get; set; }
        public string AutoDelivery { get; set; }
        public string MixedPicking { get; set; }
        public string PickingNoMinus { get; set; }
        public string ReduceStock { get; set; }
        public string LockTime { get; set; }
        public string OneMorePrint { get; set; }
        public string OneMoreOnlyEx { get; set; }
        public bool IsMain { get; set; }
        public bool IsFen { get; set; }
    }




}