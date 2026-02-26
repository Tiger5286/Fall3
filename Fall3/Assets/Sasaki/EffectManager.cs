using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// このスクリプトをアタッチしたオブジェクトをエフェクトを再生したいオブジェクトに持たせてください
/// </summary>
public class EffectManager : MonoBehaviour
{
    /// <summary>
    /// エフェクト本体をこの配列に入れてください
    /// </summary>
    [SerializeField]
    GameObject[] effectPrefabs;

    /// <summary>
    /// エフェクトを再生する関数
    /// </summary>
    /// <param name="effectPrefabNum">再生したいエフェクトが入っている配列の番号</param>
    /// <param name="trans">再生したいエフェクトのtransform情報</param>
    public void PlayEffect(int effectPrefabNum,Transform trans)
    {
        Instantiate(effectPrefabs[effectPrefabNum],trans);
    }
}
