using UnityEngine;
using System.Collections;

public static class Log
{
    static bool LogErrors = false;

    public static void Make(string s)
    {
        if(LogErrors)
            Debug.Log(s);
    }
}
