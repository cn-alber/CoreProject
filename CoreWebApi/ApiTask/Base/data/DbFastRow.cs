using System;
using System.Collections.Generic;

namespace API.Data
{
	public class DbFastRow
	{
		public IDictionary<string, int> FieldMapper
		{
			get;
			private set;
		}

		public object[] Values
		{
			get;
			private set;
		}

		public virtual object this[int index]
		{
			get
			{
				return this.Values[index];
			}
			set
			{
				this.Values[index] = value;
			}
		}

		public virtual object this[string name]
		{
			get
			{
				int num = this.FieldMapper[name];
				return this.Values[num];
			}
			set
			{
				int num = this.FieldMapper[name];
				this.Values[num] = value;
			}
		}

		public DbFastRow(IDictionary<string, int> fieldMapper, object[] values)
		{
			if (fieldMapper == null || fieldMapper.Count == 0)
			{
				throw new ArgumentNullException("fieldMapper");
			}
			if (values == null || values.Length == 0 || values.Length != fieldMapper.Count)
			{
				throw new ArgumentNullException("values");
			}
			this.FieldMapper = fieldMapper;
			this.Values = values;
		}

		public DbFastRow(IDictionary<string, object> data)
		{
			if (data == null || data.Count == 0)
			{
				throw new ArgumentNullException("data");
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>(data.Count);
			object[] array = new object[data.Count];
			int num = 0;
			foreach (KeyValuePair<string, object> current in data)
			{
				dictionary.Add(current.Key, num);
				array[num] = current.Value;
				num++;
			}
			this.FieldMapper = dictionary;
			this.Values = array;
		}
	}
}
