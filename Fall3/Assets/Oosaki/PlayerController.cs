using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform _groundCheckPoint; //足元位置
    [SerializeField] float _groundCheckDistance = 0.3f;
    [SerializeField] LayerMask _groundLayer;

    //プレイヤーのアニメーション
    PlayerAnimation _playerAnimation;

    AttackSpawner _attackSpawner;

    Rigidbody _rigidbody;

    StageManager _stageManager;

    Vector3 _pos;

    Vector3 _move;

    //プレイヤーの移動速度
    float _moveSpeed = 3.0f;

    //プレイヤーのジャンプの高さ
    float _jumpPower = 5.0f;

    //プレイヤーが攻撃中かどうか
    bool _isAttacking = false;

    bool _isGround = true;

    int _playerIndex;

    private void Awake()
    {
        //プレイヤーのアニメーションを取得している
        _playerAnimation = GetComponent<PlayerAnimation>();

        //プレイヤーのRigidbodyを取得している
        _rigidbody = GetComponent<Rigidbody>();

        //プレイヤーのインデックスを取得している
        _playerIndex = GetComponent<PlayerInput>().playerIndex;

        _attackSpawner = GetComponent<AttackSpawner>();

        _stageManager = FindObjectOfType<StageManager>();
    }

    void Start()
    {
        if (_playerIndex == 0)
        {
            transform.position = new Vector3(1.0f, 2.0f, 1.0f);
        }
        else
        {
            transform.position = new Vector3(9.0f, 2.0f, 9.0f);
        }
    }

    void FixedUpdate()
    {
        _isGround = Physics.Raycast(
        _groundCheckPoint.position,
        Vector3.down,
        _groundCheckDistance,
        _groundLayer);

        if (_isAttacking)
        {
            //攻撃中は移動しない
            _move = Vector3.zero;
        }

        //正規化
        if (_move.magnitude > 1)
        {
            _move.Normalize();
        }
        //移動
        //transform.position += move * _moveSpeed * Time.deltaTime;
        Vector3 velocity = _move * _moveSpeed;
        velocity.y = _rigidbody.velocity.y;
        _rigidbody.velocity = velocity;

        //移動の大きさが小さいときは移動しないようにする
        if (_move.sqrMagnitude < 0.01f)
        {
            _move = Vector3.zero;
        }

        //プレイヤーが見ている向きに変える
        if (_move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_move);
        }
        _playerAnimation.SetMoveSpeed(_move.magnitude);
        Debug.Log(_move.magnitude);
    }

    //移動処理
    void Move(int idx, Vector2 moveValue)
    {
        //プレイヤーのインデックスと入力されたインデックスが違うときは処理しない
        if (idx != _playerIndex) return;

        float h = moveValue.x;
        float v = moveValue.y;

        _move = new Vector3(h, 0, v);
    }

    //ジャンプ処理
    void Jump(int idx)
    {
        //プレイヤーのインデックスと入力されたインデックスが違うときは処理しない
        if (idx != _playerIndex) return;

        if (_isGround)
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
    void Attack(int idx)
    {
        //プレイヤーのインデックスと入力されたインデックスが違うときは処理しない
        if (idx != _playerIndex) return;

        if (!_isAttacking)
        {
            _attackSpawner.SpawnBall();
            _isAttacking = true;
            _playerAnimation.PlayAnimAttack();
        }
    }
    //攻撃終了を知らせる関数
    public void EndAttack()
    {
        _isAttacking = false;
    }

    public void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput += Move;
            InputManager.Instance.OnJumpInput += Jump;
            InputManager.Instance.OnAttackInput += Attack;
        }
    }

    public void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= Move;
            InputManager.Instance.OnJumpInput -= Jump;
            InputManager.Instance.OnAttackInput -= Attack;
        }
    }
}