using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageController : MonoBehaviour
{
    // ’è”’è‹`
    // U“®‚·‚é•
    private const float kVibrateRange = 7.0f;

    // U“®‚·‚éŽžŠÔ
    private const float kVibrateTime = 2.0f;

    // U“®‚·‚é‘¬‚³
    private const float kVibrateSpeed = 5.0f;

    [SerializeField] RectTransform _target;

    private float _vibrateCount = 0.0f;

    private bool _isVibrate = false;

    private Vector3 _basePos;

    private Vector3 _vibPos;

    private void Awake()
    {
        if (_target == null) _target = transform as RectTransform;
        _basePos = _target.localPosition;

        _vibPos = _basePos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vibrate();
    }

    public void StartVibrate()
    {
        if(_isVibrate)
        {
            _target.localPosition = _basePos;
            _vibPos = _basePos;
        }
        _isVibrate = true;
        _vibrateCount = 0.0f;
    }

    private void Vibrate()
    {
        if (_isVibrate)
        {
            _vibrateCount += Time.deltaTime * kVibrateSpeed;
            float vibrate = Mathf.Sin(_vibrateCount * Mathf.PI * 2.0f) * kVibrateRange;

            _vibPos = _basePos;
            _vibPos.x = _basePos.x + vibrate;

            _target.localPosition = _vibPos;

            if (_vibrateCount >= kVibrateTime)
            {
                _isVibrate = false;
            }
        }
        else
        {
            _target.localPosition = _basePos;
            _vibPos = _basePos;
        }
    }
}
