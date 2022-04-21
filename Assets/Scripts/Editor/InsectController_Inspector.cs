using UnityEngine;
using UnityEditor;
using Controllers;

[CustomEditor(typeof(InsectController))]
public class InsectController_Inspector : Editor
{
    private SerializedProperty m_WalkSpeed;
    private SerializedProperty m_RunSpeed;
    private SerializedProperty m_StickOffsetBeforeRun;
    private SerializedProperty m_IsCanJump;
    private SerializedProperty m_JumpPower;
    private SerializedProperty m_HangTime;
    private SerializedProperty m_JumpBufferTime;
    private SerializedProperty m_CrawlSpeed;
    private SerializedProperty m_RotationSpeed;
    private SerializedProperty m_JumpCrawlPower;
    private SerializedProperty m_JumpCrawlFromWallPower;
    private SerializedProperty m_GroundLayer;
    private SerializedProperty m_WalkGroundLayer;
    private SerializedProperty m_GroundCheckPos;
    private SerializedProperty m_GroundCheckRadius;
    private SerializedProperty m_GrabWallDistance;
    private SerializedProperty m_GrabGroundDistance;
    private SerializedProperty m_RayGroundLength;
    private SerializedProperty m_Sprite;
    private SerializedProperty m_Animator;
    private SerializedProperty m_CollTransform;
    private SerializedProperty m_PlatformerCollTransform;

    private bool m_IsDebug;

    private void OnEnable()
    {
        m_WalkSpeed = serializedObject.FindProperty("walkSpeed");
        m_RunSpeed = serializedObject.FindProperty("runSpeed");
        m_StickOffsetBeforeRun = serializedObject.FindProperty("stickOffsetBeforeRun");
        m_IsCanJump = serializedObject.FindProperty("isCanJump");
        m_JumpPower = serializedObject.FindProperty("jumpPower");
        m_HangTime = serializedObject.FindProperty("hangTime");
        m_JumpBufferTime = serializedObject.FindProperty("jumpBufferTime");
       
        m_CrawlSpeed = serializedObject.FindProperty("crawlSpeed");
        m_RotationSpeed = serializedObject.FindProperty("rotationSpeed");
        m_JumpCrawlPower = serializedObject.FindProperty("jumpCrawlPower");
        m_JumpCrawlFromWallPower = serializedObject.FindProperty("jumpCrawlFromWallPower");
        
        m_GroundLayer = serializedObject.FindProperty("groundLayer");
        m_WalkGroundLayer = serializedObject.FindProperty("walkGroundLayer");
        m_GroundCheckPos = serializedObject.FindProperty("groundCheckPos");
        m_GroundCheckRadius = serializedObject.FindProperty("groundCheckRadius");
        m_GrabWallDistance = serializedObject.FindProperty("grabWallDistance");
        m_GrabGroundDistance = serializedObject.FindProperty("grabGroundDistance");
        m_RayGroundLength = serializedObject.FindProperty("rayGroundLength");
        
        m_Sprite = serializedObject.FindProperty("sprite");
        m_Animator = serializedObject.FindProperty("animator");
        m_CollTransform = serializedObject.FindProperty("collTransform");
        m_PlatformerCollTransform = serializedObject.FindProperty("platformerColl");
    }

    private void AddHeader(string label, bool space = true)
    {
        if (space) EditorGUILayout.Space();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
    }

    private void NoAttributeProperty(SerializedProperty property, string label)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);
        EditorGUILayout.PropertyField(property, GUIContent.none);
        EditorGUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        AddHeader("Режим ходьбы", false); 
        //NoAttributeProperty(m_WalkSpeed, "Скорость ходьбы");
        EditorGUILayout.PropertyField(m_WalkSpeed, new GUIContent("Скорость ходьбы"));
        EditorGUILayout.PropertyField(m_RunSpeed, new GUIContent("Скорость бега"));
        EditorGUILayout.PropertyField(m_StickOffsetBeforeRun, new GUIContent("Мин. отклон. стика"));
        
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_IsCanJump, new GUIContent("Может ли прыгать?"));
        EditorGUILayout.PropertyField(m_JumpPower, new GUIContent("Сила прыжка"));
        EditorGUILayout.PropertyField(m_HangTime, new GUIContent("Прыжок Койота"));
        EditorGUILayout.PropertyField(m_JumpBufferTime, new GUIContent("Буфер прыжка"));
        
        AddHeader("Режим ползанья");
        EditorGUILayout.PropertyField(m_CrawlSpeed, new GUIContent("Скорость ползанья"));
        EditorGUILayout.PropertyField(m_RotationSpeed, new GUIContent("Скорость поворота"));
        EditorGUILayout.PropertyField(m_JumpCrawlPower, new GUIContent("Сила прыжка"));
        EditorGUILayout.PropertyField(m_JumpCrawlFromWallPower, new GUIContent("Сила прыжка от стены"));
        
        // AddHeader("Другое");
        EditorGUILayout.PropertyField(m_GroundLayer, new GUIContent("Слой поверхности"));
        EditorGUILayout.PropertyField(m_WalkGroundLayer, new GUIContent("Слой поверхности"));
        EditorGUILayout.PropertyField(m_GroundCheckPos, new GUIContent("Ground check pos."));
        EditorGUILayout.PropertyField(m_GroundCheckRadius, new GUIContent("Ground check radius"));
        EditorGUILayout.PropertyField(m_GrabWallDistance, new GUIContent("Wall grab distance"));
        EditorGUILayout.PropertyField(m_GrabGroundDistance, new GUIContent("Ground grab distance"));
        EditorGUILayout.PropertyField(m_RayGroundLength, new GUIContent("Ground ray length"));
        
        // AddHeader("Компоненты");
        
        EditorGUILayout.PropertyField(m_Sprite);
        EditorGUILayout.PropertyField(m_Animator);
        EditorGUILayout.PropertyField(m_CollTransform);
        EditorGUILayout.PropertyField(m_PlatformerCollTransform);
        
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        m_IsDebug = EditorGUILayout.Toggle("Отладка", m_IsDebug);
        
        if (m_IsDebug)
        {
            var drawTarget = (InsectController) target;
            EditorGUILayout.LabelField("transform.up: " + drawTarget.transform.up);
            EditorGUILayout.LabelField("Сторона: " + drawTarget.chSide);
            EditorGUILayout.LabelField("Состояние: " + drawTarget.State);
            EditorGUILayout.LabelField("На земле: " + drawTarget.isGround);
        }
    }
}
