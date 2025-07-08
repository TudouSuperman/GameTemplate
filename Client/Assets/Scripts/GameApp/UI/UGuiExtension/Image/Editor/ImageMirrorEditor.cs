using UnityEditor;
using UnityEditor.UI;

namespace GameApp.UI.Extension
{
    [CustomEditor(typeof(ImageMirror))]
    public sealed class ImageMirrorEditor : ImageEditor
    {
        private SerializedProperty m_MirrorType;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_MirrorType = serializedObject.FindProperty("m_MirrorType");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(m_MirrorType);
            serializedObject.ApplyModifiedProperties();
        }
    }
}