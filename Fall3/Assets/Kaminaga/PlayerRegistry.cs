using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// コントローラーの接続ID
/// </summary>
public enum PlayerId
{
    None = 0,
    Player1 = 1,
    Player2 = 2
}

/// <summary>
/// プレイヤーが参加できるスロット
/// </summary>
public class PlayerSlot
{
    public PlayerId _id; // プレイヤーのID
    public PlayerInput _playerInput; // プレイヤーのコントローラーの入力情報
    public PlayerController _controller; // プレイヤーの挙動クラス
    public bool _isReady; // 準備完了か
}

/// <summary>
/// プレイヤーを管理するクラス
/// </summary>
public class PlayerRegistry : MonoBehaviour
{
    private const int kPlayerMaxNum = 2;

    public PlayerSlot[] _slots = new PlayerSlot[kPlayerMaxNum];

    private void Awake()
    {
        _slots[0] = new PlayerSlot { _id = PlayerId.Player1 };
        _slots[1] = new PlayerSlot { _id = PlayerId.Player2 };
    }

}
