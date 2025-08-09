//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-08-09 18:06:41.719
//------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameApp.Hotfix
{
    /// <summary>
    /// 角色表。
    /// </summary>
    public sealed class DRHero : DataRowBase
    {
        private Int32 m_Id = 0;

        /// <summary>
        /// 获取角色编号。
        /// </summary>
        public override Int32 Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取角色名称。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体编号（实体表）。
        /// </summary>
        public int EntityId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取基础数据编号（角色基础数据表）。
        /// </summary>
        public int BaseDataId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取角色名称（多语言表）。
        /// </summary>
        public string HeroName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体位置。
        /// </summary>
        public Vector3 Position
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体朝向。
        /// </summary>
        public Quaternion Rotation
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(GameApp.DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(GameApp.DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
            Name = columnStrings[index++];
            EntityId = int.Parse(columnStrings[index++]);
            BaseDataId = int.Parse(columnStrings[index++]);
            HeroName = columnStrings[index++];
			Position = DataTableExtension.ParseVector3(columnStrings[index++]);
			Rotation = DataTableExtension.ParseQuaternion(columnStrings[index++]);
            index++;
            index++;

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
                    EntityId = binaryReader.Read7BitEncodedInt32();
                    BaseDataId = binaryReader.Read7BitEncodedInt32();
                    HeroName = binaryReader.ReadString();
                    Position = binaryReader.ReadVector3();
                    Rotation = binaryReader.ReadQuaternion();
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
