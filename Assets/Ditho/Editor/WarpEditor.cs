using UnityEngine;
using UnityEditor;

namespace Ditho
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Warp))]
    sealed class WarpEditor : Editor
    {
        SerializedProperty _lineCount;

        SerializedProperty _depth;
        SerializedProperty _cutoff;
        SerializedProperty _extent;

        SerializedProperty _speed;
        SerializedProperty _speedRandomness;
        SerializedProperty _length;
        SerializedProperty _lengthRandomness;

        SerializedProperty _lineColor;
        SerializedProperty _sparkleColor;
        SerializedProperty _sparkleDensity;

        static readonly GUIContent _labelRandomness = new GUIContent("Randomness");

        void OnEnable()
        {
            _lineCount = serializedObject.FindProperty("_lineCount");

            _depth  = serializedObject.FindProperty("_depth");
            _cutoff = serializedObject.FindProperty("_cutoff");
            _extent = serializedObject.FindProperty("_extent");

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

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_depth);
            EditorGUILayout.PropertyField(_cutoff);
            EditorGUILayout.PropertyField(_extent);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_speed);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_speedRandomness, _labelRandomness);
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(_length);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_lengthRandomness, _labelRandomness);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_lineColor);
            EditorGUILayout.PropertyField(_sparkleColor);
            EditorGUILayout.PropertyField(_sparkleDensity);

            serializedObject.ApplyModifiedProperties();

            if (needsReconstruct)
                foreach (Warp w in targets) w.ReconstructMesh();
        }
    }
}
