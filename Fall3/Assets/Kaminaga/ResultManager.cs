using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : GameManagerBase
{
    // 定数定義
    private const float kInputEnableTime = 2.0f;

    [Header("勝利判定関連")]
    [SerializeField] private GameSession _gameSession;

    [Header("シーン管理")]
    [SerializeField] private SceneManagerKaminaga _sceneManager;

    [Header("UI管理クラス")]
    [SerializeField] private ResultUIManager _resultUIManager;

    [Header("リザルト用のプレイヤーのプレハブ")]
    [SerializeField] private GameObject _resultPlayerPrefab1;
    [SerializeField] private GameObject _resultPlayerPrefab2;
    [Header("リザルト用のプレイヤーの生成位置")]
    [SerializeField] private Transform _spawnPoint1;
    [SerializeField] private Transform _spawnPoint2;
    [SerializeField] private Transform _winnerSpawnPoint;

    //リザルトのプレイヤーを保存するための変数
    private GameObject _resultPlayer1;
    private GameObject _resultPlayer2;

    private float _timeCount = 0.0f;
    private bool _isInputEnable = false;

    private void OnEnable()
    {
        Debug.Log("Result開始");

        // 時間カウンタをリセット
        _timeCount = 0.0f;

        // 入力可能フラグをリセット
        _isInputEnable = false;

        // プレイヤーの操作状態を非アクティブにする
        InputManager.Instance.SetAllPlayerControl(false);

        //リザルト用プレイヤー生成
        SpawnResultPlayer();

        //BGM再生
        SoundManager.Instance.PlayBGMFade(2, 1.5f);
    }

    private void OnDisable()
    {
        Debug.Log("Result終了");
    }

    public void OnRetry()
    {
        if (!_isInputEnable)
        {
            return;
        }

        //リザルトプレイヤー削除
        if (_resultPlayer1 != null) Destroy(_resultPlayer1);
        if (_resultPlayer2 != null) Destroy(_resultPlayer2);

        if (JoinManager.Instance._playerCount <= 1)
        {
            Debug.Log("プレイヤーの人数が足りません : あと" + JoinManager.Instance._playerCount + "人");
            SoundManager.Instance.PlaySe(6);
            _resultUIManager.OnPlayerNotEnough();
            return;
        }

        SoundManager.Instance.PlaySe(5);

        _sceneManager.ChangeScene(SceneType.InGame);
    }

    public void OnReturnTitle()
    {
        if (!_isInputEnable)
        {
            return;
        }

        //リザルトプレイヤー削除
        if (_resultPlayer1 != null) Destroy(_resultPlayer1);
        if (_resultPlayer2 != null) Destroy(_resultPlayer2);

        SoundManager.Instance.PlaySe(5);

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
        _timeCount += Time.deltaTime;

        // 一定時間経過した後にシーン遷移可能にする
        if (kInputEnableTime < _timeCount)
        {
            _isInputEnable = true;
        }
    }

    void SpawnResultPlayer()
    {
        // 勝敗でプレイヤーを生成する位置を決定
        Vector3 player1Pos = Vector3.zero;
        Vector3 player2Pos = Vector3.zero;
        switch (_gameSession._lastWinner)
        {
            case WinnerType.Player1:
                player1Pos = _winnerSpawnPoint.position;
                player2Pos = _spawnPoint2.position;
                break;
            case WinnerType.Player2:
                player1Pos = _spawnPoint1.position;
                player2Pos = _winnerSpawnPoint.position;
                break;
            case WinnerType.Draw:
                player1Pos = _spawnPoint1.position;
                player2Pos = _spawnPoint2.position;
                break;
        }

        //プレイヤー1生成
        _resultPlayer1 = Instantiate(_resultPlayerPrefab1, player1Pos, _spawnPoint1.rotation);
        Animator animator1 = _resultPlayer1.GetComponent<Animator>();

        //プレイヤー2生成
        _resultPlayer2 = Instantiate(_resultPlayerPrefab2, player2Pos, _spawnPoint2.rotation);
        Animator animator2 = _resultPlayer2.GetComponent<Animator>();

        //アニメーションリセット
        animator1.Play("Idle", 0, 0f);
        animator2.Play("Idle", 0, 0f);
        animator1.SetBool("isWin", false);
        animator2.SetBool("isWin", false);

        //勝敗
        switch (_gameSession._lastWinner)
        {
            case WinnerType.Player1:
                animator1.SetBool("isWin", true);
                break;

            case WinnerType.Player2:
                animator2.SetBool("isWin", true);
                break;
        }
    }
}
