using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MagnetableControllerEditor : Editor
{
    SerializedProperty _isActive;

    private void OnEnable()
    {
        _isActive = serializedObject.FindProperty("_isActive");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_isActive, new GUIContent("Enabled"));

        serializedObject.ApplyModifiedProperties();
    }
}