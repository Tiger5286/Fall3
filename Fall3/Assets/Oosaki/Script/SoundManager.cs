using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioSource _seSource;

    [SerializeField] AudioClip _bgmClip;
    [SerializeField] AudioClip _seClip;

    public void Awake()
    {
        if(Instance!=null&Instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //ÉVÅ[ÉìÇÇ‹ÇΩÇ¢Ç≈Ç‡è¡Ç¶Ç»Ç¢ÇÊÇ§Ç…Ç∑ÇÈ
        DontDestroyOnLoad(gameObject);

        AudioSource[] audioSources = GetComponents<AudioSource>();

        _bgmSource = audioSources[0];
        _seSource = audioSources[1];
    }


    public void PlayBGM()
    {
        if(!_bgmSource.isPlaying)
        {
            _bgmSource.clip = _bgmClip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }
    }

    public void PlayerSe()
    {
        _seSource.PlayOneShot(_seClip);
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }
}
