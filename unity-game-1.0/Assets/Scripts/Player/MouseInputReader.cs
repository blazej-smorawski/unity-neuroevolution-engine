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
        mouseDirection = (mousePosition - playerScreenPosition);

        if (Input.GetButton("Fire2") && mouseDirection.magnitude>Screen.height*0.1f)//Screen.height condition added in order to avoid super fast rotation when pointing at player's character's feet
        {
            controller.targetHorizontal = mouseDirection.normalized.x;
            controller.targetVertical = mouseDirection.normalized.y;
        }
    }
}
