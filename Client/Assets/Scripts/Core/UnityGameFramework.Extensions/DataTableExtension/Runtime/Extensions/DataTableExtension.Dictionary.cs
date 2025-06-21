using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace UnityGameFramework.Extensions
{
	public static partial class DataTableExtension
	{
		public static Dictionary<int,string> ParseInt32StringDictionary(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			Dictionary<int,string> dictionary = new Dictionary<int,string>(splitValue.Length);
			for (int i = 0; i < splitValue.Length; i++)
			{
				string[] keyValue = splitValue[i].Split('#');
				dictionary.Add(Int32.Parse(keyValue[0].Substring(1)),keyValue[1].Substring(0, keyValue[1].Length - 1));
			}
			return dictionary;
		}
	}
}
