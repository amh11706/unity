using System.Runtime.CompilerServices;
using UnityEngine;

public class PrefixLogger
{
    private readonly string prefix;

    public PrefixLogger(string prefix)
    {
        this.prefix = prefix;
    }

    public void Log(string message, [CallerFilePath] string filePath = "")
    {
        Debug.Log($"{prefix}: {message}\nFile: {filePath}");
    }

    // Similarly for LogWarning and LogError
    public void LogWarning(string message, [CallerFilePath] string filePath = "")
    {
        Debug.LogWarning($"{prefix}: {message}\nFile: {filePath}");
    }

    public void LogError(string message, [CallerFilePath] string filePath = "")
    {
        Debug.LogError($"{prefix}: {message}\nFile: {filePath}");
    }
}
