using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// プレイヤーインプットを取得した上で使う
[RequireComponent(typeof(PlayerInput))]
public class InputTestPlayer : MonoBehaviour
{
    public Vector2 _move;
    public float _moveSpeed = 5.0f;
    public float _jumpForce = 1.5f;
    public int _playerIndex;

    public void OnEnable()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput += HandleMoveInput;
            InputManager.Instance.OnJumpInput += HandleJumpInput;
        }
    }

    public void OnDisable()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= HandleMoveInput;
            InputManager.Instance.OnJumpInput -= HandleJumpInput;
        }
    }

    public void HandleMoveInput(int idx, Vector2 moveValue)
    {
        if(idx != _playerIndex) return;

        _move = moveValue;
    }

    public void HandleJumpInput(int idx)
    {
        if(idx != _playerIndex) return;
        Debug.Log($"Player{idx} Jump!");
    }

    private void Awake()
    {
        _playerIndex = GetComponent<PlayerInput>().playerIndex;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = new Vector3(_move.x, 0, _move.y) * _moveSpeed * Time.deltaTime;

        transform.Translate(delta, Space.World);
    }
}
