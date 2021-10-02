using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    public override void ReadInput(InputAction.CallbackContext context)
    {
        Vector2 moveAxis = context.ReadValue<Vector2>();
        targetHorizontal = moveAxis.x;
        targetVertical = moveAxis.y;
    }
}
