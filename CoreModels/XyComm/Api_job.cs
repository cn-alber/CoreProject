using System;

namespace CoreModels.XyComm
{
     public class api_job
     {
        #region Model
		private int _job_id;
		private int? _co_id;
		private int? _shop_id;
		private string _api_mode;
		private string _api_key;
		private string _split_id;
		private string _api_type;
		private string _api_group;
		private string _api_name;
		private string _api_author;
		private int? _api_interval;
		private int? _api_timeout;
		private bool _api_running= false;
		private int? _api_lazy;
		private int? _status;
		private bool _enabled= false;
		private DateTime _created;
		private DateTime _modified;
		private long _run_total=0;
		private string _run_host;
		private long? _run_id;
		private DateTime? _run_bof;
		private DateTime? _run_eof;
		private DateTime? _run_next;
		private DateTime? _run_stop;
		private decimal? _run_times=0.000M;
		private int? _run_result;
		private string _run_state;
		private DateTime _run_timestamp;
		private long _err_total=0;
		private int? _err_retry;
		private int? _err_code;
		private string _err_message;
		private DateTime _err_timestamp;
		/// <summary>
		/// 
		/// </summary>
		public int job_id
		{
			set{ _job_id=value;}
			get{return _job_id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? co_id
		{
			set{ _co_id=value;}
			get{return _co_id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? shop_id
		{
			set{ _shop_id=value;}
			get{return _shop_id;}
		}
		/// <summary>
		/// 任务模式（S：shop；R：rds；G：全局）
		/// </summary>
		public string api_mode
		{
			set{ _api_mode=value;}
			get{return _api_mode;}
		}
		/// <summary>
		/// Api方法名
		/// </summary>
		public string api_key
		{
			set{ _api_key=value;}
			get{return _api_key;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string split_id
		{
			set{ _split_id=value;}
			get{return _split_id;}
		}
		/// <summary>
		/// Api站点
		/// </summary>
		public string api_type
		{
			set{ _api_type=value;}
			get{return _api_type;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string api_group
		{
			set{ _api_group=value;}
			get{return _api_group;}
		}
		/// <summary>
		/// Api接口名称
		/// </summary>
		public string api_name
		{
			set{ _api_name=value;}
			get{return _api_name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string api_author
		{
			set{ _api_author=value;}
			get{return _api_author;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? api_interval
		{
			set{ _api_interval=value;}
			get{return _api_interval;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? api_timeout
		{
			set{ _api_timeout=value;}
			get{return _api_timeout;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool api_running
		{
			set{ _api_running=value;}
			get{return _api_running;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? api_lazy
		{
			set{ _api_lazy=value;}
			get{return _api_lazy;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool enabled
		{
			set{ _enabled=value;}
			get{return _enabled;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime created
		{
			set{ _created=value;}
			get{return _created;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime modified
		{
			set{ _modified=value;}
			get{return _modified;}
		}
		/// <summary>
		/// 
		/// </summary>
		public long run_total
		{
			set{ _run_total=value;}
			get{return _run_total;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string run_host
		{
			set{ _run_host=value;}
			get{return _run_host;}
		}
		/// <summary>
		/// 
		/// </summary>
		public long? run_id
		{
			set{ _run_id=value;}
			get{return _run_id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? run_bof
		{
			set{ _run_bof=value;}
			get{return _run_bof;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? run_eof
		{
			set{ _run_eof=value;}
			get{return _run_eof;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? run_next
		{
			set{ _run_next=value;}
			get{return _run_next;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? run_stop
		{
			set{ _run_stop=value;}
			get{return _run_stop;}
		}
		/// <summary>
		/// 运行次数
		/// </summary>
		public decimal? run_times
		{
			set{ _run_times=value;}
			get{return _run_times;}
		}
		/// <summary>
		/// 运行结果
		/// </summary>
		public int? run_result
		{
			set{ _run_result=value;}
			get{return _run_result;}
		}
		/// <summary>
		/// 运行状态
		/// </summary>
		public string run_state
		{
			set{ _run_state=value;}
			get{return _run_state;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime run_timestamp
		{
			set{ _run_timestamp=value;}
			get{return _run_timestamp;}
		}
		/// <summary>
		/// 出错总数
		/// </summary>
		public long err_total
		{
			set{ _err_total=value;}
			get{return _err_total;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? err_retry
		{
			set{ _err_retry=value;}
			get{return _err_retry;}
		}
		/// <summary>
		/// 错误编码
		/// </summary>
		public int? err_code
		{
			set{ _err_code=value;}
			get{return _err_code;}
		}
		/// <summary>
		/// 错误信息
		/// </summary>
		public string err_message
		{
			set{ _err_message=value;}
			get{return _err_message;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime err_timestamp
		{
			set{ _err_timestamp=value;}
			get{return _err_timestamp;}
		}
		#endregion Model

     }

     public class apilog{
         public int job_id{get;set;}
         public bool enabled{get;set;}
         public int shop_id{get;set;}
         public string api_name{get;set;}
         public string api_key{get;set;}
         public decimal api_interval{get;set;}
         public string run_eof{get;set;}
         public decimal run_times{get;set;}
         public decimal total{get;set;}
         public decimal run_total{get;set;}
         public decimal err_total{get;set;}
         public string err_timestamp{get;set;}
         public string err_message{get;set;}

     }


}