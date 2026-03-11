using UnityEngine;

public class ResultManager : GameManagerBase
{
    [Header("勝利判定関連")]
    [SerializeField] private GameSession _gameSession;

    [Header("シーン管理")]
    [SerializeField] private SceneManagerKaminaga _sceneManager;

    [Header("UI管理クラス")]
    [SerializeField] private ResultUIManager _resultUIManager;

    [Header("リザルト用プレイヤー")]
    [SerializeField] private GameObject _resultPlayerPrefab1;    // 1P用
    [SerializeField] private GameObject _resultPlayerPrefab2;    // 2P用
    [SerializeField] private Transform _spawnPoint1;             //シーン内で表示する位置
    [SerializeField] private Transform _spawnPoint2;

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

        //プレイヤー1生成
        GameObject player1Obj = Instantiate(_resultPlayerPrefab1, _spawnPoint1.position, _spawnPoint1.rotation);
        Animator animator1 = player1Obj.GetComponent<Animator>();

        //プレイヤー2生成
        GameObject player2Obj = Instantiate(_resultPlayerPrefab2, _spawnPoint2.position, _spawnPoint2.rotation);
        Animator animator2 = player2Obj.GetComponent<Animator>();


        //勝敗に応じてアニメーションを切り替え
        switch (_gameSession._lastWinner)
        {
            case WinnerType.Player1:
                animator1.SetBool("isWin", true);
                break;
            case WinnerType.Player2:
                animator2.SetBool("isWin", false);
                break;
            default:
                animator1.SetBool("isWin", false);
                animator2.SetBool("isWin", false);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
