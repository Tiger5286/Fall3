using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioSource _seSource;

    //複数個のBGMを切り化する可能性があるため配列で管理
    [SerializeField] AudioClip[] _bgmClips;

    [SerializeField] AudioClip _seClip;

    public void Awake()
    {
        if(Instance!=null&Instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //シーンをまたいでも消えないようにする
        DontDestroyOnLoad(gameObject);

        AudioSource[] audioSources = GetComponents<AudioSource>();

        _bgmSource = audioSources[0];
        _seSource = audioSources[1];
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

    public void PlayerSe()
    {
        _seSource.PlayOneShot(_seClip);
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
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
