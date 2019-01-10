using UnityEngine;
using UnityEditor;

namespace Ditho
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Fiber))]
    sealed class FiberEditor : Editor
    {
        SerializedProperty _pointCount;
        SerializedProperty _curveLength;
        SerializedProperty _curveAnimation;

        SerializedProperty _sourceTexture;
        SerializedProperty _depthScale;
        SerializedProperty _noiseAmplitude;
        SerializedProperty _noiseAnimation;

        SerializedProperty _lineColor;

        void OnEnable()
        {
            _pointCount     = serializedObject.FindProperty("_pointCount");
            _curveLength    = serializedObject.FindProperty("_curveLength");
            _curveAnimation = serializedObject.FindProperty("_curveAnimation");

            _sourceTexture  = serializedObject.FindProperty("_sourceTexture");
            _depthScale     = serializedObject.FindProperty("_depthScale");
            _noiseAmplitude = serializedObject.FindProperty("_noiseAmplitude");
            _noiseAnimation = serializedObject.FindProperty("_noiseAnimation");

            _lineColor      = serializedObject.FindProperty("_lineColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_pointCount);
            var needsReconstruct = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(_curveLength);
            EditorGUILayout.PropertyField(_curveAnimation);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_sourceTexture);
            EditorGUILayout.PropertyField(_depthScale);
            EditorGUILayout.PropertyField(_noiseAmplitude);
            EditorGUILayout.PropertyField(_noiseAnimation);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_lineColor);

            serializedObject.ApplyModifiedProperties();

            if (needsReconstruct)
                foreach (Fiber f in targets) f.ReconstructMesh();
        }
    }
}
