using System.Collections;
using System.Collections.Generic;

public interface Mutational
{
    void Mutate();
}

[System.Serializable]
public class Node : Mutational
{
    private NeuralNetwork neuralNetwork;
    private int id;
    private int layer;
    private float value;
    List<Edge> connectedNodes;

    public Node(NeuralNetwork neuralNetwork, int id, int value, int layer)
    {
        this.neuralNetwork = neuralNetwork;
        this.id = id;
        this.value = value;
        this.layer = layer;
    }

    public float GetValue()
    {
        return value;
    }

    //Maybe add some smoothstep?
    public void AddValue(float addValue)
    {
        value += addValue;
        value = NeuralNetwork.NormalizeNodeValue(value);
    }

    public void InfluenceConnectedNodes()
    {
        for(int i=0;i<connectedNodes.Count;i++)
        {
            connectedNodes[i].FireConnection();
        }
    }

    public void Mutate()
    {

    }
}
