using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : MonoBehaviour
{
    public float WalkSpeed = 2f;
    public float RunSpeed = 5f;

    public AnimatorController Idle;
    public AnimatorController Walk;
    public AnimatorController Run;
    public AnimatorController Jump;


    void Update()
    {
        var offset = new Vector3();

        var currentAnimation = Idle;



        if (Input.GetKey(KeyCode.D))
        {
            offset.x += 1;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if (Input.GetKey(KeyCode.A))
        {
            offset.x -= 1;
            GetComponent<SpriteRenderer>().flipX = true;
        }

        var speed = WalkSpeed;
        if (offset != Vector3.zero)
        {
            currentAnimation = Walk;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = RunSpeed;
                currentAnimation = Run;
            }
        }
        transform.position += offset * speed * Time.deltaTime;


        GetComponent<Animator>().runtimeAnimatorController = currentAnimation;
    }
}
