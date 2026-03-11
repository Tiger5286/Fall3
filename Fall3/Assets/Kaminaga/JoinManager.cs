using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(PlayerInputManager))]
public class JoinManager : MonoBehaviour
{
    public static JoinManager Instance { get; private set; }

    private PlayerInputManager _playerInputManager;
    public int _playerCount;


    [SerializeField] private PlayerFactory _playerFactory;
    [SerializeField] private PlayerRegistry _playerRegistry;

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

        DontDestroyOnLoad(gameObject);

        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerCount = _playerRegistry.GetJoinedCount();
    }

    private void OnEnable()
    {
        _playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoinedUnityEvent);
        _playerInputManager.playerLeftEvent.AddListener(OnPlayerLeftUnityEvent);
    }

    private void OnDisable()
    {
        if (_playerInputManager == null)
        {
            return;
        }

        _playerInputManager.playerJoinedEvent.RemoveListener(OnPlayerJoinedUnityEvent);
        _playerInputManager.playerLeftEvent.RemoveListener(OnPlayerLeftUnityEvent);
    }

    private void OnPlayerJoinedUnityEvent(PlayerInput p) => HandleJoined(p, "UnityEvent");

    private void OnPlayerLeftUnityEvent(PlayerInput p) => HandleLeft(p, "UnityEvent");

    private void HandleJoined(PlayerInput player, string via)
    {
        Debug.Log($"[JoinManager] Joined({via}): idx={player.playerIndex}, device = {string.Join(",", player.devices)}");

        if (player.currentActionMap == null)
        {
            player.SwitchCurrentActionMap("Disable");
        }

        var slot = _playerRegistry.FindFreeSlot();
        if (slot == null)
        {
            Debug.LogWarning("参加できるプレイヤーのスロットがありません");
            return;
        }

        var link = player.GetComponent<PlayerLink>();
        link.SetPlayerId(slot._id);

        _playerFactory.SpawnPlayerForSlot(slot._id, player);

        _playerRegistry.AssignToSlot(slot, player, slot._controller);

        InputManager.Instance.RegisterPlayerToSlot((int)slot._id, player, slot._controller);

        // コントローラーの情報が勝手に切り替わらないようにする
        player.neverAutoSwitchControlSchemes = true;

        player.onDeviceLost += OnDeviceLost;

        _playerCount = _playerRegistry.GetJoinedCount();
    }

    private void HandleLeft(PlayerInput player, string via)
    {
        Debug.Log($"[JoinManager] Left({via}): idx={player.playerIndex}");

        var slot = _playerRegistry.FindSlotByInput(player);
        if (slot != null)
        {
            InputManager.Instance.UnRegisterPlayer(player);

            _playerRegistry.ReleaseSlot(slot);
        }

        _playerCount = _playerRegistry.GetJoinedCount();
    }

    private void OnDeviceLost(PlayerInput player)
    {
        var slot = _playerRegistry.FindSlotByInput(player);
        if (slot != null)
        {
            InputManager.Instance.UnRegisterPlayer(player);

            _playerRegistry.ReleaseSlot(slot);

            Destroy(player.gameObject);
        }
        _playerCount = _playerRegistry.GetJoinedCount();
    }

    public void OnPlayerDeath(PlayerId id)
    {
        var slot = _playerRegistry.FindSlotById(id);
        if (slot == null)
        {
            return;
        }

        _playerRegistry.DetachController(slot);
    }
}