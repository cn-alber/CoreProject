using System;
using System.Collections.Generic;

namespace CoreModels.XyCore{

    public class Wmspile{
        #region Model
		private int _id;
		private string _pcode;
		private int? _skuautoid;
		private string _skuid;
		private int _warehouseid;
		private string _warehousename;
		private int _type=0;
		private int _pctype=0;
		private int _order;
		private int _qty=0;
		private int _lockqty=0;
		private bool _enable= false;
		private string _creator;
		private DateTime? _createdate;
		private int _coid;
		private int _maxqty=0;
		private string _pcodec;
		private int _whidc=0;
		private string _whnamec;
		private string _area;
		private string _row;
		private string _col;
		private string _storey;
		private string _cell;
		private bool _isdelete= false;
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 库位编码
		/// </summary>
		public string PCode
		{
			set{ _pcode=value;}
			get{return _pcode;}
		}
		/// <summary>
		/// sku自增id
		/// </summary>
		public int? Skuautoid
		{
			set{ _skuautoid=value;}
			get{return _skuautoid;}
		}
		/// <summary>
		/// 商家编码
		/// </summary>
		public string SkuID
		{
			set{ _skuid=value;}
			get{return _skuid;}
		}
		/// <summary>
		/// 仓库编号
		/// </summary>
		public int WarehouseID
		{
			set{ _warehouseid=value;}
			get{return _warehouseid;}
		}
		/// <summary>
		/// 仓库名称
		/// </summary>
		public string WarehouseName
		{
			set{ _warehousename=value;}
			get{return _warehousename;}
		}
		/// <summary>
		/// 仓库类型
		/// </summary>
		public int Type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// 库位类型
		/// </summary>
		public int PCType
		{
			set{ _pctype=value;}
			get{return _pctype;}
		}
		/// <summary>
		/// 顺序
		/// </summary>
		public int Order
		{
			set{ _order=value;}
			get{return _order;}
		}
		/// <summary>
		/// 数量
		/// </summary>
		public int Qty
		{
			set{ _qty=value;}
			get{return _qty;}
		}
		/// <summary>
		/// 锁定库存
		/// </summary>
		public int lockqty
		{
			set{ _lockqty=value;}
			get{return _lockqty;}
		}
		/// <summary>
		/// 是否启用
		/// </summary>
		public bool Enable
		{
			set{ _enable=value;}
			get{return _enable;}
		}
		/// <summary>
		/// 创建人
		/// </summary>
		public string Creator
		{
			set{ _creator=value;}
			get{return _creator;}
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime? CreateDate
		{
			set{ _createdate=value;}
			get{return _createdate;}
		}
		/// <summary>
		/// 公司ID
		/// </summary>
		public int CoID
		{
			set{ _coid=value;}
			get{return _coid;}
		}
		/// <summary>
		/// 库位最大库存
		/// </summary>
		public int maxqty
		{
			set{ _maxqty=value;}
			get{return _maxqty;}
		}
		/// <summary>
		/// 库位编码(出)
		/// </summary>
		public string PCodeC
		{
			set{ _pcodec=value;}
			get{return _pcodec;}
		}
		/// <summary>
		/// 仓库编号(出)
		/// </summary>
		public int WhIDC
		{
			set{ _whidc=value;}
			get{return _whidc;}
		}
		/// <summary>
		/// 仓库名称(出)
		/// </summary>
		public string WhNameC
		{
			set{ _whnamec=value;}
			get{return _whnamec;}
		}
		/// <summary>
		/// 区域
		/// </summary>
		public string Area
		{
			set{ _area=value;}
			get{return _area;}
		}
		/// <summary>
		/// 仓位-行
		/// </summary>
		public string Row
		{
			set{ _row=value;}
			get{return _row;}
		}
		/// <summary>
		/// 仓位-列
		/// </summary>
		public string Col
		{
			set{ _col=value;}
			get{return _col;}
		}
		/// <summary>
		/// 仓位-层
		/// </summary>
		public string Storey
		{
			set{ _storey=value;}
			get{return _storey;}
		}
		/// <summary>
		/// 仓位-格
		/// </summary>
		public string Cell
		{
			set{ _cell=value;}
			get{return _cell;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsDelete
		{
			set{ _isdelete=value;}
			get{return _isdelete;}
		}
		#endregion Model

    }

	public class Pilelist{
		public int ID{get;set;}
		public string PCode{get;set;}
		public bool Enable{get;set;}
		public string SkuID{get;set;}
		public string Skuautoid{get;set;}
		public string Area{get;set;}
		public string Row{get;set;}
		public string Col{get;set;}
		public string Storey{get;set;}
		public string Cell{get;set;}
	}
	public class Sub{
		public string parent{get;set;}
		public List<Sub> children{get;set;}
	}


	public class PileInsert{
		public int WarehouseID{get;set;}
		public string WarehouseName{get;set;} 
		public int Type{get;set;}
		public string area{get;set;}
		public string[] row{get;set;}
		public string[] col{get;set;}
		public string[] storey{get;set;}
		public string[] cell{get;set;}
	}



}