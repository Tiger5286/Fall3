using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    // ゲーム開始のテキストオブジェクト
    [SerializeField] GameObject _gameStartTextObj;
    private TextMeshProUGUI _gameStartText; // TextMeshProUGUIコンポーネントを入れる変数

    // プレイヤーの操作を制限するためのInputManagerオブジェクト
    [SerializeField] GameObject _inputManagerObj;
    private InputManager _inputManager; // InputManagerコンポーネントを入れる変数

    // ステージが落下するかどうか制御するためのStageManagerオブジェクト
    [SerializeField] GameObject _stageManagerObj;
    private StageManager _stageManager; // StageManagerコンポーネントを入れる変数

    // ゲーム開始の状態を管理する列挙型
    enum GameStartState
    {
        Default,
        Wait,
        Ready,
        Start,
        Started
    }

    // ゲーム開始のタイマーと状態
    float _timer = 0f;
    GameStartState _gameStartState = GameStartState.Wait;

    /// <summary>
    /// ゲーム開始の処理を開始するメソッド
    /// </summary>
    public void GameStart()
    {
        _timer = 0f;
        _gameStartState = GameStartState.Wait;
        _gameStartText.text = "";
        _inputManager.SetAllPlayerControl(false); // プレイヤーの操作を無効にする
        _stageManager.SetCanFall(false); // ステージを落下不可にする
    }

    private void Start()
    {
        _gameStartText = _gameStartTextObj.GetComponent<TextMeshProUGUI>();
        _inputManager = _inputManagerObj.GetComponent<InputManager>();
        _stageManager = _stageManagerObj.GetComponent<StageManager>();
        _gameStartText.text = "";
        _gameStartState = GameStartState.Default;
    }

    void Update()
    {
        if(_gameStartState == GameStartState.Default)
        {
            return;
        }

        if (_gameStartState != GameStartState.Started)
        {
            _timer += Time.deltaTime;

            if (_gameStartState == GameStartState.Wait && _timer >= 3f)
            {
                // 3秒経過でReadyに遷移
                _gameStartState = GameStartState.Ready;
                _gameStartText.text = "Ready...";
            }

            if (_gameStartState == GameStartState.Ready && _timer >= 5f)
            {
                // 4秒経過でStartに遷移
                _gameStartState = GameStartState.Start;
                _gameStartText.text = "Fall!!";
                _inputManager.SetAllPlayerControl(true); // プレイヤーの操作を有効にする
                _stageManager.SetCanFall(true); // ステージを落下可能にする
            }

            if (_gameStartState == GameStartState.Start && _timer >= 6f)
            {
                // Start処理後にStartedに遷移
                _gameStartState = GameStartState.Started;
                _gameStartText.text = "";
            }
        }
    }
}
