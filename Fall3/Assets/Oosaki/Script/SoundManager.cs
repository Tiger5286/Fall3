using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioSource _seSource;

    //複数個のBGMを切り替えする可能性があるため配列で管理
    [SerializeField] AudioClip[] _bgmClips;
    [SerializeField] AudioClip[] _seClips;

    //歩行専用SE
    [SerializeField] AudioSource _walkSeSource;
    [SerializeField] AudioClip _walkSeClip;

    public void Awake()
    {
        if(Instance!=null&&Instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //シーンをまたいでも消えないようにする
        DontDestroyOnLoad(gameObject);

        AudioSource[] audioSources = GetComponents<AudioSource>();

        //AudioSourceが2つ以上あるときは、1つ目をBGM用、2つ目をSE用に変更
        if (audioSources.Length >= 2)
        {
            _bgmSource = audioSources[0];
            _seSource = audioSources[1];
        }
    }


    public void PlayBGM(int index)
    {
        if (_bgmClips.Length <= index) return;

        if (_bgmSource.clip == _bgmClips[index]) return;

        _bgmSource.Stop();
        _bgmSource.clip = _bgmClips[index];
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void PlaySe(int index)
    {
        if (_seClips == null) return;
        if (_seClips.Length <= index) return;
        if (_seClips[index] == null) return;
        if (_seSource == null) return;

        _seSource.PlayOneShot(_seClips[index]);
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void PlayWalkSe()
    {
        if (_walkSeSource == null || _walkSeClip == null) return;
        if (!_walkSeSource.isPlaying)
        {
            _walkSeSource.clip = _walkSeClip;
            _walkSeSource.loop = true;
            _walkSeSource.Play();
        }
    }

    //歩行SE停止
    public void StopWalkSe()
    {
        if (_walkSeSource == null) return;
        if (_walkSeSource.isPlaying)
        {
            _walkSeSource.Stop();
        }
    }

    //フェードアウトしてBGMを停止
    public void StopBGMFade(float fadeTime)
    {
        StartCoroutine(FadeOutBGM(fadeTime));
    }

    //だんだん音量を下げていく
    IEnumerator FadeOutBGM(float fadeTime)
    {
        float startVolume = _bgmSource.volume;
        while (_bgmSource.volume > 0)
        {
            _bgmSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        _bgmSource.Stop();
        _bgmSource.volume = startVolume;
    }
}
