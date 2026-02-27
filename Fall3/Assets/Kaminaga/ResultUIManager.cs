using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
    // 定数定義
    private const float kCursorMoveSpeed = 500.0f; // カーソルが動くスピード
    private const float kWarningTime = 3.0f; // 警告を出す時間(秒単位)

    [Header("キャンバス")]
    [SerializeField] private Canvas _canvas;

    [Header("カーソルの座標(RectTransform)")]
    [SerializeField] private RectTransform _cursor;

    [Header("キャンバスのレイキャスター")]
    [SerializeField] private GraphicRaycaster _graphicRaycaster;

    [Header("ゲーム内のイベントシステム")]
    [SerializeField] private EventSystem _eventSystem;

    [Header("InputSystemのAction")]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _clickAction;

    [Header("プレイヤーの数を示すテキスト")]
    [SerializeField] private TextMeshProUGUI _playerText;

    [Header("プレイヤーが足りない際に表示するテキスト")]
    [SerializeField] private TextMeshProUGUI _warningText;

    [Header("勝利判定関連")]
    [SerializeField] private GameSession _gameSession;

    [SerializeField] private TextMeshProUGUI _winText;

    [SerializeField] private TextMeshProUGUI _resultText;

    // 警告を表示する時間
    private float _warningTime = 0.0f;

    // キャンバス内のカーソルの座標
    private Vector2 _cursorPosCanvas;

    // キャンバスの設定で使うカメラ(使わない)
    private Camera _uiCamera;

    // 最後に選択されていたボタン(GameObjectで取得)
    private GameObject _lastSelectTarget;

    /// <summary>
    /// 最初に行う処理
    /// </summary>
    private void OnEnable()
    {
        // 警告テキストを非アクティブに
        _warningText.enabled = false;

        // アクションデータを有効化
        // 
        _moveAction?.action?.Enable();
        _clickAction?.action?.Enable();

        // キャンバスの中央にカーソルの位置を設定
        _cursorPosCanvas = GetCanvasCenter();

        // キャンバスの設定を適用
        ApplyCursorVisual();

        // 現在選択されているボタンを更新
        UpdateSelecting(_cursorPosCanvas);

        if(_gameSession._lastWinner == WinnerType.Draw)
        {
            _winText.text = _gameSession._lastWinner.ToString();
        }
        else
        {
            _winText.text = _gameSession._lastWinner + "Win!";
        }

        _resultText.text ="player1 : " + _gameSession._winCountPlayer1.ToString() + "player2 : " + _gameSession._winCountPlayer2.ToString();
    }

    /// <summary>
    /// 最後に行う処理
    /// </summary>
    private void OnDisable()
    {
        // アクションデータを無効化
        // 
        _moveAction?.action?.Disable();
        _clickAction?.action?.Disable();

        // 選択されたボタンがある場合
        if (_lastSelectTarget != null)
        {
            // 選択されたボタンの状態をnullにする
            //
            SendPointerExit(_lastSelectTarget);
            _lastSelectTarget = null;
            // EventSystemにも登録されているのでnullにする
            _eventSystem.SetSelectedGameObject(null);
        }
    }

    void Update()
    {
        // カーソルかキャンバスが参照されていなければ更新処理を行わない
        if (_cursor == null || _canvas == null)
        {
            return;
        }

        _playerText.text = "PlayerNum : " + JoinManager.Instance._playerCount.ToString();

        if (_warningTime > 0.0f)
        {
            _warningTime -= Time.deltaTime;
        }

        if (_warningTime < 0.0f)
        {
            _warningTime = 0.0f;
            _warningText.enabled = false;
        }

        // カーソルの動き
        // 
        Vector2 move = _moveAction?.action?.ReadValue<Vector2>() ?? Vector2.zero;

        // カーソルが動いている時
        if (move.sqrMagnitude > 0.0f)
        {
            // カーソルの座標を更新
            // カーソルのベクトルに時間とスピードを掛ける
            _cursorPosCanvas += move * kCursorMoveSpeed * Time.unscaledDeltaTime;
        }

        // カーソルのアンカーポイントをセットする
        SetCursorAnchoredPosition(_cursorPosCanvas);

        // 選ばれているボタンの更新
        UpdateSelecting(_cursorPosCanvas);

        // 決定入力が押された時
        if (_clickAction?.action?.WasPerformedThisFrame() == true)
        {
            ClickUIAt(_cursorPosCanvas);
        }
    }

    public void OnPlayerNotEnough()
    {
        _warningText.enabled = true;
        _warningTime = kWarningTime;
    }

    /// <summary>
    /// 選ばれているボタンを更新する
    /// </summary>
    /// <param name="canvasPos">キャンバス座標</param>
    private void UpdateSelecting(Vector2 canvasPos)
    {
        // キャンバスの情報かイベントシステムが参照されていないなら更新しない
        if (_graphicRaycaster == null || _eventSystem == null)
        {
            return;
        }

        // スクリーン座標を取得
        Vector2 screenPos = CanvasToScreenPosition(canvasPos);

        // EventSystemにあるpointerEventDataの一部を設定
        var data = new PointerEventData(_eventSystem)
        {
            // 座標をスクリーン座標にする
            position = screenPos,
            // ポインターのIDを-1にする
            // 
            pointerId = -1
        };

        // レイキャストした結果を保存するList
        var results = new List<RaycastResult>();

        // Raycastでポインターと重なっているオブジェクトを見る
        // 
        _graphicRaycaster.Raycast(data, results);

        // このフレームで重なっているオブジェクトを取得
        // 見つからなかったらnull、見つかったら最初に見つかったオブジェクトがターゲットとする
        GameObject newTarget = (results.Count > 0) ? results[0].gameObject : null;

        // 前フレームのオブジェクトがあり、このフレームのオブジェクトでなかったら
        if (_lastSelectTarget != null && _lastSelectTarget != newTarget)
        {
            // 前回のオブジェクトからは離れたとする
            SendPointerExit(_lastSelectTarget, data);
        }

        // このフレームのオブジェクトがあり、前フレームのオブジェクトでなかったら
        if (newTarget != null && _lastSelectTarget != newTarget)
        {
            // このフレームのオブジェクトからは離れたとする
            // 
            SendPointerExit(newTarget, data);

            // イベントシステムに選択されていると設定
            _eventSystem.SetSelectedGameObject(newTarget);
        }

        // このフレームのオブジェクトがnullで、前回のオブジェクトがあったら
        if (newTarget == null && _lastSelectTarget != null)
        {
            // 前回のオブジェクトからは離れたとする
            SendPointerExit(_lastSelectTarget, data);
            // 前回のオブジェクトをnullにする
            _lastSelectTarget = null;
            // イベントシステムに選択されているオブジェクトをnullにする
            _eventSystem.SetSelectedGameObject(null);
        }

        // ターゲットを更新
        _lastSelectTarget = newTarget;

    }

    /// <summary>
    /// カーソルが出たときを判定する
    /// </summary>
    /// <param name="target"></param>
    /// <param name="data"></param>
    private void SendPointerExit(GameObject target, PointerEventData data = null)
    {
        // 
        data ??= new PointerEventData(_eventSystem);
        // カーソルが離れたというイベントをターゲットに通知
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerExitHandler);
    }

    /// <summary>
    /// キャンバスの設定をみてカーソルの状態を設定(消すかも)
    /// </summary>
    private void ApplyCursorVisual()
    {
        if (_cursor != null)
        {
            _cursor.gameObject.SetActive(true);
            _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;
            SetCursorAnchoredPosition(_cursorPosCanvas);
        }
    }

    /// <summary>
    /// カーソルのアンカーポイントをキャンバスの座標にする
    /// </summary>
    /// <param name="canvasPos"></param>
    private void SetCursorAnchoredPosition(Vector2 canvasPos)
    {
        if (_cursor == null)
        {
            return;
        }

        _cursor.anchoredPosition = canvasPos;
    }

    /// <summary>
    /// キャンバスの中央を取得
    /// </summary>
    /// <returns></returns>
    private Vector2 GetCanvasCenter()
    {
        var rect = (_canvas.transform as RectTransform).rect;
        return rect.center;
    }

    /// <summary>
    /// キャンバスの座標をスクリーン座標に変換する
    /// </summary>
    /// <param name="canvasPos"></param>
    /// <returns></returns>
    Vector2 CanvasToScreenPosition(Vector2 canvasPos)
    {
        // キャンバスのtransformをRectTransformに変換
        var rectTransform = (_canvas.transform as RectTransform);

        // キャンバスのワールド座標を取得
        //
        Vector3 worldPos = rectTransform.TransformPoint(canvasPos);

        // ワールド座標をスクリーン座標に変換
        return RectTransformUtility.WorldToScreenPoint(_uiCamera, worldPos);

    }

    /// <summary>
    /// 指定したキャンバス座標にあるUIが押されたときの処理
    /// </summary>
    /// <param name="canvasPos">キャンバス座標</param>
    void ClickUIAt(Vector2 canvasPos)
    {
        // キャンバスかイベントシステムの参照がない場合は処理を行わない
        if (_graphicRaycaster == null || _eventSystem == null)
        {
            return;
        }

        // キャンバス座標からスクリーン座標を取得
        Vector2 screenPos = CanvasToScreenPosition(canvasPos);

        // EventSystemにあるpointerEventDataの一部を設定
        // 今のカーソルのデータを設定している
        var data = new PointerEventData(_eventSystem)
        {
            position = screenPos, // 座標をスクリーン座標に設定
            button = PointerEventData.InputButton.Left, // カーソルのボタンをLeftにする((
            clickTime = Time.unscaledTime, // ボタンを押した時間を設定(フレームに依存しない)
            clickCount = 1, // 押されたカウント数を設定
        };

        // レイキャストした結果を保存するList
        var results = new List<RaycastResult>();

        // 今のカーソルの情報からレイキャストする
        _graphicRaycaster.Raycast(data, results);

        // オブジェクトが見つからなかった場合はreturn
        if (results.Count == 0)
        {
            return;
        }

        // 最初のオブジェクトを取得
        var target = results[0].gameObject;

        // 押された時、押している時、離された時のイベントを一気に通知
        // 今のところ押されたらシーン遷移するので問題はない
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerClickHandler);
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerUpHandler);
    }
}

