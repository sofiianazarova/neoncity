using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterTemplate))]
public class CharacterTemplateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CharacterTemplate template = (CharacterTemplate)target;

        EditAnimationData("Idle", ref template.IdleAnimation);
        EditAnimationData("Walk", ref template.WalkAnimation);
        EditAnimationData("Run", ref template.RunAnimation);
        EditAnimationData("Jump", ref template.JumpAnimation);
    }

    void EditAnimationData(string name, ref AnimationData data)
    {
        GUILayout.Label(name);
        data.Texture = EditorGUILayout.ObjectField("Texture", data.Texture, typeof(Texture), data.Texture) as Texture;
        data.FrameRate = EditorGUILayout.FloatField("FrameRate", data.FrameRate);
        data.Speed = EditorGUILayout.FloatField("Speed", data.Speed);
    }
}