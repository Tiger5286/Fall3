using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneType
{
    Title,
    Ingame,
    Result,
}

public class SceneManagerKaminaga : MonoBehaviour
{
    // 現在のシーン
    private SceneType _currentScene;

    // カメラマネージャーの参照
    [SerializeField] private CameraManager _cameraManager;

    /// <summary>
    /// シーンを切り替える関数
    /// </summary>
    /// <param name="sceneType">切り替えるシーン</param>
    public void ChangeScene(SceneType sceneType)
    {
        _currentScene = sceneType;
    }

    void Start()
    {
        _currentScene = SceneType.Title;

        _cameraManager.SetActiveCamera(CameraType.TitleCamera);
    }

    void Update()
    {
        
    }
}
