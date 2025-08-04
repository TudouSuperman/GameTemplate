//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.DataTable;
using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameApp.DataTable
{
    public static partial class DataTableExtension
    {
        private const string DataRowClassPrefixName = "GameApp.DataTable.DR";
        public static readonly char[] DataSplitSeparators = new char[] { '\t' };
        public static readonly char[] DataTrimSeparators = new char[] { '\"' };

        public static void LoadDataTable(this DataTableComponent dataTableComponent, string dataTableName, string dataTableAssetName, object userData)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string[] splitedNames = dataTableName.Split('_');
            if (splitedNames.Length > 2)
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string dataRowClassName = DataRowClassPrefixName + splitedNames[0];
            Type dataRowType = Type.GetType(dataRowClassName);
            if (dataRowType == null)
            {
                Log.Warning("Can not get data row type with class name '{0}'.", dataRowClassName);
                return;
            }

            string name = splitedNames.Length > 1 ? splitedNames[1] : null;
            DataTableBase dataTable = dataTableComponent.CreateDataTable(dataRowType, name);
            dataTable.ReadData(dataTableAssetName, Constant.AssetPriority.DataTable_Asset, userData);
        }

        public static void LoadDataTable(this DataTableComponent dataTableComponent, string dataTableName, string dataTableAssetName, Type dataRowType, object userData)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string[] splitedNames = dataTableName.Split('_');
            if (splitedNames.Length > 2)
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string name = splitedNames.Length > 1 ? splitedNames[1] : null;
            DataTableBase dataTable = dataTableComponent.CreateDataTable(dataRowType, name);
            dataTable.ReadData(dataTableAssetName, Constant.AssetPriority.DataTable_Asset, userData);
        }

        public static void LoadDataTable<T>(this DataTableComponent dataTableComponent, string dataTableName, string dataTableAssetName, object userData) where T : DataTableBase
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string[] splitedNames = dataTableName.Split('_');
            if (splitedNames.Length > 2)
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            Type dataRowType = typeof(T);
            string name = splitedNames.Length > 1 ? splitedNames[1] : null;
            DataTableBase dataTable = dataTableComponent.CreateDataTable(dataRowType, name);
            dataTable.ReadData(dataTableAssetName, Constant.AssetPriority.DataTable_Asset, userData);
        }

        public static TDataRow GetDataRow<TDataRow>(this DataTableComponent dataTableComponent, int dataRowId) where TDataRow : DataRowBase
        {
            IDataTable<TDataRow> dataTable = dataTableComponent.GetDataTable<TDataRow>();
            TDataRow dataRow = dataTable?.GetDataRow(dataRowId);
            if (dataRow == null)
            {
                Log.Warning("Can not load {0} '{1}' from data table.", typeof(TDataRow), dataRowId.ToString());
                return null;
            }

            return dataRow;
        }

        public static Color32 ParseColor32(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Color32(byte.Parse(splitedValue[0]), byte.Parse(splitedValue[1]), byte.Parse(splitedValue[2]), byte.Parse(splitedValue[3]));
        }

        public static Color ParseColor(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Color(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Quaternion ParseQuaternion(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Quaternion(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Rect ParseRect(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Rect(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Vector2 ParseVector2(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector2(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]));
        }

        public static Vector3 ParseVector3(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector3(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]));
        }

        public static Vector4 ParseVector4(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector4(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }
    }
}