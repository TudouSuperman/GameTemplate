using UnityEditor;
using GameApp.Hotfix.UI.Extension;

namespace GameApp.UI.Extension.Editor
{
    [CustomEditor(typeof(GraphicRaycast))]
    public sealed class GraphicRaycastInspector : UnityEditor.Editor
    {
        SerializedProperty m_RaycastTarget;

        private void OnEnable()
        {
            m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_RaycastTarget);
        }
    }
}