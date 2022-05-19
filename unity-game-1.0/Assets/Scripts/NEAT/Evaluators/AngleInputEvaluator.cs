using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleInputEvaluator : InputEvaluator
{
    public string suffix = "";

    public override void UpdateEvalutator(Organism organism)
    {
        organism.neuralNetwork.SetInput("Angle X" + suffix, transform.rotation.x);
        organism.neuralNetwork.SetInput("Angle Y" + suffix, transform.rotation.y);
        organism.neuralNetwork.SetInput("Angle Z" + suffix, transform.rotation.z);
        //Debug.Log("NeuralNetwork|Input: " + transform.rotation);
    }
}
