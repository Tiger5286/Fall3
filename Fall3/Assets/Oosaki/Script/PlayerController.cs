using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform _groundCheckPoint; //足元位置
    [SerializeField] float _groundCheckDistance = 1.0f;
    [SerializeField] LayerMask _groundLayer;

    [SerializeField] float _FallLimitY = -10f;

    [Header("死亡時のエフェクト")]
    [SerializeField] GameObject _effectPrefab;

    GameObject _effectInstance;

    float _effectScale = 1.0f;

    //プレイヤーのアニメーション
    PlayerAnimation _playerAnimation;

    //プレイヤーの攻撃生成
    AttackSpawner _attackSpawner;

    //プレイヤーのRigidbody
    Rigidbody _rigidbody;

    //ステージ管理
    StageManager _stageManager;

    // 入力とインプットの状況を取得する
    PlayerLink _playerLink;

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

    //前のフレームでプレイヤーが地面にいたかどうか
    bool _wasGround = true;

    //ノックバックしているかどうか
    bool _isKnockBack = false;

    //プレイヤーが死んでいるかどうか
    bool _isDead = false;

    //プレイヤーの識別番号
    int _playerIndex;

    //プレイヤーの識別番号改善版
    PlayerId _playerId;

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
            // PlayerInputを持つ親オブジェクトを取得
            _playerLink = GetComponentInParent<PlayerLink>();
        }
        //-------コントローラー関連の処理-------

        _attackSpawner = GetComponent<AttackSpawner>();

        _stageManager = FindObjectOfType<StageManager>();
    }

    void Start()
    {
        //-------コントローラー関連の処理-------
        {
            // プレイヤーの入力管理を取得している
            _playerInput = _playerLink._playerInput;

            // コントローラーの入力を受け付けないようにする
            _playerInput.SwitchCurrentActionMap("Disable");

            //プレイヤーのインデックスを取得している
            _playerIndex = _playerInput.playerIndex;

        }
        //-------コントローラー関連の処理-------

        Init();
    }

    void Update()
    {
        Debug.Log("Update動いてる");

        //デバッグ
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            transform.position = new Vector3(
                transform.position.x,
                -20,
                transform.position.z);
        }

        bool currentGround = Physics.Raycast(
            _groundCheckPoint.position,
            Vector3.down,
            _groundCheckDistance,
            _groundLayer);

        //地面接地情報の更新
        _wasGround = _isGround;
        _isGround = currentGround;

        //着地した瞬間
        if (_isGround)
        {
            CheckFallStage();
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
            if (_isDead)
            {
                return;
            }
            _isDead = true;

            _playerInput.SwitchCurrentActionMap("Disable");

            JoinManager.Instance.OnPlayerDeath(_playerId);

            InputManager.Instance.ReportPlayerDied(_playerIndex);

            _effectInstance = Instantiate(_effectPrefab,transform.position,transform.rotation);
            _effectInstance.transform.localScale = Vector3.one * _effectScale;
            Debug.Log("消えたーーーーーーーー");
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        if (_playerIndex == 0)
        {
            transform.position = new Vector3(1.0f, 2.0f, 4.0f);
            transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }
        else
        {
            transform.position = new Vector3(8.0f, 2.0f, 4.0f);
            transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        }
    }

    /// <summary>
    /// プレイヤーの入力状態を変更する
    /// </summary>
    /// <param name="isEnable">入力できるかどうか</param>
    public void SetInputActive(bool isEnable)
    {
        // プレイヤーの入力管理が取得出来ている場合
        if (_playerInput != null)
        {
            _playerInput.SwitchCurrentActionMap(isEnable ? "GameInput" : "Disable");
        }
    }

    //移動処理
    void Move(int idx, Vector2 moveValue)
    {
        //プレイヤーのインデックスと入力されたインデックスが違うときは処理しない
        if (idx != (int)_playerId) return;

        float h = moveValue.x;
        float v = moveValue.y;

        _move = new Vector3(h, 0, v);

        // 移動中なら歩行SEを再生、停止したら止める
        if (_move.magnitude > 0.1f)
        {
            SoundManager.Instance.PlayWalkSe();
        }
        else
        {
            SoundManager.Instance.StopWalkSe();
        }
    }

    //ジャンプ処理
    void Jump(int idx)
    {
        //プレイヤーのインデックスと入力されたインデックスが違うときは処理しない
        if (idx != (int)_playerId) return;

        if (_isGround)
        {
            //ジャンプの高さを設定
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x,
                _jumpPower,
                _rigidbody.velocity.z);

            _isGround = false;
            _playerAnimation.PlayAnimJump();
            //ジャンプSE再生
            SoundManager.Instance.PlaySe(1);
        }
    }

    //攻撃処理
    void Attack(int idx)
    {
        //プレイヤーのインデックスと入力されたインデックスが違うときは処理しない
        if (idx != (int)_playerId) return;

        if (_isAttacking) return;

        //ジャンプ中は攻撃できないようにする
        if (!_isGround) return;

        //ノックバック中は攻撃できないようにする
        if (_isKnockBack) return;

        _attackSpawner.SpawnBall();
        _isAttacking = true;
        _playerAnimation.PlayAnimAttack();
        //攻撃SE再生
        SoundManager.Instance.PlaySe(2);
    }

    //攻撃終了を知らせる関数
    public void EndAttack()
    {
        Debug.Log("EndAttack呼ばれた");
        _isAttacking = false;
    }

    void CheckFallStage()
    {
        Ray ray = new Ray(_groundCheckPoint.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            Stage stage = hit.collider.GetComponent<Stage>();

            if (stage != null)
            {
                stage.Fall();
            }
        }
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

    public void SetPlayerId(PlayerId id)
    {
        _playerId = id;
    }

    public void ApplyKnockBack(Vector3 force)
    {
        StartCoroutine(KnockbackRoutine(force));
    }

    IEnumerator KnockbackRoutine(Vector3 force)
    {
        _isKnockBack = true;

        _rigidbody.velocity = Vector3.zero; //現在の速度をリセット
        _rigidbody.AddForce(force, ForceMode.Impulse);

        //ノックバックの時間
        yield return new WaitForSeconds(0.5f);

        _isKnockBack = false;
    }

}