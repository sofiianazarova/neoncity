using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using UnityEngine.Splines;
using Spline = UnityEngine.Splines.Spline;

public class Character : MonoBehaviour
{
    public float WalkSpeed = 2f;
    public float RunSpeed = 5f;

    public Texture IdleTexture;
    public Texture WalkTexture;
    public Texture RunTexture;
    public Texture JumpTexture;

    public float roadPosition = 0;
    public float roadZOffset = 0;
    public SplineContainer Road;

    private Vector3 ToVec(float3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }

    public MeshRenderer ShadowRenderer;
    public MeshRenderer SpriteRenderer;

    void Update()
    {
        WalkUpdate();
        PinToSpline();
        AnimationUpdate();
    }

    private float Frame = 0;
    private float FrameRate = 3;
    private Texture CurrentTexture;
    private float Flipped = 1.0f;

    private void Start()
    {
        CurrentTexture = IdleTexture;
    }

    void AnimationUpdate()
    {
        var frameCount = CurrentTexture.width / CurrentTexture.height;
        Frame += FrameRate * Time.deltaTime;
        Frame %= frameCount;
        SetTextureScale(1.0f / frameCount * Flipped, 1.0f);
        SetTextureOffset(1.0f / frameCount * Mathf.FloorToInt(Frame), 0f);
    }

    void SetTextureScale(float x, float y)
    {
        var value = new Vector2(x, y);
        SpriteRenderer.material.mainTextureScale = value;
        ShadowRenderer.material.mainTextureScale = value;
    }

    void SetTextureOffset(float x, float y)
    {
        var value = new Vector2(x, y);
        SpriteRenderer.material.mainTextureOffset = value;
        ShadowRenderer.material.mainTextureOffset = value;
    }

    void WalkUpdate()
    {
        var offset = 0;
        var zOffset = 0;
        FrameRate = 3;

        CurrentTexture = IdleTexture;

        if (Input.GetKey(KeyCode.D))
        {
            offset += 1;
            Flipped = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            offset -= 1;
            Flipped = -1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            zOffset += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            zOffset -= 1;
        }

        var speed = WalkSpeed;
        if (offset != 0 || zOffset != 0)
        {
            CurrentTexture = WalkTexture;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = RunSpeed;
                CurrentTexture = RunTexture;
                FrameRate = 6;
            }
        }
        roadPosition += offset * speed * Time.deltaTime;
        roadZOffset += zOffset * speed * Time.deltaTime;
        roadZOffset = Mathf.Clamp(roadZOffset, -1f, 1f);

        SpriteRenderer.material.mainTexture = CurrentTexture;
        ShadowRenderer.material.mainTexture = CurrentTexture;
        ShadowRenderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
        SpriteRenderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
        ShadowRenderer.material.mainTexture.filterMode = FilterMode.Point;
        SpriteRenderer.material.mainTexture.filterMode = FilterMode.Point;

    }

    void PinToSpline()
    {
        var k = roadPosition / Road.Spline.CalculateLength(Matrix4x4.identity);
        Road.Spline.Evaluate(k, out float3 position, out float3 tangent, out float3 up);

        Vector3 pos = Road.transform.position + ToVec(position);
        transform.position = pos;
        transform.right = tangent;

        SpriteRenderer.transform.localPosition = new Vector3(0, 0.5f, roadZOffset);
        ShadowRenderer.transform.localPosition = new Vector3(0, 0.5f, roadZOffset);

        //transform.position += transform.forward * roadZOffset;
        //transform.up = up;
    }

    public void BuildMesh()
    {
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

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        //ShadowMesh.mesh = mesh;
        //SpriteMesh.mesh = mesh;
    }

}
