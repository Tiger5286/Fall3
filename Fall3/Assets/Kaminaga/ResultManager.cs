using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : GameManagerBase
{
    [Header("シーン管理")]
    [SerializeField] private SceneManagerKaminaga _sceneManager;

    private void OnEnable()
    {
        Debug.Log("Result開始");
        // プレイヤーの操作状態を非アクティブにする
        InputManager.Instance.SetAllPlayerControl(false);
    }

    private void OnDisable()
    {
        Debug.Log("Result終了");
    }

    public void OnRetry()
    {
        _sceneManager.ChangeScene(SceneType.InGame);
    }

    public void OnReturnTitle()
    {
        _sceneManager.ChangeScene(SceneType.Title);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
