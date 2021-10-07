using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge : Mutational
{
    [SerializeField]
    private NeuralNetwork neuralNetwork;
    [SerializeField]
    private Node from;
    [SerializeField]
    private Node to;
    float weight;

    public Edge(NeuralNetwork neuralNetwork, Node from, Node to, float weight)
    {
        this.neuralNetwork = neuralNetwork;
        this.from = from;
        this.to = to;
        this.weight = weight;

    }

    public bool EndsIn(Node endNode)
    {
        return to == endNode;
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
