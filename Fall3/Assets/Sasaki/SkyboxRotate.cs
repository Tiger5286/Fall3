using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 0.0f;
    Material skyMat;

    void Start()
    {
        skyMat = RenderSettings.skybox;
        if (skyMat == null)
        {
            Debug.LogWarning("RenderSettings.skyboxが未設定です");
        }
        else
        {
            skyMat.SetFloat("_Rotation", 0.0f);
        }
    }

    void Update()
    {
        if (skyMat == null) return;
        float rot = skyMat.GetFloat("_Rotation");
        rot += rotateSpeed * Time.deltaTime;
        skyMat.SetFloat("_Rotation", rot);
        DynamicGI.UpdateEnvironment();
    }
}
