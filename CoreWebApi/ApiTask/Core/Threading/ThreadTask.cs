namespace API.Core.Threading
{
	internal class ThreadTask<T>
	{
		public TaskAction<T> Callback
		{
			get;
			set;
		}

		public T Context
		{
			get;
			set;
		}

		public int Timeout
		{
			get;
			set;
		}
	}
}
