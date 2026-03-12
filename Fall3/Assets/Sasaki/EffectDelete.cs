using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDelete : MonoBehaviour
{
    [SerializeField]
    float deleteTime = 5.0f;

    float _time = 0.0f;

    void Update()
    {
        _time += Time.deltaTime;
        if (_time > deleteTime)
        {
            Destroy(this.gameObject);
        }
    }
}
