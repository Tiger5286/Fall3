using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBase : MonoBehaviour
{
    // シーンが終了するときにtrueになる
    bool isEndScene = false;

    // シーン終了を取得
    public bool IsEndScene()
    {
        return isEndScene;
    }
}