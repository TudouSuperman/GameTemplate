using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace UnityGameFramework.Extensions
{
	public static partial class BinaryReaderExtension
	{
		public static Dictionary<int,string> ReadInt32StringDictionary(this BinaryReader binaryReader)
		{
			int count = binaryReader.Read7BitEncodedInt32();
			Dictionary<int,string> dictionary = new Dictionary<int,string>(count);
			for (int i = 0; i < count; i++)
			{
				dictionary.Add(binaryReader.Read7BitEncodedInt32(),binaryReader.ReadString());
			}
			return dictionary;
		}
	}
}
