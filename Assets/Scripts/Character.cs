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
using System;
using System.Linq;

public class Character : MonoBehaviour
{
    public CharacterTemplate Template;
    private List<SplineContainer> Roads;

    public MeshRenderer ShadowRenderer;
    public MeshRenderer SpriteRenderer;
    public float roadPosition = 0;
    public float roadZOffset = 0;
   
    private AnimationData Animation;
    private float Frame = 0;
    private float Flipped = 1.0f;
    public float Offset = 0;
    public float ZOffset = 0;

    public Transform Pin;

    private void Start()
    {
        Animation = Template.IdleAnimation;
        Roads = GameObject.FindObjectsOfType<SplineContainer>().ToList();
    }

    void Update()
    {
        WalkUpdate();
        AnimationUpdate();
        PinToSpline();
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

        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.D))
        {
            direction += transform.right;
            Flipped = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction -= transform.right;
            Flipped = -1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            direction += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction -= transform.forward;
        }

        if (direction != Vector3.zero)
        {
            Animation = Template.WalkAnimation;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Animation = Template.RunAnimation;
            }
        }

        transform.GetComponent<Rigidbody>().velocity = direction.normalized * Animation.Speed;


        SpriteRenderer.material.mainTexture = Animation.Texture;
        ShadowRenderer.material.mainTexture = Animation.Texture;
        ShadowRenderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
        SpriteRenderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
        ShadowRenderer.material.mainTexture.filterMode = FilterMode.Point;
        SpriteRenderer.material.mainTexture.filterMode = FilterMode.Point;
    }

    void PinToSpline()
    {
        var anchors = new List<SplineAnchor>();
        foreach (var road in Roads)
        {
            anchors.Add(GetSplineAnchor(road));
        }


        var direction = Vector3.zero;
        foreach (var anchor in anchors)
        {
            var w = 1.0f / anchor.Distance;
            direction += anchor.Direction * w;
        }

        transform.forward = direction;
    }

    SplineAnchor GetSplineAnchor(SplineContainer road)
    {
        var roadOffset = road.transform.position;
        SplineUtility.GetNearestPoint(road.Spline, transform.position - roadOffset, out float3 nearest, out float t, 20, 4);
        road.Spline.Evaluate(t, out nearest, out float3 tangent, out float3 up);

        var pinPosition = roadOffset + ToVec(nearest);
        Pin.transform.position = roadOffset + ToVec(nearest);

        var distance = (pinPosition - transform.position).magnitude;
        var direction = new Vector3(-tangent.z, 0, tangent.x).normalized;
        return new SplineAnchor { Distance = distance, Direction = direction };   
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
