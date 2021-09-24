using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Mkey
{
    public class EditorUtils : Editor
    {
      public static void ClearConsole()
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
    }
}

