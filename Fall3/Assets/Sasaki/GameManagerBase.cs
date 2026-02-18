using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBase : MonoBehaviour
{
    // シーンが終了するときにtrueになる
    //bool _isEndScene = false;

    // シーンがアクティブ状態かどうか
    protected bool _isActive = false;

    // シーン終了を取得
    //public bool IsEndScene()
    //{
    //    return _isEndScene;
    //}

    // シーンのアクティブ状態をセットする
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        gameObject.SetActive(isActive);
    }
}