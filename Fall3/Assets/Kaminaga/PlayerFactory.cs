using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

[System.Serializable]
public struct PlayerPrefabInfo
{
    public PlayerId _playerId;
    public GameObject _obj;
}

public class PlayerFactory : MonoBehaviour
{
    [Header("ê∂ê¨Ç∑ÇÈÉvÉåÉCÉÑÅ[ÉäÉXÉg")]
    [SerializeField] private List<PlayerPrefabInfo> _playerPrefabs = new();

    [Header("ÉvÉåÉCÉÑÅ[ÇÃê∂ê¨Ç≈Ç´ÇÈòg")]
    [SerializeField] private PlayerRegistry _registry;

    private readonly Dictionary<PlayerId, GameObject> _playerPrefabMap = new();

    private void Awake()
    {
        _playerPrefabMap.Clear();

        foreach (var player in _playerPrefabs)
        {
            if (player._obj == null)
            {
                continue;
            }
            _playerPrefabMap[player._playerId] = player._obj;
        }
    }

    public PlayerController SpawnPlayer(Transform playerRoot, PlayerId id)
    {

        if(id == PlayerId.None)
        {
            Debug.LogError("PlayerFactory : ê∂ê¨é∏îs");
            return null;
        }

        var player = Instantiate(_playerPrefabMap[id], playerRoot);

        var controller = player.GetComponent<PlayerController>();

        if (controller == null)
        {
            Debug.LogError("PlayerFactory : ê∂ê¨é∏îs");
            return null;
        }
        controller.SetPlayerId(id);
        return controller;
    }

    public void SpawnPlayerForSlot(PlayerId id, PlayerInput input)
    {
        if (id == PlayerId.None)
        {
            Debug.LogError("PlayerFactory : ê∂ê¨é∏îs");
            return;
        }

        var player = Instantiate(_playerPrefabMap[id], input.transform);

        var controller = player.GetComponent<PlayerController>();

        if (controller == null)
        {
            Debug.LogError("PlayerFactory : ê∂ê¨é∏îs");
            return;
        }
        controller.SetPlayerId(id);

        var slot = _registry.FindSlotById(id);
        _registry.AssignController(slot, controller);

        InputManager.Instance.UpdateControllerForSlot((int)id, controller);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
