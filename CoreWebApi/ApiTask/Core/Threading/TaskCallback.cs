
namespace API.Core.Threading
{
	public delegate bool TaskCallback();
	public delegate bool TaskCallback<T>(T item);
}
