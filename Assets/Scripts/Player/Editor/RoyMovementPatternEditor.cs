
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoyMovementPattern))]
public class RoyMovementPatternEditor : Editor
{
    SerializedProperty _speed;
    SerializedProperty _grappleMax;
    SerializedProperty _grappleVelocity;
    SerializedProperty _jumpHeight;
    SerializedProperty _airSpeed;
    SerializedProperty _mov1Charge;
    SerializedProperty _mov2Charge;
    SerializedProperty _debugging;
    SerializedProperty _stringForce;
    SerializedProperty _damp;
    SerializedProperty _gasLevel;
    SerializedProperty _aimLine;
    SerializedProperty _hookLine;
    SerializedProperty _royBodyView;
    SerializedProperty _groundDetectorOffset;
    SerializedProperty _groundDetectorSize;
    SerializedProperty _style;

    private void OnEnable()
    {
        _speed = serializedObject.FindProperty("_speed");
        _grappleVelocity = serializedObject.FindProperty("_grappleVelocity");
        _grappleMax = serializedObject.FindProperty("_grappleMax");
        _airSpeed = serializedObject.FindProperty("_airSpeed");
        _jumpHeight = serializedObject.FindProperty("_jumpHeight");
        _mov1Charge = serializedObject.FindProperty("_mov1Charge");
        _mov2Charge = serializedObject.FindProperty("_mov2Charge");
        _stringForce = serializedObject.FindProperty("_stringForce");
        _damp = serializedObject.FindProperty("_damp");
        _debugging = serializedObject.FindProperty("_debugging");
        _gasLevel = serializedObject.FindProperty("_gasLevel");
        _aimLine = serializedObject.FindProperty("_aimLine");
        _hookLine = serializedObject.FindProperty("_hookLine");
        _royBodyView = serializedObject.FindProperty("_royBodyView");
        _groundDetectorOffset = serializedObject.FindProperty("_groundDetectorOffset");
        _groundDetectorSize = serializedObject.FindProperty("_groundDetectorSize");
        _style = serializedObject.FindProperty("_style");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        (target as RoyMovementPattern)._groupConstants = EditorGUILayout.BeginFoldoutHeaderGroup((target as RoyMovementPattern)._groupConstants,
            new GUIContent("Move Constants"));

        if ((target as RoyMovementPattern)._groupConstants)
        {
            EditorGUILayout.PropertyField(_speed, new GUIContent("Player Speed/s"));
            EditorGUILayout.PropertyField(_airSpeed, new GUIContent("Player Speed/s on Air"));
            EditorGUILayout.PropertyField(_jumpHeight, new GUIContent("Jump Height"));
            EditorGUILayout.PropertyField(_gasLevel, new GUIContent("Var Gas Level"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        (target as RoyMovementPattern)._groupConsume =
            EditorGUILayout.BeginFoldoutHeaderGroup((target as RoyMovementPattern)._groupConsume,
            new GUIContent("Special Move Constants"));

        if ((target as RoyMovementPattern)._groupConsume)
        {
            EditorGUILayout.PropertyField(_stringForce, new GUIContent("String Force"));
            EditorGUILayout.PropertyField(_damp, new GUIContent("Damping factor"));
            EditorGUILayout.LabelField("[Movimentação 1] Custo por segundo");
            _mov1Charge.floatValue = EditorGUILayout.Slider(_mov1Charge.floatValue, 0, 100, null);
            EditorGUILayout.LabelField("[Movimentação 2] Custo por unidade");
            _mov2Charge.floatValue = EditorGUILayout.Slider(_mov2Charge.floatValue, 0, 100, null);
            EditorGUILayout.PropertyField(_grappleMax, new GUIContent("Grapple max size"));
            EditorGUILayout.PropertyField(_grappleVelocity, new GUIContent("Player Speed/s on Grapple"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        (target as RoyMovementPattern)._gameObjects =
            EditorGUILayout.BeginFoldoutHeaderGroup((target as RoyMovementPattern)._groupConsume,
            new GUIContent("Child game objects importants"));

        if ((target as RoyMovementPattern)._gameObjects)
        {
            EditorGUILayout.PropertyField(_aimLine, new GUIContent("Line Renderer for aim"));
            EditorGUILayout.PropertyField(_hookLine, new GUIContent("Line Renderer for hook"));
            EditorGUILayout.PropertyField(_royBodyView, new GUIContent("Roy Main Mesh"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        (target as RoyMovementPattern)._debbgOptions =
        EditorGUILayout.BeginFoldoutHeaderGroup((target as RoyMovementPattern)._groupConsume,
        new GUIContent("Debug Options"));

        if ((target as RoyMovementPattern)._debbgOptions)
        {
            EditorGUILayout.PropertyField(_style, new GUIContent("Font Style"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        (target as RoyMovementPattern)._physicsSettings =
        EditorGUILayout.BeginFoldoutHeaderGroup((target as RoyMovementPattern)._physicsSettings,
        new GUIContent("Physics settings"));

        if ((target as RoyMovementPattern)._physicsSettings)
        {
            EditorGUILayout.PropertyField(_groundDetectorOffset, new GUIContent("Offset of ground checker"));
            EditorGUILayout.PropertyField(_groundDetectorSize, new GUIContent("Checker size"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
