﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace SystemHeat
{
  public enum LogType
  {
    UI,
    Settings,
    Modules,
    Overlay,
    Simulator,
    Any
  }
  public static class Utils
  {
    public static string logTag = "SystemHeat";

    /// <summary>
    /// Log a message with the mod name tag prefixed
    /// </summary>
    /// <param name="str">message string </param>
    public static void Log(string str, LogType logType)
    {
      bool doLog = false;
      if (logType == LogType.Settings && SystemHeatSettings.DebugSettings) doLog = true;
      if (logType == LogType.UI && SystemHeatSettings.DebugUI) doLog = true;
      if (logType == LogType.Modules && SystemHeatSettings.DebugModules) doLog = true;
      if (logType == LogType.Overlay && SystemHeatSettings.DebugOverlay) doLog = true;
      if (logType == LogType.Simulator && SystemHeatSettings.DebugSimulation) doLog = true;
      if (logType == LogType.Any) doLog = true;

      if (doLog)
        Debug.Log(String.Format("[{0}]{1}", logTag, str));
    }

    public static void Log(string str)
    {
      Debug.Log(String.Format("[{0}]{1}", logTag, str));
    }

      public static void LogWarning(string toLog)
    {
      Debug.LogWarning(String.Format("[{0}]{1}", logTag, toLog));
    }
    public static void LogError(string toLog)
    {
      Debug.LogError(String.Format("[{0}]{1}", logTag, toLog));
    }


    // <summary>
    /// Return true if the Part Action Window for this part is shown, false otherwise
    /// </summary>
    public static bool IsPAWVisible(this Part part)
    {
      return part.PartActionWindow != null && part.PartActionWindow.isActiveAndEnabled;
    }


    public static string ToSI(float d, string format = null, float factor= 1000f)
    {
      if (d == 0.0)
        return d.ToString(format);

      char[] incPrefixes = new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
      char[] decPrefixes = new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

      d *= factor;

      int degree = Mathf.Clamp((int)Math.Floor(Math.Log10(Math.Abs(d)) / 3), -8, 8);
      if (degree == 0)
        return d.ToString(format);

      double scaled = d * Math.Pow(1000, -degree);

      char? prefix = null;

      switch (Math.Sign(degree))
      {
        case 1: prefix = incPrefixes[degree - 1]; break;
        case -1: prefix = decPrefixes[-degree - 1]; break;
      }

      return scaled.ToString(format) + " " + prefix;
    }

    /// <summary>
    /// Get a reference in a child of a type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static T FindChildOfType<T>(string name, Transform parent)
    {
      T result = default(T);
      try
      {
        result = parent.FindDeepChild(name).GetComponent<T>();
      }
      catch (NullReferenceException e)
      {
        Debug.LogError($"Couldn't find {name} in children of {parent.name}");
      }
      return result;
    }

  }

  public static class TransformDeepChildExtension
  {
    /// <summary>
    /// Find a child recursively by name
    /// </summary>
    /// <param name="aParent"></param>
    /// <param name="aName"></param>
    /// <returns></returns>
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
      Queue<Transform> queue = new Queue<Transform>();
      queue.Enqueue(aParent);
      while (queue.Count > 0)
      {
        var c = queue.Dequeue();
        if (c.name == aName)
          return c;
        foreach (Transform t in c)
          queue.Enqueue(t);
      }
      return null;
    }
  }
}
