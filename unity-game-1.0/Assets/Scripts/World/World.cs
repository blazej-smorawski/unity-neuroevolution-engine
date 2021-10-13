using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public bool logs = false;
    public float timeScale = 1f;

    void Start()
    {
        Debug.unityLogger.logEnabled = logs;
        Time.timeScale = timeScale;
    }
}
