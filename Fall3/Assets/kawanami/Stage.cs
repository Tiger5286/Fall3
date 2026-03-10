using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Schema;
using UnityEngine;



public class Stage : MonoBehaviour
{
    //グリッド上の座標
    public int x;
    public int y;
    public int z;

    private int _fallWaitCounter = 0;
    private bool _isFall = false;
  
    private const float _fallSpeed = 0.2f;
    private int _fallWaitCounterNum = 40;

    public Vector3 _position;
    public Vector3 _velocity = Vector3.zero;

    // 落下可能かどうか
    bool _isCanFall = true;
    public void SetCanFall(bool canFall)
    {
        _isCanFall = canFall;
    }

    /// <summary>
    /// グリッド座標を設定して、ポジションに反映する
    /// </summary>
    /// <param name="gx">グリッド上のx</param>
    /// <param name="gy">グリッド上のx</param>
    public void SetGridPos(int gx,int gy,int gz)
    {
        x = gx;
        y = -gy;
        z = gz;
        //ワールド座標に変換
        //ワールド座標では縦がz
        _position = new Vector3 ((float)x, (float)y, (float)z);
    }

    public void Fall()
    {
        // 落下不可のときは落下させない(佐々木)
        if (!_isCanFall) return;

        _fallWaitCounter = _fallWaitCounterNum;
        _isFall = true;
    }

    public void Start()
    {
        
    }

    public void FixedUpdate()
    {
        if(_fallWaitCounter >0)
        {
            _fallWaitCounter --;
        }

        if (_fallWaitCounter <= 0&&_isFall)
        {
            _velocity = new Vector3(0, -_fallSpeed, 0);
        }

        _position.y += _velocity.y;
        transform.position = _position;
    }
}
