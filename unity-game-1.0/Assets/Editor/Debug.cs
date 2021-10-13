#if DEBUG
#define VERBOSE
#endif

#if !UNITY_EDITOR && !VERBOSE
 using UnityEngine;
 using System.Diagnostics;
 using System.Runtime.InteropServices;
 
 public static class Debug {
 
     [Conditional("VERBOSE_EXTRA")]
     public static void LogInfo(string message, UnityEngine.Object obj = null) {
         UnityEngine.Debug.Log(message, obj);
     }
 
     [Conditional("VERBOSE")]
     public static void Log(string message, UnityEngine.Object obj = null) {
         UnityEngine.Debug.Log(message, obj);
     }
 
     public static void LogWarning(string message, UnityEngine.Object obj = null) {
         UnityEngine.Debug.LogWarning(message, obj);
     }
 
     public static void LogError(string message, UnityEngine.Object obj = null) {
         UnityEngine.Debug.LogError(message, obj);
     }
 
     public static void LogException(System.Exception e) {
         UnityEngine.Debug.LogError(e.Message);
     }
 }
#endif