using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightEvaluator : InputEvaluator
{
    public int inputNumber = 1;
    public float sightRange = 10f;
    public int rewardedId = -1;
    public float rewardedAmount = 0.5f;
    private VisibleObject hitObject;

    public override void UpdateEvalutator(Organism organism)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, sightRange))
        {
            //Debug.Log("Visible|Did Hit");
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * sightRange, Color.yellow);
            hitObject = hit.collider.gameObject.GetComponent<VisibleObject>();

            if (hitObject!=null)
            {
                //Debug.Log("Visible|I See: "+hitObject.GetVisibleId());
                organism.neuralNetwork.SetInput("Sight " + inputNumber, hitObject.GetVisibleId());
                organism.neuralNetwork.SetInput("Distance " + inputNumber, hit.distance);
                organism.neuralNetwork.SetInput("Distance " + inputNumber, 0);

                if (hitObject.GetVisibleId()==rewardedId)
                {
                    organism.neuralNetwork.fitness += rewardedAmount * Time.deltaTime;
                }
            }
            else
            {
                organism.neuralNetwork.SetInput("Sight " + inputNumber, -1);
                organism.neuralNetwork.SetInput("Distance " + inputNumber, 0);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * sightRange, Color.white);
            //Debug.Log("Visible|Did not Hit");
            organism.neuralNetwork.SetInput("Sight " + inputNumber, -1);
            organism.neuralNetwork.SetInput("Distance " + inputNumber, 0);
        }
    }
}
