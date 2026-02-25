using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EffectManagerを使ったエフェクト再生のテスト用スクリプト
/// </summary>
public class SasakiTestObject : MonoBehaviour
{
    /// <summary>
    /// effectManagerをアタッチしたオブジェクトを入れます
    /// </summary>
    [SerializeField]
    GameObject effectManagerObj;

    private void Start()
    {
        // effectManagerを持っているオブジェクトのスクリプトを取得します
        var temp = effectManagerObj.GetComponent<EffectManager>();

        // 取得したスクリプトの関数を呼び、エフェクトを再生します
        temp.PlayEffect(2,transform);
    }
}
