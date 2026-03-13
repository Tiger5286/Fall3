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
    private ImageController _gameStartTextController; // UIを操作するコンポーネントを入れる変数

    // ゲーム開始のゲージを表示するイメージオブジェクト
    [SerializeField] GameObject _gameStartGaugeObj;
    private Image _gameStartGauge; // Imageコンポーネントを入れる変数

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
        _gameStartGauge.gameObject.SetActive(false); // ゲーム開始のゲージを非表示にする
    }

    private void Start()
    {
        _gameStartText = _gameStartTextObj.GetComponent<TextMeshProUGUI>();
        _gameStartTextController = _gameStartTextObj.GetComponent<ImageController>();
        _gameStartGauge = _gameStartGaugeObj.GetComponent<Image>();
        _inputManager = _inputManagerObj.GetComponent<InputManager>();
        _stageManager = _stageManagerObj.GetComponent<StageManager>();
        _gameStartText.text = "";
        _gameStartState = GameStartState.Default;
        _gameStartGauge.gameObject.SetActive(false); // ゲーム開始のゲージを非表示にする
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

            _gameStartGauge.fillAmount = (_timer - 3.0f) / 2.0f; // Ready状態からStart状態までの時間割合をゲージに設定

            if (_gameStartState == GameStartState.Wait && _timer >= 3f)
            {
                // 3秒経過でReadyに遷移
                _gameStartState = GameStartState.Ready;
                _gameStartText.text = "Ready...";
                _gameStartTextController.StartExpand(); // ゲームスタートのテキストを拡大する
                _gameStartGauge.gameObject.SetActive(true); // ゲーム開始のゲージを表示する
            }

            if (_gameStartState == GameStartState.Ready && _timer >= 5f)
            {
                // 4秒経過でStartに遷移
                _gameStartState = GameStartState.Start;
                _gameStartText.text = "Fall!!";
                _gameStartTextController.StartDown(); // ゲームスタートのテキストを落ちる感じで動かす
                _gameStartGauge.gameObject.SetActive(false); // ゲーム開始のゲージを非表示にする
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
