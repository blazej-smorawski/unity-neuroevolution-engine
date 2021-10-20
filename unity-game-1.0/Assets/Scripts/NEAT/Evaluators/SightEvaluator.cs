using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightEvaluator : InputEvaluator
{
    public int inputNumber = 1;
    public float sightRange = 10f;
    private VisibleObject hitObject;

    public override void UpdateEvalutator(Organism organism)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, sightRange))
        {
            Debug.Log("Visible|Did Hit");
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * sightRange, Color.yellow);
            hitObject = hit.collider.gameObject.GetComponent<VisibleObject>();

            if (hitObject!=null)
            {
                Debug.Log("Visible|I See: "+hitObject.GetVisibleId());
                organism.brain.SetInput("Sight " + inputNumber, hitObject.GetVisibleId());
                organism.brain.SetInput("Distance " + inputNumber, hit.distance);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * sightRange, Color.white);
            Debug.Log("Visible|Did not Hit");
            organism.brain.SetInput("Sight " + inputNumber, 0);
            organism.brain.SetInput("Distance " + inputNumber, 0);
        }
    }
}
