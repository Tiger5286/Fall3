using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class HadokenEffect : MonoBehaviour
{
    [Header("波動拳本体のエフェクト")]
    [SerializeField]
    GameObject effectPrefab;
    [SerializeField]
    float effectScale = 1.0f;
    [Header("波動拳が当たった時のエフェクト")]
    [SerializeField]
    GameObject hitEffectPrefab;
    [SerializeField]
    float hitEffectScale = 1.0f;

    GameObject effectInstance;

    // Start is called before the first frame update
    void Start()
    {
        effectInstance = Instantiate(effectPrefab, transform);
        effectInstance.transform.localScale = Vector3.one * effectScale;
    }

    // Update is called once per frame
    void Update()
    {
        effectInstance.transform.position = transform.position;
    }

    public void Die()
    {
        var eff = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
        eff.transform.localScale = Vector3.one * hitEffectScale;
        Destroy(this.gameObject);
    }
}
