using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameApp.DataTable
{
	public static partial class DataTableExtension
	{
		public static int[] ParseInt32Array(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split(',');
			int[] array = new int[splitValue.Length];
			for (int i = 0; i < splitValue.Length; i++)
			{
				array[i] = Int32.Parse(splitValue[i]);
			}

			return array;
		}
		public static Vector3[] ParseVector3Array(string value)
		{
			if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().Equals("null"))
				return null;
			string[] splitValue = value.Split('|');
			Vector3[] array = new Vector3[splitValue.Length];
			for (int i = 0; i < splitValue.Length; i++)
			{
				array[i] = ParseVector3(splitValue[i]);
			}

			return array;
		}
	}
}
