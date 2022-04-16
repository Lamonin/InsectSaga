using System;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PlayAnimationAfterDelay))]
public class PlayAnimationAfterDelay_Inspector : Editor
{
    private SerializedProperty _isDelayInRange;
    private SerializedProperty _delayFrom;
    private SerializedProperty _delayTo;
    private SerializedProperty _animationName;

    private void OnEnable()
    {
        _isDelayInRange = serializedObject.FindProperty("randomFromRange");
        _delayFrom = serializedObject.FindProperty("delayFrom");
        _delayTo = serializedObject.FindProperty("delayTo");
        _animationName = serializedObject.FindProperty("animationName");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_isDelayInRange, new GUIContent("Случайная задержка"));
        if (_isDelayInRange.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_delayFrom, new GUIContent("От:"));
            EditorGUILayout.PropertyField(_delayTo, new GUIContent("До:"));
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.PropertyField(_delayTo, new GUIContent("Задержка"));
        }
        EditorGUILayout.PropertyField(_animationName, new GUIContent("Название анимации"));

        serializedObject.ApplyModifiedProperties();
    }
}
