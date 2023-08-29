using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create DoubleSideMesh"))
        {
            Character character = (Character)target;

            var mesh = GenerateDoubleSideMesh();
            character.transform.Find("SpriteQuad").GetComponent<MeshFilter>().mesh = mesh;
            character.transform.Find("ShadowQuad").GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    Mesh GenerateDoubleSideMesh()
    {
        // 1 +-----+ 2
        //   |   / |
        //   |  /  |
        //   | /   |
        // 0 +-----+ 3 

        var mesh = new Mesh();
        mesh.vertices = new Vector3[] 
        {
            new Vector3(-0.5f, 0, 0),
            new Vector3(-0.5f, 1, 0),
            new Vector3(0.5f, 1, 0),
            new Vector3(0.5f, 0, 0)
        };

        mesh.uv = new Vector2[] 
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };

        mesh.triangles = new int[]
        {
            0, 1, 2, // CW
            0, 2, 3, // CW
            0, 2, 1, // CCW
            0, 3, 2  // CCW
        };

        return mesh;
    }
}