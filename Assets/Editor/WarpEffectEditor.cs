using UnityEngine;
using UnityEditor;

namespace DepthCamFx
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(WarpEffect))]
    sealed class WarpEffectEditor : Editor
    {
        SerializedProperty _lineCount;

        SerializedProperty _speed;
        SerializedProperty _speedRandomness;
        SerializedProperty _length;
        SerializedProperty _lengthRandomness;

        SerializedProperty _lineColor;
        SerializedProperty _sparkleColor;
        SerializedProperty _sparkleDensity;

        void OnEnable()
        {
            _lineCount = serializedObject.FindProperty("_lineCount");

            _speed            = serializedObject.FindProperty("_speed");
            _speedRandomness  = serializedObject.FindProperty("_speedRandomness");
            _length           = serializedObject.FindProperty("_length");
            _lengthRandomness = serializedObject.FindProperty("_lengthRandomness");

            _lineColor      = serializedObject.FindProperty("_lineColor");
            _sparkleColor   = serializedObject.FindProperty("_sparkleColor");
            _sparkleDensity = serializedObject.FindProperty("_sparkleDensity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_lineCount);
            var needsReconstruct = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(_speed);
            EditorGUILayout.PropertyField(_speedRandomness);
            EditorGUILayout.PropertyField(_length);
            EditorGUILayout.PropertyField(_lengthRandomness);

            EditorGUILayout.PropertyField(_lineColor);
            EditorGUILayout.PropertyField(_sparkleColor);
            EditorGUILayout.PropertyField(_sparkleDensity);

            serializedObject.ApplyModifiedProperties();

            if (needsReconstruct)
                foreach (WarpEffect we in targets) we.ReconstructMesh();
        }
    }
}
