namespace API.Core.Threading
{
	internal class TaskWrapper<T> : ITasking
	{
		private TaskAction<T> _action;

		private T _state;

		public TaskWrapper(TaskAction action)
		{
			this._action = delegate(T s)
			{
				action();
			};
			this._state = default(T);
		}

		public TaskWrapper(TaskAction<T> action, T state)
		{
			this._action = action;
			this._state = state;
		}

		public void Execute()
		{
			this._action(this._state);
		}
	}
}
