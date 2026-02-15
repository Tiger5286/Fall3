using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    None,
    Normal,
    Fall
}
public class StageManager : MonoBehaviour
{
    //ステージのサイズ
    [Header("Stage Size")]
    //横マス数
    [SerializeField] private int width = 10;
    //縦マス数
    [SerializeField] private int height = 10;

    //ステージのPrefab
    [Header("Stage Prefab")]
    [SerializeField] private GameObject stagePrefab;

    //グリッド上のステージ情報
    //(2次元配列)
    private StageType[,] grid;

    //ステージオブジェクトへの参照
    //(2次元配列)
    private Stage[,] stages;

    // デバッグ用：現在選択中のマス
    private int selectX = 0;
    private int selectY = 0;

    //初期化
    private void Start()
    {
        //グリッド初期化
        InitGrid();
        //ステージ作成
        BuildStage();
    }

    //グリッドの初期化
    void InitGrid()
    {
        //2次元配列を作る
        //グリッドの配列
        grid = new StageType[width, height];
        //見た目の配列
        stages = new Stage[width, height];

        //ステージ配置
        for(int y = 0;y<height;y++)
        {
            for(int x = 0;x<width;x++)
            {
                grid[x, y] = StageType.Normal;
            }
        }
    }

    //ステージ作成
    void BuildStage()
    {
        for(int y = 0;y<height;y++)
        {
            for(int x = 0;x<width;x++)
            {
                SpawnStage(x, y);
            }
        }
    }
    //指定されたマスにステージを生成する
    void SpawnStage(int x,int y)
    {
        //置く必要がなかったら何もしない
        if (grid[x,y] == StageType.None)
        {
            return;
        }
        //ステージの生成
        GameObject go = Instantiate(stagePrefab,transform);
        Stage stage = go.GetComponent<Stage>();
        //グリッド座標を設定する
        stage.SetGridPos(x, y);
        //見た目の配列に登録
        stages[x,y] = stage;
    }

    //ステージを破壊する
    public void BreakStage(int x,int y)
    {
        //範囲外チェック
        if(!InRange(x,y))
        {
            return;
        }

        //すでに空なら何もしない
        if (grid[x,y] == StageType.None)
        {
            return;
        }
        //グリッドのデータを変更
        grid[x,y] = StageType.None;

        //見た目の変更
        if (stages[x,y] != null)
        {
            Destroy(stages[x, y].gameObject);
            stages[x, y] = null;
        }
    }

    public void FallStage(int x,int y)
    {
        Debug.Log($"FallStage {x},{y}");
        //範囲外チェック
        if (!InRange(x, y))
        {
            return;
        }

        //すでに空なら何もしない
        if (grid[x, y] == StageType.None)
        {
            return;
        }

        //グリッドのデータを変更
        grid[x, y] = StageType.Fall;

        //stage本体の関数を呼び出す
        stages[x, y].Fall();
    }

    // グリッドの範囲チェック
    bool InRange(int x, int y)
    {
        return x >= 0 && x < width &&
               y >= 0 && y < height;
    }

    private void Update()
    {
        // カーソル移動
        if (Input.GetKeyDown(KeyCode.A))
        {
            selectX--;
            Debug.Log($"selectX:{selectX}, selectY:{selectY}");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            selectX++;
            Debug.Log($"selectX:{selectX}, selectY:{selectY}");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            selectY++;
            Debug.Log($"selectX:{selectX}, selectY:{selectY}");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            selectY--;
            Debug.Log($"selectX:{selectX}, selectY:{selectY}");
        } 

        // 範囲内に制限
        selectX = Mathf.Clamp(selectX, 0, width - 1);
        selectY = Mathf.Clamp(selectY, 0, height - 1);

        // スペースキーで落とす
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FallStage(selectX, selectY);
        }
    }
    //デバッグ用にどのブロックを選んでいるか可視化する関数
    void OnDrawGizmos()
    {
        if (stages == null) return;

        Stage stage = stages[selectX, selectY];
        if (stage == null) return;
        
        //色を変える
        Gizmos.color = Color.red;
        Vector3 pos = stage.transform.position;
        pos.x += 0.5f;
        pos.y += 0.5f;
        pos.z += 0.5f;
        Gizmos.DrawWireCube(
            pos,
            Vector3.one
        );
    }
}
