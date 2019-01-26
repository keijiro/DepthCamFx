using UnityEngine;
using UnityEditor;

namespace Ditho
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Surface))]
    sealed class SurfaceEditor : Editor
    {
        SerializedProperty _columnCount;
        SerializedProperty _rowCount;

        SerializedProperty _sourceTexture;
        SerializedProperty _depth;
        SerializedProperty _cutoff;
        SerializedProperty _noiseAmplitude;
        SerializedProperty _noiseAnimation;

        SerializedProperty _renderMode;
        SerializedProperty _lineColor;
        SerializedProperty _lineWidth;
        SerializedProperty _lineRepeat;
        SerializedProperty _sparkleColor;
        SerializedProperty _sparkleDensity;

        void OnEnable()
        {
            _columnCount = serializedObject.FindProperty("_columnCount");
            _rowCount    = serializedObject.FindProperty("_rowCount");

            _sourceTexture  = serializedObject.FindProperty("_sourceTexture");
            _depth          = serializedObject.FindProperty("_depth");
            _cutoff         = serializedObject.FindProperty("_cutoff");
            _noiseAmplitude = serializedObject.FindProperty("_noiseAmplitude");
            _noiseAnimation = serializedObject.FindProperty("_noiseAnimation");

            _renderMode     = serializedObject.FindProperty("_renderMode");
            _lineColor      = serializedObject.FindProperty("_lineColor");
            _lineWidth      = serializedObject.FindProperty("_lineWidth");
            _lineRepeat     = serializedObject.FindProperty("_lineRepeat");
            _sparkleColor   = serializedObject.FindProperty("_sparkleColor");
            _sparkleDensity = serializedObject.FindProperty("_sparkleDensity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_columnCount);
            EditorGUILayout.PropertyField(_rowCount);
            var needsReconstruct = EditorGUI.EndChangeCheck();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_sourceTexture);
            EditorGUILayout.PropertyField(_depth);
            EditorGUILayout.PropertyField(_cutoff);
            EditorGUILayout.PropertyField(_noiseAmplitude);
            EditorGUILayout.PropertyField(_noiseAnimation);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_renderMode);
            EditorGUILayout.PropertyField(_lineColor);
            EditorGUILayout.PropertyField(_lineWidth);
            EditorGUILayout.PropertyField(_lineRepeat);
            EditorGUILayout.PropertyField(_sparkleColor);
            EditorGUILayout.PropertyField(_sparkleDensity);

            serializedObject.ApplyModifiedProperties();

            if (needsReconstruct)
                foreach (Surface s in targets) s.ReconstructMesh();
        }
    }
}
