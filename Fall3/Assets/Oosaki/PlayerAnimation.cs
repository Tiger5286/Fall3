using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    float moveSpeed = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        WalkAnimation();
        AttackAnimation();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        if(move.magnitude > 1)
        {
            move.Normalize();
        }

        //ˆÚ“®
        transform.position += move * moveSpeed * Time.deltaTime;

        //Œü‚«‚ð•Ï‚¦‚é
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }
    }

    void WalkAnimation()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float speed = new Vector2(h, v).magnitude;

        if (speed < 0.1f)
        {
            speed = 0;
        }

        animator.SetFloat("Speed", speed);
    }

    void AttackAnimation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack");
        }
    }
}
