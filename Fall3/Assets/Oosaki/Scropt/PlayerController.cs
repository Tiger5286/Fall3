using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform _groundCheckPoint; //足元位置
    [SerializeField] float _groundCheckDistance = 0.3f;
    [SerializeField] LayerMask _groundLayer;

    [SerializeField] float _FallLimitY = -10f;

    //プレイヤーのアニメーション
    PlayerAnimation _playerAnimation;

    //プレイヤーの攻撃生成
    AttackSpawner _attackSpawner;

    //プレイヤーのRigidbody
    Rigidbody _rigidbody;

    //ステージ管理
    StageManager _stageManager;

    //インプット
    PlayerInput _playerInput;

    //プレイヤーの位置
    Vector3 _pos;

    //プレイヤーの移動
    Vector3 _move;

    //プレイヤーの移動速度
    float _moveSpeed = 3.0f;

    //プレイヤーのジャンプの高さ
    float _jumpPower = 5.0f;

    //プレイヤーが攻撃中かどうか
    bool _isAttacking = false;

    //プレイヤーが地面にいるかどうか
    bool _isGround = true;

    //ノックバックしているかどうか
    bool _isKnockBack = false;

    //プレイヤーが死んでいるかどうか
    bool _isDead = false;

    //プレイヤーの識別番号
    int _playerIndex;

    //現在のグリッド座標
    Vector3Int _currentGrid = new Vector3Int(-1, -1, -1);

    private void Awake()
    {
        //プレイヤーのアニメーションを取得している
        _playerAnimation = GetComponent<PlayerAnimation>();

        //プレイヤーのRigidbodyを取得している
        _rigidbody = GetComponent<Rigidbody>();

        //-------コントローラー関連の処理-------
        {
            // プレイヤーの入力管理を取得している
            _playerInput = GetComponent<PlayerInput>();

            // コントローラーの入力を受け付けないようにする
            _playerInput.SwitchCurrentActionMap("GameInput");

            //プレイヤーのインデックスを取得している
            _playerIndex = _playerInput.playerIndex;
        }
        //-------コントローラー関連の処理-------

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

        CheckFallStage();

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
        if (!_isKnockBack)
        {
            //transform.position += move * _moveSpeed * Time.deltaTime;
            Vector3 velocity = _move * _moveSpeed;
            velocity.y = _rigidbody.velocity.y;
            _rigidbody.velocity = velocity;
        }

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

        //落下処理
        if (transform.position.y < _FallLimitY)
        {
            //_isDead = true;
            _playerInput.SwitchCurrentActionMap("Disable");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// プレイヤーの入力状態を変更する
    /// </summary>
    /// <param name="isEnable">入力できるかどうか</param>
    public void SetInputActive(bool isEnable)
    {
        // プレイヤーの入力管理が取得出来ている場合
        if(_playerInput != null)
        {
            _playerInput.SwitchCurrentActionMap(isEnable ? "GameInput" : "Disable");
        }
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

        //ジャンプ中は攻撃できないようにする
        if (!_isGround) return;

        //ノックバック中は攻撃できないようにする
        if (_isKnockBack) return;

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

    void CheckFallStage()
    {
        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.FloorToInt(transform.position.y);
        int z = Mathf.FloorToInt(transform.position.z);

        Vector3Int newGrid = new Vector3Int(x, y, z);

        if (newGrid == _currentGrid) return;

        _currentGrid = newGrid;
        _stageManager.FallStage(x, -y, z);
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

    public void ApplyKnockBack(Vector3 force)
    {
        StartCoroutine(KnockbackRoutine(force));
    }

    IEnumerator KnockbackRoutine(Vector3 force)
    {
        _isKnockBack = true;

        _rigidbody.velocity=Vector3.zero; //現在の速度をリセット
        _rigidbody.AddForce(force, ForceMode.Impulse);

        //ノックバックの時間
        yield return new WaitForSeconds(0.5f);

        _isKnockBack = false;
    }

}