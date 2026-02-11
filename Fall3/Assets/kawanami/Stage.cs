using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;



public class Stage : MonoBehaviour
{
    //グリッド上の座標
    public int x;
    public int y;

    /// <summary>
    /// グリッド座標を設定して、ポジションに反映する
    /// </summary>
    /// <param name="gx">グリッド上のx</param>
    /// <param name="gy">グリッド上のx</param>
    public void SetGridPos(int gx,int gy)
    {
        x = gx;
        y = gy;

        //ワールド座標に変換
        //ワールド座標では縦がz
        transform.position = new Vector3 (x, 0, y);
    }
}
