using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] RectTransform _target;


    float _margin = 0.5f;
    float _speed = 1f;

    Vector3 _baseScale;
    Coroutine _moveCoroutine;

    private void Awake()
    {
        if(_target == null) _target = transform as RectTransform;
        _baseScale = _target.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_moveCoroutine == null) _moveCoroutine = StartCoroutine(Move());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(Move());
            _moveCoroutine = null;
        }
        _target.localScale = _baseScale;
    }
    IEnumerator Move()
    {
        while (true)
        {
            float scale = 1.0f + Mathf.Sin(Time.deltaTime * Mathf.PI * 2.0f * _speed) * _margin;
            _target.localScale = _baseScale * scale;
            yield return null;
        }
    }
}
