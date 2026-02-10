using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator _animator;
    //歩く速度
    float kWalkSpeed = 0.1f;
    void Start()
    {
        _animator = GetComponent<Animator>();
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

}
