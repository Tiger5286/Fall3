using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneType
{
    Title,
    InGame,
    Result,
}

[System.Serializable]
public struct SceneEntry
{
    public SceneType _type;
    public GameManagerBase _manager;
}

public class SceneManagerKaminaga : MonoBehaviour
{
    // 現在のシーン
    private SceneType _currentScene;

    // カメラマネージャーの参照
    [SerializeField] private CameraManager _cameraManager;

    [Header("登録するシーン")]
    [SerializeField] private List<SceneEntry> _scenes = new();

    [Header("初期シーン")]
    [SerializeField] private SceneType _firstScene = SceneType.Title;

    private readonly Dictionary<SceneType, GameManagerBase> _sceneMap = new();

    private void Awake()
    {
        _sceneMap.Clear();
        
        foreach (var scene in _scenes)
        {
            if(scene._manager == null)
            {
                continue;
            }
            _sceneMap[scene._type] = scene._manager;
        }

        SetAllActive(false);
    }

    /// <summary>
    /// シーンを切り替える関数
    /// </summary>
    /// <param name="sceneType">切り替えるシーン</param>
    public void ChangeScene(SceneType sceneType)
    {
        if(!_sceneMap.TryGetValue(sceneType, out var nextScene) || nextScene == null)
        {
            return;
        }

        SetAllActive(false);

        nextScene.SetActive(true);
        _currentScene = sceneType;
    }

    void Start()
    {
        _currentScene = SceneType.Title;

        ChangeScene(_firstScene);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            ChangeScene(SceneType.Title);
            _cameraManager.SetActiveCamera(CameraType.TitleCamera);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeScene(SceneType.InGame);
            _cameraManager.SetActiveCamera(CameraType.InGameCamera);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeScene(SceneType.Result);
            _cameraManager.SetActiveCamera(CameraType.ResultCamera);
        }

    }

    private void SetAllActive(bool isActive)
    {
        foreach (var scene in _sceneMap.Values)
        {
            scene.SetActive(isActive);
        }
    }
}
