//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-07-18 21:28:30.698
//------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameApp
{
    /// <summary>
    /// 界面动态加载资源配置表。
    /// </summary>
    public sealed class DRUIItem : DataRowBase
    {
        private Int32 m_Id = 0;

        /// <summary>
        /// 获取配置编号。
        /// </summary>
        public override Int32 Id
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
            Name = columnStrings[index++];
            AssetId = int.Parse(columnStrings[index++]);
            ItemGroupId = int.Parse(columnStrings[index++]);
            AssetName = columnStrings[index++];
            ItemGroupName = columnStrings[index++];

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
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
