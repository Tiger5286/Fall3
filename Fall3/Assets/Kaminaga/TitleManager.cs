using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : GameManagerBase
{
    [Header("シーン管理")]
    [SerializeField] private SceneManagerKaminaga _sceneManager;

    [Header("タイトルのUI管理クラス")]
    [SerializeField] private TitleUIManager _titleUIManager;

    private void OnEnable()
    {
        Debug.Log("Title開始");
        SoundManager.Instance.PlayBGM(3);
    }

    private void OnDisable()
    {
        Debug.Log("Title終了");
    }

    public void OnGameStart()
    {
        if (JoinManager.Instance._playerCount <= 1)
        {
            Debug.Log("プレイヤーの人数が足りません : あと" + JoinManager.Instance._playerCount + "人");
            SoundManager.Instance.PlayBGM(6);
            _titleUIManager.OnPlayerNotEnough();
            return;
        }

        InputManager.Instance.SetAllPlayerControl(true);

        SoundManager.Instance.PlaySe(5);
        SoundManager.Instance.StopBGMFade(2.0f);
        _sceneManager.ChangeScene(SceneType.InGame);
        Debug.Log("ゲーム開始");
    }

    public void OnGameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }



    void Start()
    {
    }

    void Update()
    {
        
    }
}
