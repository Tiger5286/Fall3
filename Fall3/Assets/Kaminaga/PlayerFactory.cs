using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;


    public PlayerController SpawnPlayer(Transform playerRoot, PlayerId id)
    {
        var player = Instantiate(_playerPrefab, playerRoot);
        var controller = player.GetComponent<PlayerController>();

        if (controller == null)
        {
            Debug.LogError("PlayerFactory : ê∂ê¨é∏îs");
            return null;
        }
        controller.SetPlayerId(id);
        return controller;
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
