using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public enum CollisionType
    { 
        Capsule,//プレイヤー
        Box     //マップ
    }

    public enum CollisionTag
    { 
        Player1,
        Player2,
        Player1Attack,
        Player2Attack,
        Map
    }

    public CollisionType _collisionType;
    public CollisionTag _collisionTag;

    public Vector3 _size = Vector3.one;
    public float _radius = 0.5f;

    //すべてのコライダーの配列
    CollisionManager[] _colliders;

    void Start()
    {
        //全てのコライダーを取得
        _colliders = FindObjectsOfType<CollisionManager>();
    }

    void Update()
    {
        CheckCollision();
    }

    //攻撃が当たる人と当たらない人を分けるための判定処理
    bool IsAttackHit(CollisionManager col1,CollisionManager col2)
    {
        if (col1._collisionTag == CollisionTag.Player1Attack &&
            col2._collisionTag == CollisionTag.Player2) return true;

        return false;
    }
    void CheckCollision()
    {
        //他のコライダーと当たり判定を行うから、
        //全てのコライダーの組み合わせをチェックする
        for (int i = 0; i < _colliders.Length; i++)
        {
            for (int j = i + 1; j < _colliders.Length; j++)
            {
                CollisionManager col1 = _colliders[i];
                CollisionManager col2 = _colliders[j];
                if (IsAttackHit(col1, col2))
                {
                    Debug.Log("Player 1's attack hit Player 2!");
                }
            }
        }
    }
}
