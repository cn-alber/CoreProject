namespace API.Core.Threading
{
	public delegate void TaskAction();
	public delegate void TaskAction<T>(T item);
}
