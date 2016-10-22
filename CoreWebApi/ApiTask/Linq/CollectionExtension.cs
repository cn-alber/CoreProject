using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtension
{
	public static bool IsNullOrEmpty(this ICollection collection)
	{
		return collection == null || collection.Count == 0;
	}

	public static bool IsNullOrEmpty(this IEnumerable enumerable)
	{
		if (enumerable == null)
		{
			return true;
		}
        IEnumerator enumerator = enumerable.GetEnumerator();
			if (enumerator.MoveNext())
			{
				object arg_14_0 = enumerator.Current;
				return false;
			}
		
		return true;
	}

	public static IList<T> PageList<T>(this ICollection<T> collection, int pageSize, ref int pageIndex, ref int recordCount, out int pageCount)
	{
		if (collection == null)
		{
			recordCount = 0;
			pageCount = 0;
			pageIndex = 0;
			return new List<T>(0);
		}
		recordCount = collection.Count;
		if (recordCount < 1)
		{
			pageCount = 0;
			pageIndex = 0;
			return new List<T>(0);
		}
		if (pageSize < 1)
		{
			pageSize = 1;
		}
		if (pageSize > recordCount)
		{
			pageSize = recordCount;
		}
		int result;
		//pageCount = Math.DivRem(recordCount, pageSize, out result);
		pageCount = recordCount / pageSize;
		result = recordCount % pageSize;
		if (result > 0)
		{
			pageCount++;
		}
		if (pageIndex < 1)
		{
			pageIndex = 1;
		}
		if (pageIndex > pageCount)
		{
			pageIndex = pageCount;
		}
		if (pageIndex > 1)
		{
			return collection.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList<T>();
		}
		return collection.Take(pageSize).ToList<T>();
	}

	public static IList<T> PageList<T>(this IEnumerable<T> enumerable, int pageSize, ref int pageIndex, ref int recordCount, out int pageCount)
	{
		if (enumerable == null)
		{
			recordCount = 0;
			pageCount = 0;
			pageIndex = 0;
			return new List<T>(0);
		}
		recordCount = enumerable.Count<T>();
		if (recordCount < 1)
		{
			pageCount = 0;
			pageIndex = 0;
			return new List<T>(0);
		}
		if (pageSize < 1)
		{
			pageSize = 1;
		}
		if (pageSize > recordCount)
		{
			pageSize = recordCount;
		}
		int result;
		//pageCount = Math.DivRem(recordCount, pageSize, out result);
		pageCount = recordCount / pageSize;
		result = recordCount % pageSize;
		if (result > 0)
		{
			pageCount++;
		}
		if (pageIndex < 1)
		{
			pageIndex = 1;
		}
		if (pageIndex > pageCount)
		{
			pageIndex = pageCount;
		}
		if (pageIndex > 1)
		{
			return enumerable.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList<T>();
		}
		return enumerable.Take(pageSize).ToList<T>();
	}

	public static void PageEach<T>(this ICollection<T> collection, int pageSize, Action<IList<T>> action)
	{
		int pageIndex = 0;
		int recordCount = 0;
		int pageCount = 0;
		while (true)
		{
			IList<T> pageList = collection.PageList(pageSize, ref pageIndex, ref recordCount, out pageCount);
			if (recordCount <= 0)
			{
				break;
			}
			action(pageList);
			if (pageCount <= pageIndex)
			{
				break;
			}
			pageIndex++;
		}
	}

	public static void PageEach<T>(this IEnumerable<T> enumerable, int pageSize, Action<IList<T>> action)
	{
		int pageIndex = 0;
		int recordCount = 0;
		int pageCount = 0;
		while (true)
		{
			IList<T> pageList = enumerable.PageList(pageSize, ref pageIndex, ref recordCount, out pageCount);
			if (recordCount <= 0)
			{
				break;
			}
			action(pageList);
			if (pageCount <= pageIndex)
			{
				break;
			}
			pageIndex++;
		}
	}

}
