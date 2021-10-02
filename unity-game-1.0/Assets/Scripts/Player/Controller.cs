using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    [Header("Movement")]
    protected float horizontal;
    protected float vertical;
    protected float targetHorizontal;
    protected float targetVertical;
    protected float walkBlendSpeed;
    protected float targetWalkBlendSpeed;
    public float acceleration;
    public float rotationSpeed;
    protected Transform cameraTarget;

    protected Animator animator;
    protected Vector3 rotationOffset;
    protected Vector3 direction;
    protected Vector3 newPostion;

    void Start()
    {
        SetVariables();
    }

    public virtual void SetVariables()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        MoveCharacter();
    }

    public abstract void ReadInput(InputAction.CallbackContext context);//Here targetHorizontal and targetVertical must be set!

    public virtual void MoveCharacter()
    {
        AccelerateCharacter();//I want speed to be non-linear, so this functions increases SPEED until it reaches TARGET_SPEED

        if ((targetHorizontal != 0 || targetVertical != 0))
        {
            direction = new Vector3(targetHorizontal, 0f, targetVertical);//Vector based on left joystick;
            targetWalkBlendSpeed = direction.magnitude;//Target speed=<0,1> because it is only affecting blend tree and blends between idle(0) and run(1) with walk in between those two

            rotationOffset = Camera.main.transform.TransformDirection(direction);//Move in camera space

            rotationOffset.y = 0f;//We do not want rotation in planes other than XZ
            transform.forward += Vector3.Lerp(transform.forward, rotationOffset, Time.deltaTime * rotationSpeed);//Character is rotated towards specific axis
        }
        else
        {
            targetWalkBlendSpeed = 0;//If input is equal to 0 we decelerate in order to smoothly stop and maybe transition to attack animation
        }
    }

    public void AccelerateCharacter()
    {
        if (walkBlendSpeed < targetWalkBlendSpeed)
        {
            walkBlendSpeed += acceleration * Time.deltaTime;
        }
        if (walkBlendSpeed > targetWalkBlendSpeed)
        {
            walkBlendSpeed -= acceleration * Time.deltaTime;
        }

        animator.SetFloat("walkBlend", walkBlendSpeed);//SPEED is an agrument of WALKBLEND blend tree
    }
}

//Błażej Smorawski 02.10.2021, e-mail: blazej.smorawski@gmail.com
