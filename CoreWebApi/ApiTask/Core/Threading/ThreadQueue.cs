using System;
using System.Collections.Concurrent;
using System.Threading;

namespace API.Core.Threading
{
	public class ThreadQueue<T> : IDisposable
	{
		private ConcurrentQueue<ThreadTask<T>> waitingItems;

		private ConcurrentQueue<ThreadItem<T>> threadItems;

		private AutoResetEvent waitHandler;

		private Thread waitThread;

		private int _maxThreads;

		private int _minThreads;

		private int _currentThreads;

		private long _workingTasks;

		private long _workedTasks;

		private bool disposed;

		public event TaskAction<ThreadItem<T>> TaskTimeout;

		public int MaxThreads
		{
			get
			{
				return this._maxThreads;
			}
			set
			{
				this._maxThreads = value;
				if (this._maxThreads < this._minThreads)
				{
					this._minThreads = this._maxThreads;
				}
			}
		}

		public int MinThreads
		{
			get
			{
				return this._minThreads;
			}
			set
			{
				this._minThreads = value;
				if (this._minThreads > this._maxThreads)
				{
					this._maxThreads = this._minThreads;
				}
			}
		}

		public int CurrentThreads
		{
			get
			{
				return this._currentThreads;
			}
		}

		public long WaitingTasks
		{
			get
			{
				return (long)this.waitingItems.Count;
			}
		}

		public long WorkingTasks
		{
			get
			{
				return Interlocked.Read(ref this._workingTasks);
			}
		}

		public long WorkedTasks
		{
			get
			{
				return Interlocked.Read(ref this._workedTasks);
			}
		}

		public ThreadQueue() : this(20, 4)
		{
		}

		public ThreadQueue(int maxThread) : this(maxThread, 4)
		{
		}

		public ThreadQueue(int maxThread, int minThread)
		{
			if (maxThread < 1)
			{
				throw new ArgumentOutOfRangeException("maxthread");
			}
			if (minThread < 1)
			{
				throw new ArgumentOutOfRangeException("minthread");
			}
			if (minThread < maxThread)
			{
				this._maxThreads = maxThread;
				this._minThreads = minThread;
			}
			else
			{
				this._maxThreads = maxThread;
				this._minThreads = maxThread;
			}
			this.waitingItems = new ConcurrentQueue<ThreadTask<T>>();
			this.threadItems = new ConcurrentQueue<ThreadItem<T>>();
			this.waitHandler = new AutoResetEvent(true);
			this.waitThread = new Thread(new ThreadStart(this.DoWaiting));
			this.waitThread.IsBackground = true;
			this.waitThread.Start();
		}

		private ThreadItem<T> GetNewThread()
		{
			ThreadItem<T> threadItem = new ThreadItem<T>();
			threadItem.TaskStarted += new TaskCallback(this.DoStarted);
			threadItem.TaskCompleted += new TaskCallback<ThreadItem<T>>(this.DoCompleted);
			threadItem.TaskTimeout += new TaskAction<ThreadItem<T>>(this.DoTimeout);
			return threadItem;
		}

		private ThreadItem<T> GetFreeThread()
		{
			ThreadItem<T> result = null;
			if (!this.threadItems.TryDequeue(out result) && this._currentThreads < this.MaxThreads)
			{
				if (Interlocked.Increment(ref this._currentThreads) > this.MaxThreads)
				{
					Interlocked.Decrement(ref this._currentThreads);
				}
				else
				{
					result = this.GetNewThread();
				}
			}
			return result;
		}

		private ThreadTask<T> GetWaitingTask()
		{
			ThreadTask<T> result = null;
			this.waitingItems.TryDequeue(out result);
			return result;
		}

		private void DoWaiting()
		{
			try
			{
				while (true)
				{
					if (this._currentThreads < this.MaxThreads)
					{
						ThreadTask<T> waitingTask = this.GetWaitingTask();
						if (waitingTask != null)
						{
							Interlocked.Increment(ref this._currentThreads);
							ThreadItem<T> newThread = this.GetNewThread();
							newThread.Start(waitingTask);
						}
					}
					Thread.Sleep(100);
				}
			}
			catch
			{
			}
		}

		private bool DoStarted()
		{
			Interlocked.Increment(ref this._workingTasks);
			return true;
		}

		private bool DoCompleted(ThreadItem<T> item)
		{
			Interlocked.Decrement(ref this._workingTasks);
			Interlocked.Increment(ref this._workedTasks);
			ThreadTask<T> waitingTask = this.GetWaitingTask();
			if (waitingTask != null)
			{
				item.Start(waitingTask);
				return true;
			}
			try
			{
				if (this.CurrentThreads > this.MinThreads)
				{
					Interlocked.Decrement(ref this._currentThreads);
					item.Stop();
					bool result = false;
					return result;
				}
				if (this.threadItems != null)
				{
					this.threadItems.Enqueue(item);
					bool result = true;
					return result;
				}
			}
			finally
			{
				if (this.WorkingTasks <= 0L)
				{
					this.waitHandler.Set();
				}
			}
			return false;
		}

		private void DoTimeout(ThreadItem<T> item)
		{
			Interlocked.Decrement(ref this._workingTasks);
			Interlocked.Decrement(ref this._currentThreads);
			if (this.TaskTimeout != null)
			{
				this.TaskTimeout(item);
			}
		}

		public void AddTaskAsync(TaskAction callback, int timeout = -1)
		{
			TaskAction<T> callback2 = delegate(T o)
			{
				callback();
			};
			this.AddTaskAsync(callback2, default(T), timeout);
		}

		public void AddTaskAsync(TaskAction<T> callback, T context, int timeout = -1)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("ThreadQueue");
			}
			this.waitHandler.Reset();
			ThreadItem<T> freeThread = this.GetFreeThread();
			if (freeThread != null)
			{
				freeThread.Start(callback, context, timeout);
				return;
			}
			ThreadTask<T> threadTask = new ThreadTask<T>();
			threadTask.Callback = callback;
			threadTask.Context = context;
			threadTask.Timeout = timeout;
			this.waitingItems.Enqueue(threadTask);
		}

		public bool WaitAsync()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("ThreadQueue");
			}
			bool result = this.waitHandler.WaitOne();
			this.waitHandler.Set();
			return result;
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
				if (this.waitThread != null)
				{
					// try
					// {
					// 	this.waitThread.Abort();
					// }
					// catch
					// {
					// }
					this.waitThread = null;
				}
				if (this.threadItems != null)
				{
					while (this.threadItems.Count > 0)
					{
						ThreadItem<T> threadItem = null;
						if (this.threadItems.TryDequeue(out threadItem))
						{
							threadItem.Stop();
							threadItem = null;
						}
					}
					this.threadItems = null;
				}
				Interlocked.Exchange(ref this._currentThreads, 0);
				if (this.waitHandler != null)
				{
					//this.waitHandler.Close();
					this.waitHandler = null;
				}
			}
			this.disposed = true;
		}

		~ThreadQueue()
		{
			this.Dispose(false);
		}
	}
	public class ThreadQueue : ThreadQueue<object>
	{
		public ThreadQueue() : base(20, 4)
		{
		}

		public ThreadQueue(int maxThreads) : base(maxThreads, 4)
		{
		}

		public ThreadQueue(int maxThreads, int minThreads) : base(maxThreads, minThreads)
		{
		}
	}
}
