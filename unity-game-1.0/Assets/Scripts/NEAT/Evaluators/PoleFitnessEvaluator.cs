using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleFitnessEvaluator : FitnessEvaluator
{
    public bool inRightPosition = true;

    public override void UpdateEvalutator(Organism organism)
    {
        if(transform.rotation.eulerAngles.x > 85 || transform.rotation.eulerAngles.x < -85)
        {
            inRightPosition = false;
        }

        if(inRightPosition)
        {
            organism.fitness += Time.deltaTime;
        }
    }
}
