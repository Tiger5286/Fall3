using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] effects;

    public void PlayEffect(int  effectIndex,Transform transform)
    {
        Instantiate(effects[effectIndex],transform);
    }
}
