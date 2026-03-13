using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImageController : MonoBehaviour
{
    // 定数定義
    // 振動する幅
    private const float kVibrateRange = 7.0f;

    // 振動する時間
    private const float kVibrateTime = 2.0f;

    // 振動する速さ
    private const float kVibrateSpeed = 5.0f;

    [SerializeField] RectTransform _target;

    private float _vibrateCount = 0.0f;
    private float _expandCount = 0.0f;
    private float _downCount = 0.0f;
    private float _expandSpeed = 0.0f;

    private bool _isVibrate = false;
    private bool _isExpand = false;
    private bool _isDown = false;

    private Vector3 _basePos; // 初期座標
    private Vector3 _vibPos; // 振動するための値
    private Vector3 _downPos; // 落ちるように動かすときの開始位置

    private Vector3 _baseScale;


    private void Awake()
    {
        if (_target == null) _target = transform as RectTransform;
        _basePos = _target.localPosition;
        _baseScale = _target.localScale;

        _vibPos = _basePos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vibrate();
        Expand();
        Down();
    }

    /// <summary>
    /// 画像を震えさせるときに使用する
    /// </summary>
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

    /// <summary>
    /// 画像を拡大させるときに使用する
    /// </summary>
    public void StartExpand()
    {
        if(_isExpand)
        {
            _target.localScale = _baseScale;
        }
        _isExpand = true;
        _expandCount = 0.0f;
        _expandSpeed = 0.5f;
    }

    /// <summary>
    /// 画像を落ちる感じで動かすときに使用する
    /// </summary>
    public void StartDown()
    {
        _downPos = _basePos + new Vector3(0.0f, 400.0f, 0.0f);

        _downCount = 0.0f;

        _isDown = true;
        _target.localPosition = _basePos;
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
                _target.localPosition = _basePos;
                _vibPos = _basePos;
                _isVibrate = false;
            }
        }
    }

    private void Expand()
    {
        if(_isExpand)
        {
            _expandCount += Time.deltaTime;
            float expand = 1.0f + _expandCount * _expandSpeed;

            if(_target.localScale.x >= 1.5f)
            {
                _expandSpeed = 0.003f;
            }

            _target.localScale *= expand;

            if(_expandCount >= 2.0f)
            {
                _target.localScale = _baseScale;
                _isExpand = false;
            }
        }
    }

    private void Down()
    {
        if (_isDown)
        {
            _downCount += Time.deltaTime;
            _target.localPosition = Vector3.Lerp(_downPos, _basePos, _downCount * 10.0f);

            if (_target.localPosition == _basePos)
            {
                _target.localPosition = _basePos;
                _isDown = false;
            }
        }
    }
}
