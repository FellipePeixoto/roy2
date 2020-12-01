using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PairingManagerEditor : Editor
{
    SerializedProperty _pairedPlayers;

    private void OnEnable()
    {
        _pairedPlayers = serializedObject.FindProperty("_pairedPlayers");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();



        serializedObject.ApplyModifiedProperties();
    }
}
