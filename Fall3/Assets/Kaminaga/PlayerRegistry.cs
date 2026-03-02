using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// このスロットが開いているかどうかを判別する
    /// コントローラーの情報が入っていなければtrue
    /// </summary>
    public bool IsFree => _playerInput == null;
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
        // スロットの初期化
        _slots[0] = new PlayerSlot { _id = PlayerId.Player1, _playerInput = null, _controller = null, _isReady = false };
        _slots[1] = new PlayerSlot { _id = PlayerId.Player2, _playerInput = null, _controller = null, _isReady = false };
    }


    public PlayerSlot FindFreeSlot()
    {
        // プレイヤーが入っていないスロットを判別
        for(int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i]._playerInput == null)
            {
                return _slots[i];
            }
        }

        // プレイヤーが入っていなければnullを返す
        return null;
    }

    /// <summary>
    /// Idからプレイヤーのスロットを取得する
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PlayerSlot FindSlotById(PlayerId id)
    {
        // 指定Idと一致するスロットを判別
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i]._id == id)
            {
                return _slots[i];
            }
        }

        // 該当するプレイヤーが無ければnullを返す
        return null;
    }

    public PlayerSlot FindSlotByInput(PlayerInput input)
    {
        // 指定PlayerInputと一致するスロットを判別
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i]._playerInput == input)
            {
                return _slots[i];
            }
        }

        // 該当するプレイヤーが無ければnullを返す
        return null;
    }

    /// <summary>
    /// プレイヤーの情報をスロットにセットする
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="input"></param>
    /// <param name="controller"></param>
    public void AssignToSlot(PlayerSlot slot, PlayerInput input, PlayerController controller)
    {
        if(slot == null)
        {
            return;
        }
        slot._playerInput = input;
        slot._controller = controller;
        slot._isReady = false;
    }

    public void ReleaseSlot(PlayerSlot slot)
    {
        if(slot == null)
        {
            return;
        }
        slot._playerInput = null;
        slot._controller = null;
        slot._isReady = false;
    }

    public void DetachController(PlayerSlot slot)
    {
        if(slot == null)
        {
            return;
        }
        slot._controller = null;
        slot._isReady = false;
    }

    public void DetachInput(PlayerSlot slot)
    {
        if(slot == null)
        {
            return;
        }
        slot._playerInput = null;
        slot._controller = null;
        slot._isReady = false;
    }

    /// <summary>
    /// すべてのプレイヤーの準備状態をリセット(準備していない状態)する
    /// </summary>
    public void ResetAllReady()
    {
        foreach(PlayerSlot slot in _slots)
        {
            slot._isReady = false;
        }
    }

    public int GetJoinedCount()
    {
        int count = 0;
        foreach (PlayerSlot slot in _slots)
        {
            if(slot._playerInput != null)
            {
                count++;
            }
        }
        return count;
    }

}
