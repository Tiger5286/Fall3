using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public readonly struct PlayerInfo
{
    public readonly PlayerInput _input;
    public readonly PlayerController _controller;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="input"></param>
    /// <param name="controller"></param>
    public PlayerInfo(PlayerInput input, PlayerController controller)
    {
        _input = input; 
        _controller = controller;
    }
}

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
    public event Action<int> OnAttackInput; // 攻撃入力が行われたときに呼ぶイベント
    
    // 通常イベント群
    public event Action<int> OnPlayerDied; // プレイヤーが死んだ時に呼ぶイベント

    // プレイヤーの入力を管理するためのリスト
    // このマネージャー内でのみ使用する
    // JoinManagerがプレイヤーを生成したときにこのリストにPlayerInputを登録する
    private readonly List<PlayerInput> _playerInputs = new();

    private readonly List<PlayerController> _playerControllers = new();

    public List<PlayerInfo> _players = new();

    private class ActionHandles
    {
        public InputAction _move;
        public InputAction _jump;
        public InputAction _attack;

        public Action<InputAction.CallbackContext> _onMovePerformed;
        public Action<InputAction.CallbackContext> _onMoveCanceled;
        public Action<InputAction.CallbackContext> _onJumpPerformed;
        public Action<InputAction.CallbackContext> _onAttackPerformed;
    }

    private readonly Dictionary<PlayerInput, ActionHandles> _actions = new();

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
        if (_playerInputs.Contains(playerInput) || _actions.ContainsKey(playerInput))
        {
            return;
        }
        // プレイヤーの入力データをリストに追加する
        _playerInputs.Add(playerInput);

        var controller = playerInput.GetComponent<PlayerController>();
        _playerControllers.Add(controller);

        var info = new PlayerInfo(playerInput, controller);
        _players.Add(info);

        // プレイヤーの番号を取得
        int idx = playerInput.playerIndex;

        // 入力アクションを取得
        var actions = playerInput.actions; // PlayerInputのアクション全体
        var moveAction = actions["Move"]; // Moveアクションを取得
        var jumpAction = actions["Jump"]; // Jumpアクションを取得
        var attackAction = actions["Attack"]; // Attackアクションを取得

        var handle = new ActionHandles
        {
            _move = moveAction,
            _jump = jumpAction,
            _attack = attackAction,
            _onMovePerformed = ctx =>
            {
                var value = ctx.ReadValue<Vector2>();
                _move[idx] = value;
                OnMoveInput?.Invoke(idx, value);
            },
            _onMoveCanceled = ctx =>
            {
                _move[idx] = Vector2.zero;
                OnMoveInput?.Invoke(idx, Vector2.zero);
            },
            _onJumpPerformed = ctx =>
            {
                OnJumpInput?.Invoke(idx);
                Debug.Log($"[InputManager] JumpInput: idx={idx}");
            },
            _onAttackPerformed = ctx =>
            {
                OnAttackInput?.Invoke(idx);
                Debug.Log($"[InputManager] AttackInput: idx={idx}");
            }
        };

        if(moveAction != null )
        {
            moveAction.performed += handle._onMovePerformed;
            moveAction.canceled += handle._onMoveCanceled;
        }
        if(jumpAction != null)
        {
            jumpAction.performed += handle._onJumpPerformed;
        }
        if (attackAction != null)
        {
            attackAction.performed += handle._onAttackPerformed;
        }

        _actions[playerInput] = handle;

        // プレイヤーの移動の入力を初期化
        _move[idx] = Vector2.zero;

        // 入力アクションにイベントを登録する
        // performedは入力が行われたとき、canceledは入力がキャンセルされたときに呼ばれるイベント
        //moveAction.performed += ctx =>
        //{
        //    var value = ctx.ReadValue<Vector2>();
        //    _move[idx] = value;
        //    OnMoveInput?.Invoke(idx, value);
        //};

        //moveAction.canceled += ctx =>
        //{
        //    _move[idx] = Vector2.zero;
        //    OnMoveInput?.Invoke(idx, Vector2.zero);
        //};

        //jumpAction.performed += ctx =>
        //{
        //    OnJumpInput?.Invoke(idx);
        //    Debug.Log($"[InputManager] JumpInput: idx={idx}");
        //};

        //attackAction.performed += ctx =>
        //{
        //    OnAttackInput?.Invoke(idx);
        //    Debug.Log($"[InputManager] AttackInput: idx={idx}");
        //};

        SetAllPlayerControl(false);
    }

    /// <summary>
    /// 移動アクションを直接取得するための関数
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public Vector2 GetMoveInput(int playerIndex) => 
        _move.TryGetValue(playerIndex, out var value) ? value : Vector2.zero;

    public void UnRegisterPlayer(PlayerInput playerInput)
    {
        // プレイヤーのインプットが存在しない場合処理をしない
        if (playerInput == null) return;

        // プレイヤーのインプットと合致するプレイヤーリストの番号を取得
        int idx = _players.FindIndex(player => player._input == playerInput);

        if (idx >= 0)
        {
            var info = _players[idx];
            _players.RemoveAt(idx);
        }

        _playerInputs.Remove(playerInput);


        var controller = playerInput.GetComponent<PlayerController>();
        if (controller != null)
        {
            _playerControllers.Remove(controller);
        }

        // 登録されているActionを取得
        if(_actions.TryGetValue(playerInput, out var action))
        {
            if(action._move != null)
            {
                action._move.performed -=  action._onMovePerformed;
                action._move.canceled -= action._onMoveCanceled;
            }
            if (action._jump != null)
            {
                action._jump.performed -= action._onJumpPerformed;
            }
            if(action._attack != null)
            {
                action._attack.performed -= action._onAttackPerformed;
            }
            _actions.Remove(playerInput);
        }

        _move.Remove(playerInput.playerIndex);
    }

    /// <summary>
    /// すべてのプレイヤーのコントローラーの入力状態を管理する
    /// </summary>
    /// <param name="isEnable">true : 入力できる false : 入力できない</param>
    public void SetAllPlayerControl(bool isEnable)
    {
        // インプットマネージャーが持っているプレイヤーコントローラーすべてに
        // 入力可能かどうかをセットする
        foreach (var controller in _playerControllers)
        {
            // プレイヤーの操作可能状態を変更する
            controller.SetInputActive(isEnable);
        }
    }

    public void ReportPlayerDied(int playerIndex)
    {
        Debug.Log($"[InputManager] player{playerIndex} is Die.");
        OnPlayerDied?.Invoke(playerIndex);
    }

    public void InitPlayers()
    {
        foreach(var info in _players)
        {
            info._controller.Init();
        }
    }
}
