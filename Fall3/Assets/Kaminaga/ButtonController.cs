using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] RectTransform _target;


    float _margin = 0.05f;
    float _speed = 1.5f;

    Vector3 _baseScale;
    Coroutine _moveCoroutine;
    bool _isSelected = false;

    private void Awake()
    {
        if(_target == null) _target = transform as RectTransform;
        _baseScale = _target.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(_isSelected) return;
        _isSelected = true;
        if (_moveCoroutine == null) _moveCoroutine = StartCoroutine(Move());
        Debug.Log("ì¸Ç¡ÇΩ");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
        if (_moveCoroutine != null)
        {
            StopCoroutine(Move());
            _moveCoroutine = null;
        }
        Debug.Log("èoÇΩ");

        _target.localScale = _baseScale;
    }
    IEnumerator Move()
    {
        //float scale = 1.0f + Mathf.Sin(Time.deltaTime * Mathf.PI * 2.0f * _speed) * _margin;
        //_target.localScale = _baseScale * scale;
        //yield return null;

        float time = 0.0f;
        while (_isSelected)
        {
            time += Time.unscaledDeltaTime * _speed;
            float scale = 1.0f + Mathf.Sin(time * Mathf.PI * 2.0f) * _margin;
            _target.localScale = _baseScale * scale;
            yield return null;
        }
        _target.localScale = _baseScale;
    }
}
