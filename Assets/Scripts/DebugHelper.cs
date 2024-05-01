using UnityEngine;
using System.Runtime.CompilerServices;

public class DebugHelper : MonoBehaviour
{
    public static void Log(object message, 
        [CallerMemberName] string memberName = "", 
        [CallerLineNumber] int lineNumber = 0)
    {
        Debug.Log($"{message} (Function: {memberName}, Line: {lineNumber})");
    }

    void Start()
    {
        Log("Start method called.");
    }
}
