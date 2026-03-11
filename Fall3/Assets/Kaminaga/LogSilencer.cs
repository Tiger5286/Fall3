using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSilencer : MonoBehaviour
{

    private ILogHandler _prevHandler;

    private void OnEnable()
    {
        _prevHandler = Debug.unityLogger.logHandler;
        
        Application.logMessageReceived += OnLogMessage;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLogMessage;
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if(condition.Contains("User index is invalid"))
        {
            return;
        }

        Debug.unityLogger.logHandler.LogFormat(type, null, condition);
    }

}
