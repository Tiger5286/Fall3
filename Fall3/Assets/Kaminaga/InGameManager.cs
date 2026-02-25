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

    private bool _isDeadPlayer1;
    private bool _isDeadPlayer2;

    private void OnEnable()
    {
        _isGameSet = false;

        _isDeadPlayer1 = false;
        _isDeadPlayer2 = false;

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
        if (playerIndex == 0)
        {
            _isDeadPlayer1 = true;
        }
        if (playerIndex == 1)
        {
            _isDeadPlayer2 = true;
        }
        
        if(!_isGameSet)
        {
            _isGameSet = true;
            StartCoroutine(CheckWinner());
        }
    }

    private IEnumerator CheckWinner()
    {
        // 最初のフレームだけ何もしない
        yield return null;

        WinnerType winner = WinnerType.None;

        // 勝敗判定

        winner =
            (_isDeadPlayer1 && _isDeadPlayer2) ? WinnerType.Draw : // プレイヤーが両方やられたならDraw
            (!_isDeadPlayer1 && _isDeadPlayer2) ? WinnerType.Player1 : // 1Pがやられていないなら1Pの勝ち
            (_isDeadPlayer1 && !_isDeadPlayer2) ? WinnerType.Player2 : // 2Pがやられていないなら2Pの勝ち 
            WinnerType.None; // それ以外ならNoneにする

        _gameSession.SetResult(winner);

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
