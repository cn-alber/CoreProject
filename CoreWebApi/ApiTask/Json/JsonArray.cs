using System.Collections.Generic;

namespace API.Json
{
	public sealed class JsonArray : List<object>
	{
		public static readonly JsonArray Empty = new JsonArray();
	}
}
