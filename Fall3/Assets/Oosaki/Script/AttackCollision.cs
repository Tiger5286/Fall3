using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{

    Vector3 startPos;
    public float _destroyDistance = 10f;

    //ノックバックの強さ
    public float _knockbackForce = 12.0f;

    //上方向のノックバックの強さ
    public float _knockbackUpwardForce = 12.0f;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(startPos, transform.position);

        if (distance >= _destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

        if (playerController != null)
        {
            //攻撃からプレイヤーへの方向を計算
            Vector3 horizontalDir = collision.transform.position - transform.position;
            horizontalDir.y = 0f;
            horizontalDir.Normalize();

            Vector3 force =
                horizontalDir * _knockbackForce +
                Vector3.up * _knockbackUpwardForce;

            playerController.ApplyKnockBack(force);

        }
        //SE再生
        SoundManager.Instance.PlaySe(7);

        // 当たったら消す
        this.GetComponent<HadokenEffect>().Die();
        Destroy(gameObject);
    }
}
