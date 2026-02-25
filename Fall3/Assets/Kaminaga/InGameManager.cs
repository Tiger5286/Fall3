using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : GameManagerBase
{

    [Header("勝利判定関連")]
    [SerializeField] private GameSession _gameSession;

    [Header("シーン管理")]
    [SerializeField] private SceneManagerKaminaga _sceneManager;

    private bool _isGameSet;

    private void OnEnable()
    {
        _isGameSet = false;
        Debug.Log("InGame開始");
        if(InputManager.Instance != null)
        {
            InputManager.Instance.SetAllPlayerControl(true);
            InputManager.Instance.OnPlayerDied += HandlePlayerDead;
        }
    }

    private void OnDisable()
    {
        Debug.Log("InGame終了");
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPlayerDied -= HandlePlayerDead;
        }
    }

    public void HandlePlayerDead(int playerIndex)
    {
        // すでにゲームが終了している場合処理をしない
        if (_isGameSet) return;

        WinnerType winner = WinnerType.None;

        if (playerIndex == 0)
        {
            winner = WinnerType.Player2;
        }
        else
        {
            winner = WinnerType.Player1;
        }

        _gameSession.SetResult(winner);
        
        _isGameSet = true;

        _sceneManager.ChangeScene(SceneType.Result);
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
