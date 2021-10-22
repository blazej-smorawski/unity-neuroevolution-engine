using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Visible
{
    public int GetVisibleId();
}

public class VisibleObject : MonoBehaviour, Visible
{
    public int visibleId;

    public int GetVisibleId()
    {
        return visibleId;
    }

    public void Start()
    {
        if(GetComponent<BoxCollider>() == null)
        {
            Debug.Log("Visible|No BoxCollider on VisibleObject!!!");
        }
    }
}
