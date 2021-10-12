using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleInputEvaluator : InputEvaluator
{
    public override void UpdateEvalutator(Organism organism)
    {
        organism.brain.SetInput("Angle X", transform.rotation.x);
        organism.brain.SetInput("Angle Y", transform.rotation.y);
        organism.brain.SetInput("Angle Z", transform.rotation.z);
        Debug.Log("NeuralNetwork|Input: " + transform.rotation);
    }
}
