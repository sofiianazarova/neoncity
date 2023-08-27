using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //if (GUILayout.Button("Generate"))
        //{
        //    Character character = (Character)target;
        //    character.BuildMesh();
        //}
    }
}