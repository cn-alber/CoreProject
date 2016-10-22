using System;

namespace API.Core.Threading
{
	public static class TaskManager
	{
		public const int DefaultTimeout = 600;

		private static TaskPool pool;

		public static event TaskAction<ThreadItem<TaskItem>> TaskTimeout;

		public static bool Stoped
		{
			get
			{
				return TaskManager.pool.Stoped;
			}
		}

		public static long TotalTasks
		{
			get
			{
				return TaskManager.pool.TotalTasks;
			}
		}

		public static long WaitingTasks
		{
			get
			{
				return TaskManager.pool.WaitingTasks;
			}
		}

		public static long WorkingTasks
		{
			get
			{
				return TaskManager.pool.WorkingTasks;
			}
		}

		public static long WorkedTasks
		{
			get
			{
				return TaskManager.pool.WorkedTasks;
			}
		}

		public static int CurrentThreads
		{
			get
			{
				return TaskManager.pool.CurrentThreads;
			}
		}

		public static int MinThreads
		{
			get
			{
				return TaskManager.pool.MinThreads;
			}
			set
			{
				if (!TaskManager.pool.Stoped)
				{
					TaskManager.pool.MinThreads = value;
				}
			}
		}

		public static int MaxThreads
		{
			get
			{
				return TaskManager.pool.MaxThreads;
			}
			set
			{
				if (!TaskManager.pool.Stoped)
				{
					TaskManager.pool.MaxThreads = value;
				}
			}
		}

		static TaskManager()
		{
			int num = Environment.ProcessorCount * 4;
			int max = num * 4;
			TaskManager.pool = new TaskPool(max, num);
			TaskManager.pool.TaskTimeout += new TaskAction<ThreadItem<TaskItem>>(TaskManager.DoTimeout);
		}

		public static void Add(ITasking task, int waitTime = 0, int interval = -1, int timeout = 600)
		{
			TaskManager.pool.Add(task, waitTime, interval, timeout);
		}

		public static void Add(ITasking task, object state, int waitTime = 0, int interval = -1, int timeout = 600)
		{
			TaskManager.pool.Add(task, state, waitTime, interval, timeout);
		}

		public static void Add(TaskAction action, int waitTime = 0, int interval = -1, int timeout = 600)
		{
			TaskManager.pool.Add(action, waitTime, interval, timeout);
		}

		public static void Add<T>(TaskAction<T> action, T state, int waitTime = 0, int interval = -1, int timeout = 600)
		{
			TaskManager.pool.Add<T>(action, state, waitTime, interval, timeout);
		}

		public static void Stop()
		{
			TaskManager.pool.Stop();
		}

		private static void DoTimeout(ThreadItem<TaskItem> item)
		{
			if (TaskManager.TaskTimeout != null)
			{
				TaskManager.TaskTimeout(item);
			}
		}
	}
}
