using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionOnAwake : Resetter
{
    public Transform startTransform;
    public Rigidbody rigidbody;
    private void Awake()
    {
        //transform.position = startTransform.position;
        //transform.rotation = startTransform.rotation;
        //rigidbody.rotation = startTransform.rotation;
        //rigidbody.position = startTransform.position;
    }

    public override void Reset()
    {
        rigidbody.rotation = startTransform.rotation;
        rigidbody.position = startTransform.position;
    }
}
