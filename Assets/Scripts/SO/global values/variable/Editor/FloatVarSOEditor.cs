using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloatVarSO))]
public class FloatVarSOEditor : Editor
{
    SerializedProperty _inRange;
    SerializedProperty _min;
    SerializedProperty _max;

    private void OnEnable()
    {
        _inRange = serializedObject.FindProperty("_inRange");
        _min = serializedObject.FindProperty("_min");
        _max = serializedObject.FindProperty("_max");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_inRange, new GUIContent("Clamp values between range?"));

        if (_inRange.boolValue)
        {
            EditorGUILayout.PropertyField(_min, new GUIContent("Minimum Value"));
            EditorGUILayout.PropertyField(_max, new GUIContent("Maximum Value"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
