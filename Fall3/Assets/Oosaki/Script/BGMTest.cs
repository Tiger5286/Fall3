using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMTest : MonoBehaviour
{
    void Update()
    {
        //Element1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SoundManager.Instance.PlayBGM(0);
        }

        //Element1
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SoundManager.Instance.PlayBGM(1);
        }

        //BGM‚¾‚ñ‚¾‚ñ’âŽ~
        if (Input.GetKeyDown(KeyCode.S))
        {
            SoundManager.Instance.StopBGMFade(2f);
        }

        //‘¦’âŽ~
        if (Input.GetKeyDown(KeyCode.K))
        {
            SoundManager.Instance.StopBGM();
        }
    }
}
