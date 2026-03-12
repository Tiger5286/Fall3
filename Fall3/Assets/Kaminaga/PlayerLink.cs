using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerLink : MonoBehaviour
{
    public PlayerInput _playerInput {get; private set;}
    public PlayerId _id { get; private set;}

    private void Awake()
    {
        Debug.Log("èàóùç≈èâ");
        _playerInput = GetComponent<PlayerInput>();
    }

    public void SetPlayerId(PlayerId id)
    {
        _id = id;
    }

}
