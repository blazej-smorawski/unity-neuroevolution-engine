using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    public override void ReadInput()
    {
        foreach(InputReader input in inputs)
        {
            input.ReadInput(this);
        }
    }
}
