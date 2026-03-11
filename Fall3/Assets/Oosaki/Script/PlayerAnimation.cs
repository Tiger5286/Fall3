using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    PlayerController _controller;
    Animator _animator;

    //歩く速度
    float kWalkSpeed = 0.1f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<PlayerController>();
    }

    void Start()
    {
        //_animator = GetComponent<Animator>();
    }

    public void SetMoveSpeed(float speed)
    {
        //移動速度が0.1未満なら0にする
        if (speed < kWalkSpeed)
        {
            speed = 0;
        }
        _animator.SetFloat("Speed", speed);
    }

    //攻撃アニメーションを再生する
    public void PlayAnimAttack()
    {
        _animator.SetTrigger("Attack");
    }

    //ジャンプアニメーションを再生する
    public void PlayAnimJump()
    {
        _animator.SetTrigger("Jump");
    }

    public void EndAttack()
    {
        _controller.EndAttack();
    }

}