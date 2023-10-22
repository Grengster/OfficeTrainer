using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLogs : MonoBehaviour
{
    public uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();
    public Text text = null;    

    void Start()
    {
        Debug.Log("Started up logging.");
    }

    void OnEnable()
    {
        
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    private void Update()
    {
        text.text = "\n" + string.Join("\n", myLogQueue.ToArray());

    }

}