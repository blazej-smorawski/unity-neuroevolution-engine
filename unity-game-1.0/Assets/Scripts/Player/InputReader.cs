using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputReader : MonoBehaviour
{
    //Sets controller's horizontal and vertical and trigger functions like PerformAttack()
    public virtual void ReadInput(Controller controller)
    {
        if(Input.GetButton("Attack"))
        {
            controller.PerformAttack(0);
        }
    }
}
