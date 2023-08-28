using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using UnityEngine.Splines;
using Spline = UnityEngine.Splines.Spline;
using UnityEngine.Playables;

public class Character : MonoBehaviour
{
    public CharacterTemplate Template;
    public SplineContainer Road;

    public MeshRenderer ShadowRenderer;
    public MeshRenderer SpriteRenderer;
    public float roadPosition = 0;
    public float roadZOffset = 0;
   
    private AnimationData Animation;
    private float Frame = 0;
    private float Flipped = 1.0f;
    public float Offset = 0;
    public float ZOffset = 0;

    private void Start()
    {
        Animation = Template.IdleAnimation;
    }

    void Update()
    {
        WalkUpdate();
        PinToSpline();
        AnimationUpdate();
    }

    private Vector3 ToVec(float3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }

    void WalkUpdate()
    {
        Offset = 0;
        ZOffset = 0;

        Animation = Template.IdleAnimation;

        if (Input.GetKey(KeyCode.D))
        {
            Offset += 1;
            Flipped = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Offset -= 1;
            Flipped = -1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            ZOffset += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            ZOffset -= 1;
        }

        if (Offset != 0 || ZOffset != 0)
        {
            Animation = Template.WalkAnimation;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Animation = Template.RunAnimation;
            }
        }

        roadPosition += Offset * Animation.Speed * Time.deltaTime;
        roadZOffset += ZOffset * Animation.Speed * Time.deltaTime;
        roadZOffset = Mathf.Clamp(roadZOffset, -1f, 1f);

        SpriteRenderer.material.mainTexture = Animation.Texture;
        ShadowRenderer.material.mainTexture = Animation.Texture;
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
    }


    void AnimationUpdate()
    {
        var frameCount = Animation.Texture.width / Animation.Texture.height;
        Frame += Animation.FrameRate * Time.deltaTime;
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



}
