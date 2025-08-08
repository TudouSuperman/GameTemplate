//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameApp.Editor
{
    public sealed partial class DataTableProcessor
    {
        public static class DataProcessorUtility
        {
            private static readonly IDictionary<string, DataProcessor> s_DataProcessors = new SortedDictionary<string, DataProcessor>(StringComparer.Ordinal);

            static DataProcessorUtility()
            {
                System.Type dataProcessorBaseType = typeof(DataProcessor);
                Assembly assembly = Assembly.GetExecutingAssembly();
                System.Type[] types = assembly.GetTypes();
                List<DataProcessor> addList = new List<DataProcessor>();
                for (int i = 0; i < types.Length; i++)
                {
                    if (!types[i].IsClass || types[i].IsAbstract || types[i].ContainsGenericParameters) continue;
                    if (dataProcessorBaseType.IsAssignableFrom(types[i]))
                    {
                        DataProcessor dataProcessor = (DataProcessor)Activator.CreateInstance(types[i]);
                        foreach (string typeString in dataProcessor.GetTypeStrings())
                        {
                            s_DataProcessors.Add(typeString.ToLowerInvariant(), dataProcessor);
                        }

                        addList.Add(dataProcessor);
                    }
                }

                AddListType(addList);
                AddArrayType(addList);
                AddDictionary(addList);
            }

            private static void AddArrayType(List<DataProcessor> addList)
            {
                var dataProcessorBaseType = typeof(DataProcessor);
                var type = typeof(ArrayProcessor<,>);
                for (var i = 0; i < addList.Count; i++)
                {
                    Type dataProcessorType = addList[i].GetType();
                    if (!dataProcessorType.HasImplementedRawGeneric(typeof(GenericDataProcessor<>))) continue;

                    var memberInfo = dataProcessorType.BaseType;

                    if (memberInfo != null)
                    {
                        Type[] typeArgs =
                        {
                            dataProcessorType,
                            memberInfo.GenericTypeArguments[0]
                        };
                        var arrayType = type.MakeGenericType(typeArgs);
                        if (dataProcessorBaseType.IsAssignableFrom(arrayType))
                        {
                            var dataProcessor = (DataProcessor)Activator.CreateInstance(arrayType);
                            var tDataProcessor = addList[i];
                            foreach (var typeString in dataProcessor.GetTypeStrings())
                            foreach (var tTypeString in tDataProcessor.GetTypeStrings())
                            {
                                var key = Utility.Text.Format(typeString.ToLower(), tTypeString);
                                s_DataProcessors.Add(key, dataProcessor);
                            }
                        }
                    }
                }
            }

            private static void AddListType(List<DataProcessor> addList)
            {
                var dataProcessorBaseType = typeof(DataProcessor);

                var type = typeof(ListProcessor<,>);

                for (var i = 0; i < addList.Count; i++)
                {
                    Type dataProcessorType = addList[i].GetType();

                    if (!dataProcessorType.HasImplementedRawGeneric(typeof(GenericDataProcessor<>))) continue;

                    var memberInfo = dataProcessorType.BaseType;

                    if (memberInfo != null)
                    {
                        Type[] typeArgs =
                        {
                            dataProcessorType,
                            memberInfo.GenericTypeArguments[0]
                        };
                        var listType = type.MakeGenericType(typeArgs);
                        if (dataProcessorBaseType.IsAssignableFrom(listType))
                        {
                            var dataProcessor =
                                (DataProcessor)Activator.CreateInstance(listType);
                            foreach (var typeString in dataProcessor.GetTypeStrings())
                            foreach (var tTypeString in addList[i].GetTypeStrings())
                            {
                                var key = Utility.Text.Format(typeString.ToLower(), tTypeString);
                                s_DataProcessors.Add(key, dataProcessor);
                            }
                        }
                    }
                }
            }

            private static void AddDictionary(List<DataProcessor> addList)
            {
                var dataProcessorBaseType = typeof(DataProcessor);
                var type = typeof(DictionaryProcessor<,,,>);
                var list = new List<DataProcessor>();
                for (var i = 0; i < addList.Count; i++)
                {
                    Type dataProcessorType = addList[i].GetType();

                    if (!dataProcessorType.HasImplementedRawGeneric(typeof(GenericDataProcessor<>))) continue;
                    var memberInfo = dataProcessorType.BaseType;

                    if (memberInfo != null) list.Add(addList[i]);
                }


                var keyValueList = PermutationAndCombination<DataProcessor>.GetCombination(list.ToArray(), 2).ToList();
                var reverseList = keyValueList.Select(types => new[] { types[1], types[0] }).ToList();
                keyValueList.AddRange(reverseList);
                foreach (var value in list) keyValueList.Add(new[] { value, value });
                foreach (var keyValue in keyValueList)
                {
                    var keyType = keyValue[0].GetType().BaseType;
                    var valueType = keyValue[1].GetType().BaseType;
                    if (keyType != null && valueType != null)
                    {
                        Type[] typeArgs =
                        {
                            keyValue[0].GetType(),
                            keyValue[1].GetType(),
                            keyType.GenericTypeArguments[0],
                            valueType.GenericTypeArguments[0]
                        };
                        var dictionaryType = type.MakeGenericType(typeArgs);
                        if (dataProcessorBaseType.IsAssignableFrom(dictionaryType))
                        {
                            var dataProcessor = (DataProcessor)Activator.CreateInstance(dictionaryType);
                            foreach (var typeString in dataProcessor.GetTypeStrings())
                            {
                                foreach (var key in keyValue[0].GetTypeStrings())
                                {
                                    foreach (var value in keyValue[1].GetTypeStrings())
                                    {
                                        var str = Utility.Text.Format(typeString.ToLower(), key, value);
                                        s_DataProcessors.Add(str, dataProcessor);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public static DataProcessor GetDataProcessor(string type)
            {
                if (type == null)
                {
                    type = string.Empty;
                }

                DataProcessor dataProcessor = null;
                if (s_DataProcessors.TryGetValue(type.ToLowerInvariant(), out dataProcessor))
                {
                    return dataProcessor;
                }

                throw new GameFrameworkException(Utility.Text.Format("Not supported data processor type '{0}'.", type));
            }
        }
    }
}