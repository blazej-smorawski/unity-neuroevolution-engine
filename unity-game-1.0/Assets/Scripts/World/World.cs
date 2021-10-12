using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public float timeScale = 1f;
    void Start()
    {
        Time.timeScale = timeScale;
    }
}
