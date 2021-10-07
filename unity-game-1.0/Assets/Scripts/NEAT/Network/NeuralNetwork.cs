using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNetwork
{
    public List<NeuralLayer> layers;

    /// <summary>
    /// Representation of connection of a neural network <br/>
    /// with focus on possibility to mutate new edges, nodes etc.
    /// </summary>
    public NeuralNetwork(int inputsAmount, int outputsAmount, string options)
    {
        Debug.Log("NeuralNetwork|NeuralNetwork(" + inputsAmount + ", " + outputsAmount + ")");

        layers = new List<NeuralLayer>();
        layers.Add(new NeuralLayer());
        layers.Add(new NeuralLayer());

        int id=0;

        for (int i=0;i<inputsAmount;i++)
        {
            layers[0].nodes.Add(new Node(id++, 1, 0));
        }

        for (int i = 0; i < outputsAmount; i++)
        {
            layers[1].nodes.Add(new Node(id++, 0, 0));
        }

        if (options.Contains("r"))
        {
            for (int i = 0; i < inputsAmount; i++)
            {
                layers[0].nodes[i].Connect(layers[1].nodes[Random.Range(0, outputsAmount)], Random.Range(0f, 1f));

            }
        }

        //Should we connect them to the output? Right now I think that we can start without any, so the first connections come from mutations
    }

    //Normalize values, so we do not end up with some crazy values or even worse - NaN's
    public static float NormalizeNodeValue(float value)
    {
        return 2*10 / Mathf.PI * Mathf.Atan(value);
    }

    public NeuralLayer GetInputs()
    {
        return layers[0];
    }

    public NeuralLayer GetOutputs()
    {
        return layers[layers.Count-1];
    }

    public Node GetNodeById(int Id)
    {
        foreach(NeuralLayer layer in layers)
        {
            foreach(Node node in layer.nodes)
            {
                if(node.GetId() == Id)
                {
                    return node;
                }
            }
        }
        return null;
    }

    //Finds node in specific layer. NOT THE NEXT ONE.
    public Node GetNodeById(int Id, int layerId)
    {
        foreach (Node node in layers[layerId].nodes)
        {
            if (node.GetId() == Id)
            {
                return node;
            }
        }
        return null;
    }

    ///<summary>We have guarantee that count of layers will NOT change during Predict()</summary>
    public void Predict()
    {
        for(int i=0;i<layers.Count;i++)
        {
            for (int j = 0; j < layers[i].nodes.Count; j++)
            {
                layers[i].nodes[j].InfluenceConnectedNodes(this);
            }
        }
    }

    public void Serialize(string path)
    {

    }
}

///<summary> Layer of neurons. As simple as that.</summary>
[System.Serializable]
public class NeuralLayer
{
    public NeuralLayer()
    {
        nodes = new List<Node>();
    }

    public List<Node> nodes;
}
