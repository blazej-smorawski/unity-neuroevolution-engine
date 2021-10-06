using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadInputReader : InputReader
{
    public override void ReadInput(Controller controller)
    {
        base.ReadInput(controller);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            controller.targetHorizontal = Input.GetAxis("Horizontal");
            controller.targetVertical = Input.GetAxis("Vertical");
        }
    }
}
