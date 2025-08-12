using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameApp.Editor
{
    public static class AssetCleanTool
    {
        private static readonly string[] s_AssetFolders = new string[] { "Assets/Res" };

        [Tooltip("可以清理prefab中多余无效的序列化数据")]
        [MenuItem("GameApp/Asset Tool/重新序列化所有Prefab")]
        public static void GamePrefabOptimization()
        {
            // 找到所有Prefab的GUID
            string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", s_AssetFolders);
            string[] prefabPaths = new string[prefabGUIDs.Length];

            for (int i = 0; i < prefabGUIDs.Length; i++)
            {
                prefabPaths[i] = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
            }

            // 强制重新序列化所有Prefab
            AssetDatabase.ForceReserializeAssets(prefabPaths);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Force reserialize all prefab is done!");
        }

        [MenuItem("GameApp/Asset Tool/重新序列化所有Scene")]
        public static void GameSceneOptimization()
        {
            // 找到所有Scene的GUID
            string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", s_AssetFolders);
            string[] scenePaths = new string[sceneGUIDs.Length];

            for (int i = 0; i < sceneGUIDs.Length; i++)
            {
                scenePaths[i] = AssetDatabase.GUIDToAssetPath(sceneGUIDs[i]);
            }

            // 强制重新序列化所有Scene
            AssetDatabase.ForceReserializeAssets(scenePaths);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Force reserialize all scene is done!");
        }

        [MenuItem("GameApp/Asset Tool/清理所有粒子系统的无效Mesh引用")]
        public static void GameParticleSystemOptimization()
        {
            // 找到所有Prefab的GUID
            string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", s_AssetFolders);

            for (int i = 0; i < prefabGUIDs.Length; i++)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]));
                ParticleSystemRenderer[] renders = go.GetComponentsInChildren<ParticleSystemRenderer>(true);
                foreach (var renderItem in renders)
                {
                    if (renderItem.renderMode != ParticleSystemRenderMode.Mesh)
                    {
                        renderItem.mesh = null;
                        EditorUtility.SetDirty(go);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Clear all particle system's invalid mesh is done!");
        }

        [MenuItem("GameApp/Asset Tool/清理Material中废弃属性")]
        public static void ClearShaderUnusedProperties()
        {
            var matGuids = AssetDatabase.FindAssets("t:Material", s_AssetFolders);

            for (var idx = 0; idx < matGuids.Length; ++idx)
            {
                var guid = matGuids[idx];
                EditorUtility.DisplayProgressBar($"批处理中...{idx + 1}/{matGuids.Length}", "清理Material废弃属性", (idx + 1.0f) / matGuids.Length);

                var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                var matInfo = new SerializedObject(mat);
                var propArr = matInfo.FindProperty("m_SavedProperties");
                propArr.Next(true);
                do
                {
                    if (!propArr.isArray)
                    {
                        continue;
                    }

                    for (int i = propArr.arraySize - 1; i >= 0; --i)
                    {
                        var prop = propArr.GetArrayElementAtIndex(i);
                        if (!mat.HasProperty(prop.displayName))
                        {
                            propArr.DeleteArrayElementAtIndex(i);
                        }
                    }
                } while (propArr.Next(false));

                matInfo.ApplyModifiedProperties();
            }

            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();

            Debug.Log("Clear all material's properties is done!");
        }

        [MenuItem("GameApp/Asset Tool/清理 Behaviour is missing 中废弃属性")]
        public static void KillSceneIdMap()
        {
            List<string> _missingScriptGameObjectName = new List<string>();
            foreach (GameObject _gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (PrefabUtility.IsPartOfPrefabInstance(_gameObject) && PrefabUtility.IsAnyPrefabInstanceRoot(_gameObject))
                {
                    continue; // 此处跳过了预制体实例，如果不想跳过自行注释。
                }

                Component[] _components = _gameObject.GetComponents<Component>();
                foreach (Component _component in _components)
                {
                    if (_component == null)
                    {
                        Debug.Log("GameObject with missing script found: " + _gameObject.name, _gameObject);
                        _missingScriptGameObjectName.Add(_gameObject.name);
                        break;
                    }
                }
            }

            foreach (string _name in _missingScriptGameObjectName)
            {
                // 将找到的该物体名字传入即可删除。此处针对 HDRP 项目中，被隐藏掉的没用的物体 “SceneIDMap”
                while (GameObject.Find(_name) != null)
                {
                    GameObject _tempGameObject = GameObject.Find(_name);
                    if (_tempGameObject != null)
                    {
                        Object.DestroyImmediate(_tempGameObject);
                        Debug.Log($"Cleared a {_name} instance");
                    }
                    else
                    {
                        Debug.Log("Clear Completed!");
                        break;
                    }
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Clear all Behaviour is missing is done!");
        }
    }
}