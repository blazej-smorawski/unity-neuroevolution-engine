using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    public override void ReadInput()
    {
        targetHorizontal = 0;//We set it to 0, so the character will stop if he has not received any diffrent directions
        targetVertical = 0;
        foreach (InputReader input in inputs)
        {
            input.ReadInput(this);
        }
    }
}
