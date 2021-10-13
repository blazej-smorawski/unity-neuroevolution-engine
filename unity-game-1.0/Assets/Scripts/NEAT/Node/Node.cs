using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Mutational
{
    void Mutate(NeuralNetwork neuralNetwork);
}

public class Node : Mutational
{
    private int id;
    private float value;
    private List<Edge> outgoingEdges;

    //Mutations
    public delegate void MutationDelegate(NeuralNetwork neuralNetwork);
    private List<MutationDelegate> mutations;

    public Node(int id, float value=0)
    {
        this.id = id;
        this.value = value;
        this.outgoingEdges = new List<Edge>();
        this.mutations = new List<MutationDelegate> {ConnectToRandomNode};
    }

    public int GetId()
    {
        return id;
    }

    public float GetValue()
    {
        return value;
    }

    public List<Edge> GetOutgoingEdges()
    {
        return outgoingEdges;
    }

    public void AddOutgoingEdge(Edge edge)
    {
        outgoingEdges.Add(edge);
    }

    public void RemoveOutgoingEdge(Edge edge)
    {
        outgoingEdges.Remove(edge);
    }

    public void AddValue(float addValue)
    {
        Debug.Log("NeuralNetwork|Node:" + GetId() + "->" + value + "+" + addValue);
        value += addValue;
        value = NeuralNetwork.NormalizeNodeValue(value);
    }

    ///<summary>
    ///Not affected by smoothstep function!
    ///</summary>
    public void SetValue(float setValue)
    {
        value = setValue;
    }

    public bool IsConnectedWith(Node node)
    {
        foreach(Edge edge in outgoingEdges)
        {
            if(edge.EndsIn(node) && edge.IsEnabled())
            {
                return true;
            }
        }
        return false;
    }

    public Edge GetEdgeWith(Node node)
    {
        foreach (Edge edge in outgoingEdges)
        {
            if (edge.EndsIn(node) && edge.IsEnabled())
            {
                return edge;
            }
        }
        return null;
    }

    ///<summary>
    /// Connect this ---> toNode
    /// </summary>
    public Edge Connect(int id,Node toNode, float weight)
    {
        Edge newEdge = new Edge(id, this, toNode, weight);
        outgoingEdges.Add(newEdge);
        return newEdge;
    }

    /// <summary>
    /// Node can only be activated once during one prediction
    /// </summary>
    /// <param name="neuralNetwork"></param>
    public void InfluenceConnectedNodes(NeuralNetwork neuralNetwork)
    {
        for (int i = 0; i < outgoingEdges.Count; i++)
        {
            Debug.Log("NeuralNetwork|Node: " + GetId() + " -> Influencing connections");
            outgoingEdges[i].FireConnection(neuralNetwork);
        }
    }

    public virtual void Mutate(NeuralNetwork neuralNetwork)
    {
        mutations[UnityEngine.Random.Range(0, mutations.Count)](neuralNetwork);
    }

    public void ConnectToRandomNode(NeuralNetwork neuralNetwork)
    {
        neuralNetwork.ConnectNodeToRandom(this);
    }
}
