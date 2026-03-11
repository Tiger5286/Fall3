using UnityEngine;

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

    private void OnEnable()
    {
        Debug.Log("Result開始");

        // 時間カウンタをリセット
        _timeCount = 0.0f;

        // 入力可能フラグをリセット
        _isInputEnable = false;
        
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
        if(!_isInputEnable)
        {
            return;
        }
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
        if (!_isInputEnable)
        {
            return;
        }
        SoundManager.Instance.StopBGMFade(2.0f);
        _sceneManager.ChangeScene(SceneType.Title);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("winner : " + _gameSession._lastWinner);
        Debug.Log("winCounter Player1:" + _gameSession._winCountPlayer1 + " Player2:" + _gameSession._winCountPlayer2);

        bool isPlayer1Winner = (_gameSession._lastWinner == PlayerType.Player1);

        //プレイヤー1生成
        GameObject player1Obj = Instantiate(_resultPlayerPrefab1, _spawnPoint1.position, _spawnPoint1.rotation);
        Animator animator1 = player1Obj.GetComponent<Animator>();
        animator1.SetBool("isWin", isPlayer1Winner);

        //プレイヤー2生成
        GameObject player2Obj = Instantiate(_resultPlayerPrefab2, _spawnPoint2.position, _spawnPoint2.rotation);
        Animator animator2 = player2Obj.GetComponent<Animator>();
        animator2.SetBool("isWin", !isPlayer1Winner);
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
}
