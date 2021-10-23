using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleInputEvaluator : InputEvaluator
{
    public override void UpdateEvalutator(Organism organism)
    {
        organism.neuralNetwork.SetInput("Angle X", transform.rotation.x);
        organism.neuralNetwork.SetInput("Angle Y", transform.rotation.y);
        organism.neuralNetwork.SetInput("Angle Z", transform.rotation.z);
        Debug.Log("NeuralNetwork|Input: " + transform.rotation);
    }
}
