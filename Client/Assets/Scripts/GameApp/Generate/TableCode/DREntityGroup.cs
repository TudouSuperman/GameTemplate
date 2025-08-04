//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-08-04 18:01:13.728
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
    /// 实体组配置表。
    /// </summary>
    public sealed class DREntityGroup : DataRowBase
    {
        private Int32 m_Id = 0;

        /// <summary>
        /// 获取组编号。
        /// </summary>
        public override Int32 Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取组名称。
        /// </summary>
        public string GroupName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体实例对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体实例对象池容量。
        /// </summary>
        public int InstanceCapacity
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体实例对象池对象过期秒数。
        /// </summary>
        public float InstanceExpireTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体实例对象池的优先级。
        /// </summary>
        public int InstancePriority
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
            GroupName = columnStrings[index++];
            InstanceAutoReleaseInterval = float.Parse(columnStrings[index++]);
            InstanceCapacity = int.Parse(columnStrings[index++]);
            InstanceExpireTime = float.Parse(columnStrings[index++]);
            InstancePriority = int.Parse(columnStrings[index++]);

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
                    GroupName = binaryReader.ReadString();
                    InstanceAutoReleaseInterval = binaryReader.ReadSingle();
                    InstanceCapacity = binaryReader.Read7BitEncodedInt32();
                    InstanceExpireTime = binaryReader.ReadSingle();
                    InstancePriority = binaryReader.Read7BitEncodedInt32();
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
