using System;
using System.Threading;

namespace API.Core.Threading
{
	public class TaskItem : IDisposable
	{
		protected Timer Timer;

		private bool disposed;

		public ITasking Task
		{
			get;
			private set;
		}

		public TimerCallback Callback
		{
			get;
			private set;
		}

		public object State
		{
			get;
			private set;
		}

		public int WaitTime
		{
			get;
			private set;
		}

		public int Interval
		{
			get;
			private set;
		}

		public int Timeout
		{
			get;
			private set;
		}

		public bool Repeated
		{
			get;
			private set;
		}

		public TaskItem(ITasking task, TimerCallback callback, object state, int waitTime = 0, int interval = -1, int timeout = -1)
		{
			this.Task = task;
			this.Callback = callback;
			this.State = state;
			this.WaitTime = waitTime;
			this.Interval = interval;
			this.Timeout = timeout;
			this.Repeated = (this.Interval > 0);
			if (this.Repeated || this.WaitTime > 0)
			{
				int dueTime = 0;
				if (waitTime > 0)
				{
					dueTime = waitTime * 1000;
				}
				this.Timer = new Timer(this.Callback, this, dueTime, -1);
				return;
			}
			this.Callback(this);
		}

		public bool Next()
		{
			return this.Repeated && this.Timer != null && this.Timer.Change(this.Interval * 1000, -1);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.Timer != null)
				{
					this.Timer.Dispose();
					this.Timer = null;
				}
				this.disposed = true;
			}
		}

		~TaskItem()
		{
			this.Dispose(false);
		}
	}
}
