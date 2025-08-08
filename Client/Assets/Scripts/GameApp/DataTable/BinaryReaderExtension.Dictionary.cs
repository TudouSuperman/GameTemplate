using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameApp
{
	public static partial class BinaryReaderExtension
	{
		public static Dictionary<int,int> ReadInt32Int32Dictionary(this BinaryReader binaryReader)
		{
			int count = binaryReader.Read7BitEncodedInt32();
			Dictionary<int,int> dictionary = new Dictionary<int,int>(count);
			for (int i = 0; i < count; i++)
			{
				dictionary.Add(binaryReader.Read7BitEncodedInt32(),binaryReader.Read7BitEncodedInt32());
			}
			return dictionary;
		}
	}
}
