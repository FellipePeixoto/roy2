using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScavengerMovementPattern))]
public class ScavengerMovementPatternEditor : Editor
{
    SerializedProperty _speed;
    SerializedProperty _minimumDistance;
    SerializedProperty _start;
    SerializedProperty _end;
    SerializedProperty stop;

    private void OnEnable()
    {
        _speed = serializedObject.FindProperty("_speed");
        _minimumDistance = serializedObject.FindProperty("_minimumDistance");
        _start = serializedObject.FindProperty("_start");
        _end = serializedObject.FindProperty("_end");
        stop = serializedObject.FindProperty("stop");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_speed, new GUIContent("Scavenger speed/s"));
        EditorGUILayout.PropertyField(_minimumDistance, new GUIContent("Distancia minina pontas (start end)"));
        EditorGUILayout.PropertyField(_start, new GUIContent("Ponto de final esquerdo"));
        EditorGUILayout.PropertyField(_end, new GUIContent("Ponto final direito"));
        EditorGUILayout.PropertyField(stop, new GUIContent("stop stop stop"));

        serializedObject.ApplyModifiedProperties();
    }
}
