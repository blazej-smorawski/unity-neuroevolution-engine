using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPoleFitnessEvaluator : FitnessEvaluator
{
    public bool inRightPosition = true;
    public List<Transform> transforms = new List<Transform>();

    public override void UpdateEvalutator(Organism organism)
    {
        inRightPosition = true;
        foreach (Transform t in transforms)
        {
            if (Vector3.Angle(t.up, Vector3.up) > 30)
            {
                inRightPosition = false;
                break;
            }
        }

        if (inRightPosition)
        {
            organism.neuralNetwork.fitness += Time.deltaTime;
        }
    }

    public override void Restart()
    {
        inRightPosition = true;
    }
}
