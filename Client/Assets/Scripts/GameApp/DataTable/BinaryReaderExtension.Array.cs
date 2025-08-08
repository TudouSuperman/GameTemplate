using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameApp
{
	public static partial class BinaryReaderExtension
	{
		public static int[] ReadInt32Array(this BinaryReader binaryReader)
		{
			int count = binaryReader.Read7BitEncodedInt32();
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = binaryReader.Read7BitEncodedInt32();
			}
			return array;
		}
		public static Vector3[] ReadVector3Array(this BinaryReader binaryReader)
		{
			int count = binaryReader.Read7BitEncodedInt32();
			Vector3[] array = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = ReadVector3(binaryReader);
			}
			return array;
		}
	}
}
