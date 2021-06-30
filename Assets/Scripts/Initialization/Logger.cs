using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Logger : MonoBehaviour
{
    //#if !UNITY_EDITOR
    static string myLog = "";
    private string output;
    private bool shouldOpen;
    public RectTransform rt;

    void OnEnable()
    {
        shouldOpen = false;
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000) myLog = myLog.Substring(0, 4000);
    }

    void OnGUI()
    {
        //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
        if(shouldOpen) myLog = GUI.TextArea(new Rect(rt.transform.position.x, rt.transform.position.y, 250, 400), myLog);
        //GUI.TextArea
    }

    public void clickOnButton()
    {
        shouldOpen = !shouldOpen;
    }
}
