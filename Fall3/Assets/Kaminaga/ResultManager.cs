using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : GameManagerBase
{
    [Header("勝利判定関連")]
    [SerializeField] private GameSession _gameSession;

    [Header("シーン管理")]
    [SerializeField] private SceneManagerKaminaga _sceneManager;

    [Header("UI管理クラス")]
    [SerializeField] private ResultUIManager _resultUIManager;

    private void OnEnable()
    {
        Debug.Log("Result開始");
        // プレイヤーの操作状態を非アクティブにする
        InputManager.Instance.SetAllPlayerControl(false);

        //BGM再生
        SoundManager.Instance.PlayBGM(2);
    }

    private void OnDisable()
    {
        Debug.Log("Result終了");
    }

    public void OnRetry()
    {
        if (JoinManager.Instance._playerCount <= 1)
        {
            Debug.Log("プレイヤーの人数が足りません : あと" + JoinManager.Instance._playerCount + "人");
            _resultUIManager.OnPlayerNotEnough();
            return;
        }

        _sceneManager.ChangeScene(SceneType.InGame);
    }

    public void OnReturnTitle()
    {
        SoundManager.Instance.StopBGMFade(2.0f);
        _sceneManager.ChangeScene(SceneType.Title);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("winner : " + _gameSession._lastWinner);
        Debug.Log("winCounter Player1:" + _gameSession._winCountPlayer1 + " Player2:" + _gameSession._winCountPlayer2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
