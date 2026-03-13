using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : GameManagerBase
{

    [Header("勝利判定関連")]
    [SerializeField] private GameSession _gameSession;

    [Header("シーン管理")]
    [SerializeField] private SceneManagerKaminaga _sceneManager;

    [Header("ステージマネージャー")]
    [SerializeField] private StageManager _stageManager;

    [SerializeField] private PlayerFactory _playerFactory;
    [SerializeField] private PlayerRegistry _playerRegistry;

    [Header("ゲームスタートマネージャー(佐々木)")]
    [SerializeField] private GameStartManager _gameStartManager;

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
            SpawnPlayer();
            InputManager.Instance.SetAllPlayerControl(true);
            InputManager.Instance.InitPlayers();
            InputManager.Instance.OnPlayerDied += HandlePlayerDead;
        }

        _stageManager.Init();

        _gameStartManager.GameStart();

        SoundManager.Instance.PlayBGMFade(1, 1.5f);
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
        SoundManager.Instance.StopBGMFade(2.0f);
        _sceneManager.ChangeScene(SceneType.Result);
    }

    private void SpawnPlayer()
    {
        Debug.Log("プレイヤーの生成");
        var player1 = _playerRegistry.FindSlotById(PlayerId.Player1);
        if(player1 != null && player1._controller == null && player1._playerInput != null)
        {
            var controller = _playerFactory.SpawnPlayer(player1._playerInput.transform, PlayerId.Player1);
            _playerRegistry.AssignController(player1, controller);
            InputManager.Instance.UpdateControllerForSlot((int)PlayerId.Player1, controller);
        }

        var player2 = _playerRegistry.FindSlotById(PlayerId.Player2);
        if (player2 != null && player2._controller == null && player2._playerInput != null)
        {
            var controller = _playerFactory.SpawnPlayer(player2._playerInput.transform, PlayerId.Player2);
            _playerRegistry.AssignController(player2, controller);
            InputManager.Instance.UpdateControllerForSlot((int)PlayerId.Player2, controller);
        }
    }
}
