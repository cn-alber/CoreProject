using System;
using System.Threading;

namespace API.Core.Threading
{
	public class TaskPool : IDisposable
	{
		private ThreadQueue<TaskItem> taskQueue;

		private long totalTasks;

		private bool disposed;

		public event TaskAction<ThreadItem<TaskItem>> TaskTimeout;

		public bool Stoped
		{
			get;
			private set;
		}

		public long TotalTasks
		{
			get
			{
				if (!this.Stoped)
				{
					return Interlocked.Read(ref this.totalTasks);
				}
				return 0L;
			}
		}

		public long WaitingTasks
		{
			get
			{
				if (!this.Stoped)
				{
					return this.taskQueue.WaitingTasks;
				}
				return 0L;
			}
		}

		public long WorkingTasks
		{
			get
			{
				if (!this.Stoped)
				{
					return this.taskQueue.WorkingTasks;
				}
				return 0L;
			}
		}

		public long WorkedTasks
		{
			get
			{
				if (!this.Stoped)
				{
					return this.taskQueue.WorkedTasks;
				}
				return 0L;
			}
		}

		public int CurrentThreads
		{
			get
			{
				if (!this.Stoped)
				{
					return this.taskQueue.CurrentThreads;
				}
				return -1;
			}
		}

		public int MinThreads
		{
			get
			{
				if (!this.Stoped)
				{
					return this.taskQueue.MinThreads;
				}
				return -1;
			}
			set
			{
				if (!this.Stoped)
				{
					this.taskQueue.MinThreads = value;
				}
			}
		}

		public int MaxThreads
		{
			get
			{
				if (!this.Stoped)
				{
					return this.taskQueue.MaxThreads;
				}
				return -1;
			}
			set
			{
				if (!this.Stoped)
				{
					this.taskQueue.MaxThreads = value;
				}
			}
		}

		public TaskPool()
		{
			int num = Environment.ProcessorCount * 4;
			int maxThread = num * 4;
			this.taskQueue = new ThreadQueue<TaskItem>(maxThread, num);
			this.taskQueue.TaskTimeout += new TaskAction<ThreadItem<TaskItem>>(this.DoTimeout);
		}

		public TaskPool(int max, int min)
		{
			this.taskQueue = new ThreadQueue<TaskItem>(max, min);
			this.taskQueue.TaskTimeout += new TaskAction<ThreadItem<TaskItem>>(this.DoTimeout);
		}

		public void Add(ITasking task, int waitTime = 0, int interval = -1, int timeout = 600)
		{
			if (!this.Stoped)
			{
				new TaskItem(task, new TimerCallback(this.DoCallback), null, waitTime, interval, timeout);
				Interlocked.Increment(ref this.totalTasks);
			}
		}

		public void Add(ITasking task, object state, int waitTime = 0, int interval = -1, int timeout = 600)
		{
			if (!this.Stoped)
			{
				new TaskItem(task, new TimerCallback(this.DoCallback), state, waitTime, interval, timeout);
				Interlocked.Increment(ref this.totalTasks);
			}
		}

		public void Add(TaskAction action, int waitTime = 0, int interval = -1, int timeout = 600)
		{
			if (!this.Stoped)
			{
				new TaskItem(new TaskWrapper<object>(action), new TimerCallback(this.DoCallback), null, waitTime, interval, timeout);
				Interlocked.Increment(ref this.totalTasks);
			}
		}

		public void Add<T>(TaskAction<T> action, T state, int waitTime = 0, int interval = -1, int timeout = 600)
		{
			if (!this.Stoped)
			{
				new TaskItem(new TaskWrapper<T>(action, state), new TimerCallback(this.DoCallback), state, waitTime, interval, timeout);
				Interlocked.Increment(ref this.totalTasks);
			}
		}

		public void Stop()
		{
			if (this.Stoped)
			{
				throw new Exception("TaskPool is stoped.");
			}
			this.Stoped = true;
			if (this.taskQueue != null)
			{
				//this.taskQueue.Dispose();
				this.taskQueue = null;
			}
		}

		private void DoCallback(object state)
		{
			if (!this.Stoped)
			{
				TaskItem taskItem = state as TaskItem;
				if (taskItem != null)
				{
					this.taskQueue.AddTaskAsync(new TaskAction<TaskItem>(this.DoExecute), taskItem, taskItem.Timeout);
				}
			}
		}

		private void DoExecute(TaskItem item)
		{
			if (!this.Stoped && item != null)
			{
				try
				{
					item.Task.Execute();
				}
				finally
				{
					if (item.Repeated)
					{
						item.Next();
					}
				}
			}
		}

		private void DoTimeout(ThreadItem<TaskItem> item)
		{
			if (this.TaskTimeout != null)
			{
				this.TaskTimeout(item);
			}
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
				this.Stoped = true;
				if (this.taskQueue != null)
				{
					this.taskQueue.Dispose();
					this.taskQueue = null;
				}
			}
			this.disposed = true;
		}

		~TaskPool()
		{
			this.Dispose(false);
		}
	}
}
