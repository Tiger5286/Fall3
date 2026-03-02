using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    
    // 入力イベント群(ゲーム内)
    // プレイヤーが生成された際にこのOn○○Inputに関数を登録すると使える
    public event Action<int, Vector2> OnMoveInput; // 移動入力が行われたときに呼ぶイベント
    public event Action<int> OnJumpInput; // ジャンプ入力が行われたときに呼ぶイベント
    public event Action<int> OnAttackInput; // 攻撃入力が行われたときに呼ぶイベント
    
    // 入力イベント群(ゲーム以外)
    // 準備OKとキャンセルのボタンが押された時
    public event Action<int> OnReadyInput;
    public event Action<int> OnCancelInput;

    // 通常イベント群
    public event Action<int> OnPlayerDied; // プレイヤーが死んだ時に呼ぶイベント

    private class ActionHandles
    {
        // GameInput
        public InputAction _move;
        public InputAction _jump;
        public InputAction _attack;
        // Menu
        public InputAction _ready;
        public InputAction _cancel;

        // GameInput
        public Action<InputAction.CallbackContext> _onMovePerformed;
        public Action<InputAction.CallbackContext> _onMoveCanceled;
        public Action<InputAction.CallbackContext> _onJumpPerformed;
        public Action<InputAction.CallbackContext> _onAttackPerformed;

        // Menu
        public Action<InputAction.CallbackContext> _onReady;
        public Action<InputAction.CallbackContext> _onCancel;
    }

    private readonly Dictionary<int, PlayerInput> _slotInputs = new();

    private readonly Dictionary<int, PlayerController> _slotControllers = new();

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

    public void RegisterPlayerToSlot(int slotId, PlayerInput input, PlayerController controller = null)
    {
        if (input == null)
        {
            return;
        }
        if (_slotInputs.ContainsKey(slotId) || _actions.ContainsKey(input))
        {
            return;
        }

        _slotInputs[slotId] = input;

        if (controller != null)
        {
            _slotControllers[slotId] = controller;
        }

        // 入力アクションを取得
        var actions = input.actions; // PlayerInputのアクション全体
        var moveAction = actions["Move"]; // Moveアクションを取得
        var jumpAction = actions["Jump"]; // Jumpアクションを取得
        var attackAction = actions["Attack"]; // Attackアクションを取得

        // インゲーム以外で使用する入力アクションを取得
        var readyAction = actions["Ready"]; // Readyアクションを取得
        var cancelAction = actions["Cancel"]; // Cancelアクションを取得

        var handle = new ActionHandles
        {
            _move = moveAction,
            _jump = jumpAction,
            _attack = attackAction,
            _ready = readyAction,
            _cancel = cancelAction,
            _onMovePerformed = ctx =>
            {
                var value = ctx.ReadValue<Vector2>();
                _move[slotId] = value;
                OnMoveInput?.Invoke(slotId, value);
            },
            _onMoveCanceled = ctx =>
            {
                _move[slotId] = Vector2.zero;
                OnMoveInput?.Invoke(slotId, Vector2.zero);
            },
            _onJumpPerformed = ctx =>
            {
                OnJumpInput?.Invoke(slotId);
                Debug.Log($"[InputManager] JumpInput: idx={slotId}");
            },
            _onAttackPerformed = ctx =>
            {
                OnAttackInput?.Invoke(slotId);
                Debug.Log($"[InputManager] AttackInput: idx={slotId}");
            },
            _onReady = ctx =>
            {
                OnReadyInput?.Invoke(slotId);
            },
            _onCancel = ctx =>
            {
                OnCancelInput?.Invoke(slotId);
            }
        };

        if (moveAction != null)
        {
            moveAction.performed += handle._onMovePerformed;
            moveAction.canceled += handle._onMoveCanceled;
        }
        if (jumpAction != null)
        {
            jumpAction.performed += handle._onJumpPerformed;
        }
        if (attackAction != null)
        {
            attackAction.performed += handle._onAttackPerformed;
        }
        if(readyAction != null)
        {
            readyAction.performed += handle._onReady;
        }
        if(cancelAction != null)
        {
            cancelAction.performed += handle._onCancel;
        }

        _actions[input] = handle;

    }

    public void UpdateControllerForSlot(int slotId, PlayerController controller)
    {
        if(controller == null)
        {
            _slotControllers.Remove(slotId);
        }
        else
        {
            _slotControllers[slotId] = controller;
        }
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

        // 登録されているActionを取得
        if(_actions.TryGetValue(playerInput, out var action))
        {
            if (action._move != null)
            {
                action._move.performed -= action._onMovePerformed;
                action._move.canceled -= action._onMoveCanceled;
            }
            if (action._jump != null)
            {
                action._jump.performed -= action._onJumpPerformed;
            }
            if (action._attack != null)
            {
                action._attack.performed -= action._onAttackPerformed;
            }
            if(action._ready != null)
            {
                action._ready.performed -= action._onReady;
            }
            if(action._cancel != null)
            {
                action._cancel.performed -= action._onCancel;
            }
            _actions.Remove(playerInput);
        }

        int targetSlotId = -1;
        foreach(var slot in _slotInputs)
        {
            if(slot.Value == playerInput)
            {
                targetSlotId = slot.Key;
                break;
            }
        }

        if( targetSlotId != -1)
        {
            _slotInputs.Remove(targetSlotId);
            _slotControllers.Remove(targetSlotId);
            _move.Remove(targetSlotId);
        }
    }

    /// <summary>
    /// すべてのプレイヤーのコントローラーの入力状態を管理する
    /// </summary>
    /// <param name="isEnable">true : 入力できる false : 入力できない</param>
    public void SetAllPlayerControl(bool isEnable)
    {
        // インプットマネージャーが持っているプレイヤーコントローラーすべてに
        // 入力可能かどうかをセットする
        foreach (var info in _slotControllers)
        {
            var controller = info.Value;
            if(controller != null)
            {
                controller.SetInputActive(isEnable);
            }
        }
    }

    public void ReportPlayerDied(int playerIndex)
    {
        Debug.Log($"[InputManager] player{playerIndex} is Die.");
        OnPlayerDied?.Invoke(playerIndex);
    }

    public void InitPlayers()
    {
        foreach (var info in _slotControllers)
        {
            var controller = info.Value;
            if (controller != null)
            {
                controller.Init();
            }
        }
    }
}
