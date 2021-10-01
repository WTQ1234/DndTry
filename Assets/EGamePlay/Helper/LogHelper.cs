using System;

public static class Log
{
    public static void Print(params object[] log)
    {
        string logg = "";
        for(int i = 0; i < log.Length; i++)
        {
            logg = logg + " , " + log[i].ToString();
        }
        UnityEngine.Debug.Log(logg);
    }

    public static void Debug(string log)
    {
        UnityEngine.Debug.Log(log);
    }

    public static void Error(string log)
    {
        UnityEngine.Debug.LogError(log);
    }

    public static void Error(Exception e)
    {
        UnityEngine.Debug.LogException(e);
    }

    public static void Info(string log)
    {
        UnityEngine.Debug.Log(log);
    }
}
