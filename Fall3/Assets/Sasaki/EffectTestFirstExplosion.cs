using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTestFirstExplosion : MonoBehaviour
{
    [Header("とりあえず最初だけエフェクト出るようにしてます")]
    [Header("消すなり煮るなり焼くなり好きにしてください")]

    [Header("EffectManagerを持っているオブジェクト")]
    [SerializeField]
    GameObject effectManagerObj;

    void Start()
    {
        var effectManager = effectManagerObj.GetComponent<EffectManager>();

        effectManager.PlayEffect(0, transform);
    }
}
