using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleFitnessEvaluator : FitnessEvaluator
{
    public bool inRightPosition = true;

    public override void UpdateEvalutator(Organism organism)
    {
        if(Vector3.Angle(transform.up, Vector3.up) > 30)
        {
            inRightPosition = false;
        }
        else
        {
            inRightPosition = true;
        }

        if(inRightPosition)
        {
            organism.neuralNetwork.fitness += Time.deltaTime;
        }
    }

    public override void Restart()
    {
        inRightPosition = true;
    }
}
