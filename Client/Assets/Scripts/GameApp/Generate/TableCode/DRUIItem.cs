//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-03-27 11:25:31.166
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;


namespace GameApp
{
    /// <summary>
    /// 界面动态加载资源配置表。
    /// </summary>
    public class DRUIItem : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取配置编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取物体名字。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源编号。
        /// </summary>
        public int AssetId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取物体组编号。
        /// </summary>
        public int ItemGroupId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string AssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源组名称。
        /// </summary>
        public string ItemGroupName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源个数。
        /// </summary>
        public int[] ItemCountArray
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源个数。
        /// </summary>
        public Vector3[] ItemCountV3
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源个数。
        /// </summary>
        public Dictionary<int,string> ItemCountDic
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源枚举。
        /// </summary>
        public GameApp.EDirection Cur
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(UnityGameFramework.Extension.DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(UnityGameFramework.Extension.DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
			Name = columnStrings[index++];
			AssetId = int.Parse(columnStrings[index++]);
			ItemGroupId = int.Parse(columnStrings[index++]);
			AssetName = columnStrings[index++];
			ItemGroupName = columnStrings[index++];
			ItemCountArray = DataTableExtension.ParseInt32Array(columnStrings[index++]);
			ItemCountV3 = DataTableExtension.ParseVector3Array(columnStrings[index++]);
			ItemCountDic = DataTableExtension.ParseInt32StringDictionary(columnStrings[index++]);
			Cur = UnityGameFramework.Extension.DataTableExtension.EnumParse<GameApp.EDirection>(columnStrings[index++]);
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
                    Name = binaryReader.ReadString();
                    AssetId = binaryReader.Read7BitEncodedInt32();
                    ItemGroupId = binaryReader.Read7BitEncodedInt32();
                    AssetName = binaryReader.ReadString();
                    ItemGroupName = binaryReader.ReadString();
					ItemCountArray = binaryReader.ReadInt32Array();
					ItemCountV3 = binaryReader.ReadVector3Array();
					ItemCountDic = binaryReader.ReadInt32StringDictionary();
					Cur = (GameApp.EDirection)binaryReader.Read7BitEncodedInt32();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private KeyValuePair<int, Vector3[]>[] m_ItemCountV = null;

        public int ItemCountVCount
        {
            get
            {
                return m_ItemCountV.Length;
            }
        }

        public Vector3[] GetItemCountV(int id)
        {
            foreach (KeyValuePair<int, Vector3[]> i in m_ItemCountV)
            {
                if (i.Key == id)
                {
                    return i.Value;
                }
            }

            throw new GameFrameworkException(Utility.Text.Format("GetItemCountV with invalid id '{0}'.", id.ToString()));
        }

        public Vector3[] GetItemCountVAt(int index)
        {
            if (index < 0 || index >= m_ItemCountV.Length)
            {
                throw new GameFrameworkException(Utility.Text.Format("GetItemCountVAt with invalid index '{0}'.", index.ToString()));
            }

            return m_ItemCountV[index].Value;
        }

        private void GeneratePropertyArray()
        {
            m_ItemCountV = new KeyValuePair<int, Vector3[]>[]
            {
                new KeyValuePair<int, Vector3[]>(3, ItemCountV3),
            };
        }
    }
}
