using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNetwork
{

    public List<List<Node>> layers;
    
    public NeuralNetwork(int inputsAmount, int outputsAmount)
    {
        Debug.Log("NeuralNetwork|NeuralNetwork(" + inputsAmount + ", " + outputsAmount + ")");
    }

    //Normalize values, so we do not end up with some crazy values or even worse - NaN's
    public static float NormalizeNodeValue(float value)
    {
        return 2 / Mathf.PI * Mathf.Atan(value);
    }

    public List<Node> GetInputs()
    {
        return layers[0];
    }

    public List<Node> GetOutputs()
    {
        return layers[layers.Count-1];
    }

    //We have guarantee that count of layers will NOT change during Predict()
    public void Predict()
    {
        for(int i=0;i<layers.Count;i++)
        {
            for (int j = 0; j < layers[i].Count; j++)
            {
                layers[i][j].InfluenceConnectedNodes();
            }
        }
    }
}
