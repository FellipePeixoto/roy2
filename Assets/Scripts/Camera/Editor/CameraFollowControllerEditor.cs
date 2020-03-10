using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraFollowController))]
public class CameraFollowControllerEditor : Editor
{
    SerializedProperty _target;
    SerializedProperty _offset;

    private void OnEnable()
    {
        _target = serializedObject.FindProperty("_target");
        _offset = serializedObject.FindProperty("_offset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_target, new GUIContent("Cam Target"));
        EditorGUILayout.PropertyField(_offset, new GUIContent("Cam target offset"));
        serializedObject.ApplyModifiedProperties();
    }
}