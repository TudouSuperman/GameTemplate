using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using GameApp.Hot.UI.Extension;

namespace GameApp.UI.Extension.Editor
{
    [CustomEditor(typeof(ImageShape))]
    public sealed class ImageShapeEditor : ImageEditor
    {
        private SerializedProperty m_Offset;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Offset = serializedObject.FindProperty(nameof(ImageShape.Offset));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Offset, new GUIContent("倾斜偏移"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}