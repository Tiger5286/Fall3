using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// カメラの種類
/// </summary>
public enum CameraType
{
    TitleCamera,    // タイトルのカメラ
    InGameCamera,   // ゲーム中のカメラ
    ResultCamera,   // リザルトのカメラ
}

/// <summary>
/// ゲーム内のカメラを管理するクラス
/// </summary>
public class CameraManager : MonoBehaviour
{
    // カメラの切り替えに必要なメインカメラのCinemachineBrainを取得
    [SerializeField] private CinemachineBrain _brain;

    // 切り替えるカメラを設定するためのリスト
    [SerializeField] private List<CinemachineVirtualCameraBase> _virtualCameras;

    [Header("Priority設計")]
    [SerializeField] private int _activePriority = 100; // アクティブなカメラのPriority
    [SerializeField] private int _inActivePriority = 0; // 非アクティブなカメラのPriority

    // カメラの種類と対応するCinemachineVirtualCameraBaseを管理する辞書
    private Dictionary<CameraType, CinemachineVirtualCameraBase> _cameraDictionary = new Dictionary<CameraType, CinemachineVirtualCameraBase>();

    // 現在アクティブなカメラの種類を保持しておく(現状未使用なので消すかも)
    private CameraType _currentCameraType;

    /// <summary>
    /// 最初に行う処理
    /// 使うカメラにあらかじめ名前を付けておくことでリストからカメラを設定するようにしています
    /// </summary>
    private void Awake()
    {
        // リスト内のカメラをすべて見る
        foreach (var camera in _virtualCameras)
        {
            // カメラがnullの場合は設定しない
            if (camera == null) continue;

            // リストのカメラの名前を取得
            var name = camera.name;

            // カメラの名前に応じて辞書に登録していく
            if (name.Contains("TitleCamera"))
            {
                // タイトルのカメラに設定
                _cameraDictionary[CameraType.TitleCamera] = camera;
            }
            else if (name.Contains("IngameCamera"))
            {
                // ゲーム中のカメラに設定
                _cameraDictionary[CameraType.InGameCamera] = camera;
            }
            else if (name.Contains("ResultCamera"))
            {
                // リザルトのカメラに設定
                _cameraDictionary[CameraType.ResultCamera] = camera;
            }
        }

        // 初期カメラはタイトルのカメラにする
        SetActiveCamera(CameraType.TitleCamera);

    }

    /// <summary>
    /// カメラを切り替えるための関数
    /// </summary>
    /// <param name="type">カメラの種類</param>
    public void SetActiveCamera(CameraType type)
    {
        // 現在のカメラの種類を更新
        _currentCameraType = type;

        // カメラの切り替え
        // Priorityの値を変更することで行う
        foreach (var camera in _virtualCameras)
        {
            // カメラがnullの場合は設定しない
            if (camera == null) continue;

            // 切り替えるカメラの種類と一致するカメラをアクティブなカメラにする
            if (_cameraDictionary[type] == camera)
            {
                camera.Priority = _activePriority;
            }
            else
            {
                // 切り替えるもの以外のカメラを非アクティブのPriorityにする
                camera.Priority = _inActivePriority;
            }
        }
    }

    /// <summary>
    /// 更新処理
    /// 現状デバッグ時にカメラを切り替えられるようにする処理のみ行っている
    /// </summary>
    public void Update()
    {
#if DEBUG
        // デバッグ時にカメラを切り替えられるようにしておく
        if (Input.GetKeyDown(KeyCode.C))
        {
            // タイトルのカメラに切り替える
            SetActiveCamera(CameraType.TitleCamera);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            // ゲーム中のカメラに切り替える
            SetActiveCamera(CameraType.InGameCamera);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // リザルトのカメラに切り替える
            SetActiveCamera(CameraType.ResultCamera);
        }
    }
#endif
}
