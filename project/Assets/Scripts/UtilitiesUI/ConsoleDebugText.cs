using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ConsoleDebugText : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "";
    }
    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }

    void LogCallback(string logString, string stackTrace, LogType type)
    {
        textMeshPro.text += logString + "\r\n\n";
    }
}
