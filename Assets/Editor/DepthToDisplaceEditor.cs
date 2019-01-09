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

        SerializedProperty _sourceTexture;
        SerializedProperty _depthScale;

        SerializedProperty _lineColor;
        SerializedProperty _lineRepeat;

        SerializedProperty _sparkleColor;
        SerializedProperty _sparkleDensity;

        SerializedProperty _renderMode;
        SerializedProperty _deformation;

        void OnEnable()
        {
            _columnCount    = serializedObject.FindProperty("_columnCount");
            _rowCount       = serializedObject.FindProperty("_rowCount");

            _sourceTexture  = serializedObject.FindProperty("_sourceTexture");
            _depthScale     = serializedObject.FindProperty("_depthScale");

            _lineColor      = serializedObject.FindProperty("_lineColor");
            _lineRepeat     = serializedObject.FindProperty("_lineRepeat");

            _sparkleColor   = serializedObject.FindProperty("_sparkleColor");
            _sparkleDensity = serializedObject.FindProperty("_sparkleDensity");

            _renderMode     = serializedObject.FindProperty("_renderMode");
            _deformation    = serializedObject.FindProperty("_deformation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_columnCount);
            EditorGUILayout.PropertyField(_rowCount);
            var needsReconstruct = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(_sourceTexture);
            EditorGUILayout.PropertyField(_depthScale);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_lineColor);
            EditorGUILayout.PropertyField(_lineRepeat);

            EditorGUILayout.PropertyField(_sparkleColor);
            EditorGUILayout.PropertyField(_sparkleDensity);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_renderMode);
            needsReconstruct |= EditorGUI.EndChangeCheck();
            EditorGUILayout.PropertyField(_deformation);

            serializedObject.ApplyModifiedProperties();

            if (needsReconstruct)
                foreach (DepthToDisplace d2d in targets) d2d.Reconstruct();
        }
    }
}
