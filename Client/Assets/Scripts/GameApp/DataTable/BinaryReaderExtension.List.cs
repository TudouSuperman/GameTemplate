using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameApp
{
	public static partial class BinaryReaderExtension
	{
		public static List<int> ReadInt32List(this BinaryReader binaryReader)
		{
			int count = binaryReader.Read7BitEncodedInt32();
			List<int> list = new List<int>(count);
			for (int i = 0; i < count; i++)
			{
				list.Add(binaryReader.Read7BitEncodedInt32());
			}
			return list;
		}
	}
}
