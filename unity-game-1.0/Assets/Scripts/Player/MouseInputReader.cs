using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputReader : InputReader
{
    private Vector2 playerScreenPosition;
    private Vector2 mousePosition;
    private Vector2 mouseDirection;

    public override void ReadInput(Controller controller)
    {
        base.ReadInput(controller);

        playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        mousePosition = Input.mousePosition;
        mouseDirection = (mousePosition - playerScreenPosition).normalized;

        if (Input.GetButton("Fire2"))
        {
            controller.targetHorizontal = mouseDirection.x;
            controller.targetVertical = mouseDirection.y;
        }
        else
        {
            controller.targetHorizontal = 0;
            controller.targetVertical = 0;
        }
    }
}
