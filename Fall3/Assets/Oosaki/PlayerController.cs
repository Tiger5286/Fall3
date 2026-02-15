using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //プレイヤーのアニメーション
    PlayerAnimation _playerAnimation;
    //プレイヤーの移動速度
    float _moveSpeed = 3f;
    //プレイヤーが攻撃中かどうか
    bool _isAttacking = false;

    void Start()
    {
        //プレイヤーのアニメーションを取得している
        _playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        Attack();
        Move();

    }

    void Move()
    {
        //攻撃しているときは攻撃できない
        if (_isAttacking) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        //正規化
        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        //移動
        transform.position += move * _moveSpeed * Time.deltaTime;

        //プレイヤーが見ている向きに変える
        if (move!=Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }
        _playerAnimation.SetMoveSpeed(move.magnitude);
        Debug.Log(move.magnitude);
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isAttacking)
        {
            _isAttacking = true;
            _playerAnimation.PlayAnimAttack();
        }
    }
    //攻撃終了を知らせる関数
    public void EndAttack()
    {
        _isAttacking = false;
    }
}