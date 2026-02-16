using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 入力を管理するシングルトンクラス
/// </summary>
public class InputManager : MonoBehaviour
{
    // インスタンスを取得する
    public static InputManager Instance { get; private set; }

    // プレイヤーごとに移動入力を保存するためのDictionary
    private readonly Dictionary<int, Vector2> _move = new();
    
    // 入力イベント群
    // プレイヤーが生成された際にこのOn○○Inputに関数を登録すると使える
    public event Action<int, Vector2> OnMoveInput; // 移動入力が行われたときに呼ぶイベント
    public event Action<int> OnJumpInput; // ジャンプ入力が行われたときに呼ぶイベント

    // プレイヤーの入力を管理するためのリスト
    // このマネージャー内でのみ使用する
    // JoinManagerがプレイヤーを生成したときにこのリストにPlayerInputを登録する
    private readonly List<PlayerInput> _playerInputs = new();

    /// <summary>
    /// 最初に行う処理
    /// インスタンスを生成して、複製不可にする
    /// </summary>
    private void Awake()
    {
        // すでにインスタンスが存在している場合は、このオブジェクトを破棄する
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // インスタンスを設定
        Instance = this;
        // シーン遷移しても破棄しないようにする
        // ※シーン遷移をしない設定のため必要ないが、念のため入れておく
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// プレイヤーを追加する
    /// </summary>
    /// <param name="playerInput">プレイヤーの入力データ(プレイヤーを区別できるように取得)</param>
    public void RegisterPlayer(PlayerInput playerInput)
    {

        // すでに同じPlayerInputが登録されている場合は、重複して登録しないようにする
        if (_playerInputs.Contains(playerInput))
        {
            return;
        }
        // プレイヤーの入力データをリストに追加する
        _playerInputs.Add(playerInput);


        // プレイヤーの番号を取得
        int idx = playerInput.playerIndex;

        // 入力アクションを取得
        var actions = playerInput.actions; // PlayerInputのアクション全体
        var moveAction = actions["Move"]; // Moveアクションを取得
        var jumpAction = actions["Jump"]; // Jumpアクションを取得

        // プレイヤーの移動の入力を初期化
        _move[idx] = Vector2.zero;

        // 入力アクションにイベントを登録する
        // performedは入力が行われたとき、canceledは入力がキャンセルされたときに呼ばれるイベント
        moveAction.performed += ctx =>
        {
            var value = ctx.ReadValue<Vector2>();
            _move[idx] = value;
            OnMoveInput?.Invoke(idx, value);
        };

        moveAction.canceled += ctx =>
        {
            _move[idx] = Vector2.zero;
            OnMoveInput?.Invoke(idx, Vector2.zero);
        };

        jumpAction.performed += ctx =>
        {
            OnJumpInput?.Invoke(idx);
            Debug.Log($"[InputManager] JumpInput: idx={idx}");
        };
    }

    /// <summary>
    /// 移動アクションを直接取得するための関数
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public Vector2 GetMoveInput(int playerIndex) => 
        _move.TryGetValue(playerIndex, out var value) ? value : Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
