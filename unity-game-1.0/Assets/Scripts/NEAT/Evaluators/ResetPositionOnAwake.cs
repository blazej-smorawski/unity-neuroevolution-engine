using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionOnAwake : MonoBehaviour
{
    public Transform startTransform;
    private void Awake()
    {
        //transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;
    }
}
