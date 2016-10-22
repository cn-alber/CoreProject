using System;
using System.Threading;

namespace API.Core.Threading
{
	public class ThreadItem<T> : IDisposable
	{
		private object _locker = new object();

		private AutoResetEvent _waiter;

		private Timer _timer;

		private Thread _thread;

		private bool _working;

		private bool _running;

		private DateTime _runBof;

		private DateTime _runEof;

		private bool closed;

		private bool disposed;

		public event TaskCallback TaskStarted;

		public event TaskCallback<ThreadItem<T>> TaskCompleted;

		public event TaskAction<ThreadItem<T>> TaskTimeout;

		public T Context
		{
			get;
			private set;
		}

		public TaskAction<T> Callback
		{
			get;
			private set;
		}

		public int Timeout
		{
			get;
			private set;
		}

		public bool Running
		{
			get
			{
				return this._running;
			}
		}

		public double RunTimes
		{
			get
			{
				DateTime runBof = this._runBof;
				DateTime runEof = this._runEof;
				if (this._running || runEof < runBof)
				{
					return Math.Round((DateTime.Now - runBof).TotalSeconds, 3);
				}
				return Math.Round((runEof - runBof).TotalSeconds, 3);
			}
		}

		public ThreadItem()
		{
			this._waiter = new AutoResetEvent(false);
			this._timer = new Timer(new TimerCallback(this.Expire),null,System.Threading.Timeout.Infinite,System.Threading.Timeout.Infinite);
			this._working = true;
			this._running = false;
			this._runEof = DateTime.Now;
			this._runBof = DateTime.Now;
			this._thread = new Thread(new ThreadStart(this.Working));
			this._thread.IsBackground = true;
			this._thread.Start();
		}

		private void SetTask(ThreadTask<T> task)
		{
			this.SetTask(task.Callback, task.Context, task.Timeout);
		}

		private void SetTask(TaskAction<T> callback, T context, int timeout)
		{
			this.Callback = callback;
			this.Context = context;
			this.Timeout = timeout;
			this._running = true;
		}

		public void Start()
		{
			if (this._waiter != null)
			{
				this._waiter.Set();
			}
		}

		internal void Start(ThreadTask<T> task)
		{
			this.SetTask(task);
			if (this._waiter != null)
			{
				this._waiter.Set();
			}
		}

		public void Start(TaskAction<T> callback, T context, int timeout)
		{
			this.SetTask(callback, context, timeout);
			if (this._waiter != null)
			{
				this._waiter.Set();
			}
		}

		private void Working()
		{
			try
			{
				while (this._working)
				{
					if (this._waiter != null)
					{
						this._waiter.WaitOne();
					}
					if (this._working && this.TaskStarted != null && !this.TaskStarted())
					{
						break;
					}
					if (this._running)
					{
						try
						{
							this._runBof = DateTime.Now;
							if (this.Timeout > 0 && this._timer != null)
							{
								this._timer.Change(this.Timeout * 1000, -1);
							}
							this.Callback(this.Context);
							this._runEof = DateTime.Now;
						}
						catch
						{
						}
						if (this.Timeout > 0 && this._timer != null)
						{
							this._timer.Change(-1, -1);
						}
					}
					if (this._running)
					{
						lock (this._locker)
						{
							if (this._running)
							{
								this._running = false;
							}
						}
					}
					if (this._working && this.TaskCompleted != null && !this.TaskCompleted(this))
					{
						break;
					}
				}
				this.Dispose();
			}
			catch
			{
			}
		}

		private void Expire(object state)
		{
			if (this._running)
			{
				lock (this._locker)
				{
					if (this._running)
					{
						this._running = false;
						this._working = false;
						if (this._waiter != null)
						{
							this._waiter.Set();
						}
						if (this._thread != null)
						{
							try
							{
								this._runEof = DateTime.Now;
								if (this._thread.IsAlive)
								{
									//this._thread.Abort();
									this._thread = null;
								}
							}
							catch
							{
							}
							this._thread = null;
						}
						if (this.TaskTimeout != null)
						{
							try
							{
								this.TaskTimeout(this);
							}
							catch
							{
							}
						}
						this.Dispose();
					}
				}
			}
		}

		public void Stop()
		{
			lock (this._locker)
			{
				this._working = false;
				this._running = false;
			}
			if (this._waiter != null)
			{
				this._waiter.Set();
			}
		}

		private void Close()
		{
			if (!this.closed)
			{
				this._working = false;
				this._running = false;
				if (this._timer != null)
				{
					this._timer.Dispose();
					this._timer = null;
				}
				if (this._waiter != null)
				{
					//this._waiter.Close();
					this._waiter = null;
				}
			}
			this.closed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.Close();
				if (this._thread != null)
				{
					try
					{
						if (this._thread.IsAlive)
						{
							//this._thread.Abort();
							this._thread = null;
						}
					}
					catch
					{
					}
					this._thread = null;
				}
			}
			this.disposed = true;
		}

		~ThreadItem()
		{
			this.Dispose(false);
		}
	}
}
