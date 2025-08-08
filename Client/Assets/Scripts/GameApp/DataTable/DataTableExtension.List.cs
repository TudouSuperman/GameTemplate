using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameApp
{
	public static partial class DataTableExtension
	{
		public static List<int> ParseInt32List(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			List<int> list = new List<int>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				list.Add(Int32.Parse(splitValue[i]));
			}
			return list;
		}
	}
}
