using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Profiling;

public class AnalyticsManager : MonoBehaviourSingleton<AnalyticsManager>
{
    // Start is called before the first frame update
    async void Start()
    {
#if UNITY_EDITOR || DEBUG
        Profiler.logFile = "profilerLog";
        Profiler.enabled = false;
        Profiler.enableBinaryLog = true;

        await UnityServices.InitializeAsync();

        AnalyticsService.Instance.StartDataCollection();
        PerformanceReporting.enabled = true;
#endif
    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR || DEBUG
        Debug.Log("Profiler stopped");
        Profiler.enabled = false;
        Profiler.enableBinaryLog = false;
#endif
    }

}
