using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private GraphicRaycaster _graphicRaycaster;
    [SerializeField] private EventSystem _eventSystem;

    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _clickAction;

    [SerializeField] private Button _button;

    private Vector2 _cursorPosCanvas;

    private Camera _uiCamera;

    private void OnEnable()
    {
        _moveAction?.action?.Enable();
        _clickAction?.action?.Enable();

        if(_eventSystem != null && _eventSystem.currentSelectedGameObject != null)
        {
            var target = _eventSystem.currentSelectedGameObject.GetComponent<RectTransform>();
            if (target != null)
            {
                _cursorPosCanvas = WorldToCanvasPosition(target.position);
            }
            else 
            {
                _cursorPosCanvas = GetCanvasCenter();
            }
        }
        else
        {
            _cursorPosCanvas = GetCanvasCenter();
        }
        ApplyCursorVisual();
    }

    private void OnDisable()
    {
        _moveAction?.action?.Disable();
        _clickAction?.action?.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_cursor == null || _canvas == null)
        {
            return;
        }

        Vector2 move = _moveAction?.action?.ReadValue<Vector2>() ?? Vector2.zero;
        if(move.sqrMagnitude > 0.0f)
        {
            _cursorPosCanvas += move * 1000.0f * Time.deltaTime;
        }

        SetCursorAnchoredPosition(_cursorPosCanvas);

        if(_clickAction?.action?.WasPerformedThisFrame() == true)
        {
            ClickUIAt(_cursorPosCanvas);
        }
    }

    private void ApplyCursorVisual()
    {
        if (_cursor != null)
        {
            _cursor.gameObject.SetActive(true);
            _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;
            SetCursorAnchoredPosition(_cursorPosCanvas);
        }
    }

    private void SetCursorAnchoredPosition(Vector2 canvasPos)
    {
        if(_cursor == null)
        {
            return;
        }

        _cursor.anchoredPosition = canvasPos;
    }

    private Vector2 GetCanvasCenter()
    {
        var rect = (_canvas.transform as RectTransform).rect;
        return rect.center;
    }

    private Vector2 WorldToCanvasPosition(Vector3 worldPos)
    {
        var rt = (_canvas.transform as RectTransform);
        if(_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null,worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPoint, null, out var local);
            return local;
        }
        else 
        {
            return worldPos;
        }
    }

    Vector2 CanvasToScreenPosition(Vector2 canvasPos)
    {
        var rt = (_canvas.transform as RectTransform);

        Vector3 worldPos = rt.TransformPoint(canvasPos);

        return RectTransformUtility.WorldToScreenPoint(_uiCamera, worldPos);

    }

    void ClickUIAt(Vector2 canvasPos)
    {
        if(_graphicRaycaster == null || _eventSystem == null)
        {
            return;
        }

        Vector2 screenPos = CanvasToScreenPosition(canvasPos);

        var data = new PointerEventData(_eventSystem)
        {
            position = screenPos,
            button = PointerEventData.InputButton.Left,
            clickTime = Time.unscaledTime,
            clickCount = 1,
        };

        var results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(data,results);

        if(results.Count == 0)
        {
            return;
        }

        var target = results[0].gameObject;
        Debug.Log(target.name);

        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerClickHandler);
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerUpHandler);


    }
}
