using UnityEngine;
using UnityEditor;
using Controllers;

[CustomEditor(typeof(InsectController))]
public class InsectController_Inspector : Editor
{
    private InsectController _drawTarget;

    private void OnEnable()
    {
        _drawTarget = (InsectController) target;
    }

    private void AddHeader(string label, bool space = true)
    {
        if (space) EditorGUILayout.Space();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
    }

    public override void OnInspectorGUI()
    {
        AddHeader("Режим ходьбы", false);
        
        _drawTarget.walkSpeed = EditorGUILayout.FloatField("Скорость ходьбы", _drawTarget.walkSpeed);
        _drawTarget.runSpeed = EditorGUILayout.FloatField("Скорость бега", _drawTarget.runSpeed);
        _drawTarget.stickOffsetBeforeRun = EditorGUILayout.Slider("Отклон стика",_drawTarget.stickOffsetBeforeRun, 0.01f, 1f);
        EditorGUILayout.Space();

        _drawTarget.isCanJump = EditorGUILayout.Toggle("Может ли прыгать?", _drawTarget.isCanJump);
        _drawTarget.jumpPower = EditorGUILayout.FloatField("Сила прыжка", _drawTarget.jumpPower);
        _drawTarget.hangTime = EditorGUILayout.FloatField("Прыжок Койота", _drawTarget.hangTime);
        _drawTarget.jumpBufferTime = EditorGUILayout.FloatField("Буфер прыжка", _drawTarget.jumpBufferTime);
        
        AddHeader("Режим ползанья");
        _drawTarget.crawlSpeed = EditorGUILayout.FloatField("Скорость ползанья", _drawTarget.crawlSpeed);
        _drawTarget.rotationSpeed = EditorGUILayout.FloatField("Скорость поворота", _drawTarget.rotationSpeed);
        _drawTarget.jumpCrawlPower = EditorGUILayout.FloatField("Сила прыжка", _drawTarget.jumpCrawlPower);
        _drawTarget.jumpCrawlFromWallPower = EditorGUILayout.FloatField("Сила прыжка от стены", _drawTarget.jumpCrawlFromWallPower);

        AddHeader("Другое");
        _drawTarget.groundLayerName = EditorGUILayout.TextField("Слой поверхности", _drawTarget.groundLayerName);
        _drawTarget.groundCheckPos = EditorGUILayout.Vector2Field("Позиция проверки поверхности", _drawTarget.groundCheckPos);
        _drawTarget.groundCheckRadius = EditorGUILayout.FloatField("Радиус проверки поверхности", _drawTarget.groundCheckRadius);
        _drawTarget.grabWallDistance = EditorGUILayout.FloatField("Расстояние прицепления к стене", _drawTarget.grabWallDistance);
        _drawTarget.grabGroundDistance = EditorGUILayout.FloatField("Расстояние прицепления к земле", _drawTarget.grabGroundDistance);
        _drawTarget.rayGroundLength = EditorGUILayout.FloatField("Длина луча к земле (поворот)",_drawTarget.rayGroundLength);


        AddHeader("Компоненты");
        _drawTarget.animator = (Animator) EditorGUILayout.ObjectField("Animator", _drawTarget.animator, typeof(Animator));
        _drawTarget.sprite = (SpriteRenderer) EditorGUILayout.ObjectField("Sprite", _drawTarget.sprite, typeof(SpriteRenderer));
        _drawTarget.collTransform = (Transform) EditorGUILayout.ObjectField("Collider Transform", _drawTarget.collTransform, typeof(Transform));
        
        AddHeader("DEBUG");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EnumPopup(_drawTarget.state);
        EditorGUILayout.EnumPopup(_drawTarget.chSide);
        EditorGUILayout.EndHorizontal();

        //base.OnInspectorGUI();
    }
}
