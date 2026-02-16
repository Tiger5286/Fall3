using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class JoinManager : MonoBehaviour
{
    private PlayerInputManager _playerInputManager;

    private void Awake()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
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
    }
}