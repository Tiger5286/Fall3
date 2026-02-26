using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _instance;

    //âπó 
    [Header("Volume")]
    [Range(0f, 1f)] public float BGMVolume = 1.0f;
    [Range(0f, 1f)] public float SEVolume = 1.0f;

    //AudioSource
    [Header("AudioSources")]
    [SerializeField] private AudioSource _BGMSource;
    [SerializeField] private AudioSource _SESource;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //BGMçƒê∂
    public void PlayBGM(AudioClip clip)
    {
        _BGMSource.clip = clip;
        _BGMSource.volume = BGMVolume;
        _BGMSource.loop = true;
        _BGMSource.Play();
    }

    //BGMí‚é~
    public void StopBGM()
    {
        _BGMSource.Stop();
    }

    //SEçƒê∂
    public void PlaySE(AudioClip clip)
    {
        _SESource.PlayOneShot(clip, SEVolume);
    }
}
