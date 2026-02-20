using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class JoinManager : MonoBehaviour
{
    public static JoinManager Instance { get; private set; }

    private PlayerInputManager _playerInputManager;
    public int _playerCount;

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
        _playerCount = _playerInputManager.playerCount;
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

    private void OnPlayerLeftUnityEvent(PlayerInput p) => Debug.Log($"[JoinManager] Left(UnityEvent): idx={p.playerIndex}");

    private void HandleJoined(PlayerInput player, string via)
    {
        Debug.Log($"[JoinManager] Joined({via}): idx={player.playerIndex}, device = {string.Join(",", player.devices)}");

        if (player.currentActionMap == null || player.currentActionMap.name != "GameInput")
        {
            player.SwitchCurrentActionMap("GameInput");
        }
        InputManager.Instance.RegisterPlayer(player);
        _playerCount = _playerInputManager.playerCount;
    }
}