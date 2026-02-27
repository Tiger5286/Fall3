using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource _bgmSource;
    public AudioSource _seSource;

    public void Awake()
    {
        _bgmSource = GetComponent<AudioSource>();
        _seSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(_bgmSource!=null)
        {
            PlayBGM();
        }
        if(_seSource!=null)
        {
            PlayerSe();
        }
    }

    public void PlayBGM()
    {
        _bgmSource.Play();
    }

    public void PlayerSe()
    {
        _seSource.Play();
    }
}
