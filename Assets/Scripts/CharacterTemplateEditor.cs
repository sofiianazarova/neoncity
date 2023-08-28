using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

[CustomEditor(typeof(CharacterTemplate))]
public class CharacterTemplateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CharacterTemplate template = (CharacterTemplate)target;

        if (GUILayout.Button("Select folder"))
        {
            var folderPath = EditorUtility.OpenFolderPanel("texture folder", "folder", "name");
            TrySetTextureFromFolder(folderPath);
        }

        DrawTexturePaths();
    }

    void TrySetTextureFromFolder(string folderPath)
    {
        CharacterTemplate template = (CharacterTemplate)target;
        SetTexture(ref template.IdleAnimation.Texture, folderPath, "Idle");
        SetTexture(ref template.WalkAnimation.Texture, folderPath, "Walk");
        SetTexture(ref template.RunAnimation.Texture, folderPath, "Run");
        SetTexture(ref template.JumpAnimation.Texture, folderPath, "Jump");

        EditorUtility.SetDirty(template);
        AssetDatabase.SaveAssets();
    }

    void SetTexture(ref Texture texture, string folderPath, string name)
    {
        var path = Path.Combine(folderPath, name + ".png");
        path = "Assets" + path.Substring(Application.dataPath.Length);
        texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
    }

    void DrawTexturePaths()
    {
        CharacterTemplate template = (CharacterTemplate)target;
        DrawTexturePath(template.IdleAnimation.Texture);
        DrawTexturePath(template.WalkAnimation.Texture);
        DrawTexturePath(template.RunAnimation.Texture);
        DrawTexturePath(template.JumpAnimation.Texture);
    }

    void DrawTexturePath(Texture texture)
    {
        GUILayout.Label(AssetDatabase.GetAssetPath(texture));
    }
}