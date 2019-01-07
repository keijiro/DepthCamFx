using UnityEngine;
using UnityEditor;

namespace DepthCamFx
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DepthToDisplace))]
    sealed class DepthToDisplaceEditor : Editor
    {
        SerializedProperty _columnCount;
        SerializedProperty _rowCount;
        SerializedProperty _baseColor;
        SerializedProperty _sparkleColor;
        SerializedProperty _depthScale;
        SerializedProperty _sourceTexture;
        SerializedProperty _shader;

        void OnEnable()
        {
            _columnCount   = serializedObject.FindProperty("_columnCount");
            _rowCount      = serializedObject.FindProperty("_rowCount");
            _baseColor     = serializedObject.FindProperty("_baseColor");
            _sparkleColor  = serializedObject.FindProperty("_sparkleColor");
            _depthScale    = serializedObject.FindProperty("_depthScale");
            _sourceTexture = serializedObject.FindProperty("_sourceTexture");
            _shader        = serializedObject.FindProperty("_shader");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_columnCount);
            EditorGUILayout.PropertyField(_rowCount);

            if (EditorGUI.EndChangeCheck())
                foreach (DepthToDisplace d2d in targets) d2d.ReconstructMesh();

            EditorGUILayout.PropertyField(_baseColor);
            EditorGUILayout.PropertyField(_sparkleColor);
            EditorGUILayout.PropertyField(_depthScale);
            EditorGUILayout.PropertyField(_sourceTexture);
            EditorGUILayout.PropertyField(_shader);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
