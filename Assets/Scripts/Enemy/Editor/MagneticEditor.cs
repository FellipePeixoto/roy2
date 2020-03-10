using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MagneticEditor : Editor
{
    SerializedProperty _force;
    SerializedProperty _repulsive;

    private void OnEnable()
    {
        _force = serializedObject.FindProperty("_force");
        _repulsive = serializedObject.FindProperty("_repulsive");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_force, new GUIContent("Magnetic Force"));
        EditorGUILayout.PropertyField(_repulsive, new GUIContent("Is repulsive field?"));

        serializedObject.ApplyModifiedProperties();
    }
}
