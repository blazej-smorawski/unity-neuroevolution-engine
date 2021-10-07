using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Mutational
{
    void Mutate();
}

[System.Serializable]
public class Node : Mutational
{
    [SerializeField]
    private NeuralNetwork neuralNetwork;
    [SerializeField]
    private int id;
    [SerializeField]
    private int layer;
    [SerializeField]
    private float value;
    [SerializeField]
    private List<Edge> connectedEdges;

    public Node(NeuralNetwork neuralNetwork, int id, int value, int layer)
    {
        this.neuralNetwork = neuralNetwork;
        this.id = id;
        this.value = value;
        this.layer = layer;

        connectedEdges = new List<Edge>();
    }

    public int GetId()
    {
        return id;
    }

    public float GetValue()
    {
        return value;
    }

    public void AddValue(float addValue)
    {
        value += addValue;
        value = NeuralNetwork.NormalizeNodeValue(value);
    }

    //Not affected by smoothstep function!
    public void SetValue(float setValue)
    {
        value = setValue;
    }

    public bool IsConnectedWith(Node node)
    {
        foreach(Edge edge in connectedEdges)
        {
            if(edge.EndsIn(node))
            {
                return true;
            }
        }
        return false;
    }

    public List<Edge> GetConnectedEdges()
    {
        return connectedEdges;
    }

    //Connect this to node
    public void Connect(Node node, float weight)
    {
        connectedEdges.Add(new Edge(neuralNetwork, this, node, weight));
    }

    public void InfluenceConnectedNodes()
    {
        for(int i=0;i<connectedEdges.Count;i++)
        {
            connectedEdges[i].FireConnection();
        }
    }

    public void Mutate()
    {

    }
}
