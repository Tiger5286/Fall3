using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{

    Vector3 startPos;
    public float destroyDistance = 10f;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(startPos, transform.position);

        if (distance >= destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("“–‚½‚Á‚½‘Šè: " + collision.gameObject.name);

        // “–‚½‚Á‚½‚çÁ‚·
        Destroy(gameObject);
    }
}
