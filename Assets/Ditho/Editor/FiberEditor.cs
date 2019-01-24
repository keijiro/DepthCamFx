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
        SerializedProperty _depth;
        SerializedProperty _cutoff;
        SerializedProperty _noiseAmplitude;
        SerializedProperty _noiseAnimation;

        SerializedProperty _lineColor;
        SerializedProperty _attenuation;

        void OnEnable()
        {
            _pointCount     = serializedObject.FindProperty("_pointCount");
            _curveLength    = serializedObject.FindProperty("_curveLength");
            _curveAnimation = serializedObject.FindProperty("_curveAnimation");

            _sourceTexture  = serializedObject.FindProperty("_sourceTexture");
            _depth          = serializedObject.FindProperty("_depth");
            _cutoff         = serializedObject.FindProperty("_cutoff");
            _noiseAmplitude = serializedObject.FindProperty("_noiseAmplitude");
            _noiseAnimation = serializedObject.FindProperty("_noiseAnimation");

            _lineColor      = serializedObject.FindProperty("_lineColor");
            _attenuation    = serializedObject.FindProperty("_attenuation");
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
            EditorGUILayout.PropertyField(_depth);
            EditorGUILayout.PropertyField(_cutoff);
            EditorGUILayout.PropertyField(_noiseAmplitude);
            EditorGUILayout.PropertyField(_noiseAnimation);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_lineColor);
            EditorGUILayout.PropertyField(_attenuation);

            serializedObject.ApplyModifiedProperties();

            if (needsReconstruct)
                foreach (Fiber f in targets) f.ReconstructMesh();
        }
    }
}
