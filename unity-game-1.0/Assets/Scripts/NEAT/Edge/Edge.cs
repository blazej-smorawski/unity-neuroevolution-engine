using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge : Mutational
{
    private NeuralNetwork neuralNetwork;
    private Node from;
    private Node to;
    float weight;

    public Edge()
    {

    }
    //Triggered after reproduction of organism
    public void Mutate()
    {

    }

    public void SplitWithNewNode()
    {

    }

    //
    public void FireConnection()
    {
        to.AddValue(weight * from.GetValue());
    }
}
