using System;
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
    private int id;
    [SerializeField]
    private int layer;
    [SerializeField]
    private float value;
    [SerializeField]
    private List<Edge> connectedEdges;

    public Node(int id, int value, int layer)
    {
        this.id = id;
        this.value = value;
        this.layer = layer;

        connectedEdges = new List<Edge>();
    }

    public int GetId()
    {
        return id;
    }

    public int GetLayer()
    {
        return layer;
    }

    public float GetValue()
    {
        return value;
    }

    public void AddValue(float addValue)
    {
        Debug.Log("NeuralNetwork|Node:" + GetId() + "->" + value + "+" + addValue);
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
        connectedEdges.Add(new Edge(id, node.GetId(), layer, weight));
    }

    public void InfluenceConnectedNodes(NeuralNetwork neuralNetwork)
    {
        for(int i=0;i<connectedEdges.Count;i++)
        {
            connectedEdges[i].FireConnection(neuralNetwork,this);
        }
    }

    public void Mutate()
    {

    }
}
