using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //プレイヤーのアニメーション
    PlayerAnimation _playerAnimation;

    Rigidbody _rigidbody;

    Vector3 _pos;

    //プレイヤーの移動速度
    float _moveSpeed = 3.0f;

    //プレイヤーのジャンプの高さ
    float _jumpPower = 5.0f;

    //プレイヤーが攻撃中かどうか
    bool _isAttacking = false;

    bool _isGround = true;

    private void Awake()
    {
        //プレイヤーのアニメーションを取得している
        _playerAnimation = GetComponent<PlayerAnimation>();

        //プレイヤーのRigidbodyを取得している
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {   
        transform.position =new Vector3(1.0f, 0, 1.0f);
    }

    void Update()
    {
        Attack();
    }

    //移動処理
    void Move(int idx,Vector2 moveValue)
    {
        //攻撃しているときは攻撃できない
        if (_isAttacking) return;

        float h = moveValue.x;
        float v = moveValue.y;

        Vector3 move = new Vector3(h, 0, v);

        //正規化
        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        //移動
        //transform.position += move * _moveSpeed * Time.deltaTime;
        Vector3 velocity = move * _moveSpeed;
        velocity.y = _rigidbody.velocity.y;
        _rigidbody.velocity = velocity;

        //移動の大きさが小さいときは移動しないようにする
        if (move.sqrMagnitude < 0.01f)
        {
            move = Vector3.zero;
        }

        //プレイヤーが見ている向きに変える
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }
        _playerAnimation.SetMoveSpeed(move.magnitude);
        Debug.Log(move.magnitude);
    }

    //ジャンプ処理
    void Jump(int idx)
    {
        if(_isGround)
        {
            //ジャンプの高さを設定
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x,
                _jumpPower,
                _rigidbody.velocity.z);

            _isGround = false;
            _playerAnimation.PlayAnimJump();
        }
    }

    //攻撃処理
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGround = true;
        }
    }
    public void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput += Move;
            InputManager.Instance.OnJumpInput += Jump;
        }
    }

    public void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= Move;
            InputManager.Instance.OnJumpInput -= Jump;
        }
    }
}