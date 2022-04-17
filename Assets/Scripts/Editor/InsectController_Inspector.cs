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
    private SerializedProperty m_GroundLayerName;
    private SerializedProperty m_GroundCheckPos;
    private SerializedProperty m_GroundCheckRadius;
    private SerializedProperty m_GrabWallDistance;
    private SerializedProperty m_GrabGroundDistance;
    private SerializedProperty m_RayGroundLength;
    private SerializedProperty m_Sprite;
    private SerializedProperty m_Animator;
    private SerializedProperty m_CollTransform;
    private SerializedProperty m_PlatformerCollTransform;

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
       
        m_GroundLayerName = serializedObject.FindProperty("groundLayerName");
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

    public override void OnInspectorGUI()
    {
        AddHeader("Режим ходьбы", false); 
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
        
        AddHeader("Другое");
        EditorGUILayout.PropertyField(m_GroundLayerName, new GUIContent("Название слоя поверхности"));
        EditorGUILayout.PropertyField(m_GroundCheckPos, new GUIContent("Позиция проверки поверхности"));
        EditorGUILayout.PropertyField(m_GroundCheckRadius, new GUIContent("Радиус проверки поверхности"));
        EditorGUILayout.PropertyField(m_GrabWallDistance, new GUIContent("Расстояние прицепления к стен"));
        EditorGUILayout.PropertyField(m_GrabGroundDistance, new GUIContent("Расстояние прицепления к земле"));
        EditorGUILayout.PropertyField(m_RayGroundLength, new GUIContent("Длина луча к земле (поворот)"));
        
        AddHeader("Компоненты");
        EditorGUILayout.PropertyField(m_Sprite);
        EditorGUILayout.PropertyField(m_Animator);
        EditorGUILayout.PropertyField(m_CollTransform);
        EditorGUILayout.PropertyField(m_PlatformerCollTransform);
        
        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    }
}
