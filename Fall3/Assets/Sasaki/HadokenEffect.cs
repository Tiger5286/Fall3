using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class HadokenEffect : MonoBehaviour
{
    [SerializeField]
    GameObject effectPrefab;
    [SerializeField]
    float effectScale = 1.0f;

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
}
