using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpawner: MonoBehaviour
{
    //生成する球
    [SerializeField] GameObject ballPrefab;
    //出現位置
    [SerializeField] Transform spawnPoint;

    //今出てる弾
    GameObject _currentBall; 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 弾が無い時だけ発射
            if (_currentBall == null)
            {
                SpawnBall();
            }
        }
    }

    public void SpawnBall()
    {
        Vector3 spawnPos = spawnPoint.position + transform.forward * 1.0f; //プレイヤーの前に出す

        // 球を生成
        GameObject ball = Instantiate(
            ballPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        // 前方向に飛ばす（仮）
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 300f);

        Physics.IgnoreCollision(ball.GetComponent<Collider>(), GetComponent<Collider>()); //プレイヤーと衝突しないようにする
    }
}
