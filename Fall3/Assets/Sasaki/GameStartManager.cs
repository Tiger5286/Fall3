using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] GameObject _gameStartTextObj;
    private TextMeshProUGUI _gameStartText;

    enum GameStartState
    {
        Wait,
        Ready,
        Start,
        Started
    }

    float _timer = 0f;
    GameStartState _gameStartState = GameStartState.Wait;

    public void GameStart()
    {
        _timer = 0f;
        _gameStartState = GameStartState.Wait;
    }

    private void Start()
    {
        _gameStartText = _gameStartTextObj.GetComponent<TextMeshProUGUI>();
        _gameStartText.text = "";
    }

    void Update()
    {
        if (_gameStartState != GameStartState.Started)
        {
            _timer += Time.deltaTime;

            if (_gameStartState == GameStartState.Wait && _timer >= 3f)
            {
                // 3ïbåoâﬂÇ≈ReadyÇ…ëJà⁄
                _gameStartState = GameStartState.Ready;
                _gameStartText.text = "Ready...";
            }

            if (_gameStartState == GameStartState.Ready && _timer >= 5f)
            {
                // 4ïbåoâﬂÇ≈StartÇ…ëJà⁄
                _gameStartState = GameStartState.Start;
                _gameStartText.text = "Fall!!";
            }

            if (_gameStartState == GameStartState.Start && _timer >= 6f)
            {
                // Startèàóùå„Ç…StartedÇ…ëJà⁄
                _gameStartState = GameStartState.Started;
                _gameStartText.text = "";
            }
        }
    }
}
