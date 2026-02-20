using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : GameManagerBase
{
    [Header("シーン管理")]
    [SerializeField] private SceneManagerKaminaga _sceneManager;


    private void OnEnable()
    {
        Debug.Log("Title開始");
    }

    private void OnDisable()
    {
        Debug.Log("Title終了");
    }

    public void OnGameStart()
    {
        _sceneManager.ChangeScene(SceneType.InGame);
        Debug.Log("ボタンを押した");
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
