//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-08-01 20:26:47.585
//------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameApp.DataTable
{
    /// <summary>
    /// 新手引导配置表。
    /// </summary>
    public sealed class DRGuide : DataRowBase
    {
        private Int32 m_Id = 0;

        /// <summary>
        /// 获取步骤编号。
        /// </summary>
        public override Int32 Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取下一步的编号。
        /// </summary>
        public int NextId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取测试数组。
        /// </summary>
        public int[] TestArray
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取测试数组2。
        /// </summary>
        public Vector3[] TestArray2
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取测试列表。
        /// </summary>
        public List<int> TestList
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取测试字典。
        /// </summary>
        public Dictionary<int,int> TestDic
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(GameApp.DataTable.DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(GameApp.DataTable.DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
            NextId = int.Parse(columnStrings[index++]);
			TestArray = DataTableExtension.ParseInt32Array(columnStrings[index++]);
			TestArray2 = DataTableExtension.ParseVector3Array(columnStrings[index++]);
			TestList = DataTableExtension.ParseInt32List(columnStrings[index++]);
			TestDic = DataTableExtension.ParseInt32Int32Dictionary(columnStrings[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    NextId = binaryReader.Read7BitEncodedInt32();
					TestArray = binaryReader.ReadInt32Array();
					TestArray2 = binaryReader.ReadVector3Array();
					TestList = binaryReader.ReadInt32List();
					TestDic = binaryReader.ReadInt32Int32Dictionary();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private KeyValuePair<int, Vector3[]>[] m_TestArray = null;

        public int TestArrayCount
        {
            get
            {
                return m_TestArray.Length;
            }
        }

        public Vector3[] GetTestArray(int id)
        {
            foreach (KeyValuePair<int, Vector3[]> i in m_TestArray)
            {
                if (i.Key == id)
                {
                    return i.Value;
                }
            }

            throw new GameFrameworkException(Utility.Text.Format("GetTestArray with invalid id '{0}'.", id));
        }

        public Vector3[] GetTestArrayAt(int index)
        {
            if (index < 0 || index >= m_TestArray.Length)
            {
                throw new GameFrameworkException(Utility.Text.Format("GetTestArrayAt with invalid index '{0}'.", index));
            }

            return m_TestArray[index].Value;
        }

        private void GeneratePropertyArray()
        {
            m_TestArray = new KeyValuePair<int, Vector3[]>[]
            {
                new KeyValuePair<int, Vector3[]>(2, TestArray2),
            };
        }
    }
}
