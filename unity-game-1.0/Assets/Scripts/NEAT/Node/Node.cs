//#define NODE_DEBUG

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
    private List<float> mutationChances;

    public Node(int id, float value=0)
    {
        this.id = id;
        this.value = value;
        this.outgoingEdges = new List<Edge>();
        this.mutations = new List<MutationDelegate> {ConnectToRandomNode, DisconnectFromRandomNode};
        this.mutationChances = new List<float>() { 50f, 50f };
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
        #if NODE_DEBUG
        Debug.Log("NeuralNetwork|Node:" + GetId() + "->" + value + "+" + addValue);
        #endif

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
            #if NODE_DEBUG
            Debug.Log("NeuralNetwork|Node: " + GetId() + " -> Influencing connections");
            #endif

            outgoingEdges[i].FireConnection(neuralNetwork);
        }
    }

    public virtual void Mutate(NeuralNetwork neuralNetwork)
    {
        //mutations[UnityEngine.Random.Range(0, mutations.Count)](neuralNetwork);

        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        float sum = 0;
        for (int i = 0; i < mutationChances.Count; i++)
        {
            sum += mutationChances[i];
            if (sum >= randomNumber)
            {
                mutations[i](neuralNetwork);//We trigger a random mutations
                return;
            }
        }
    }

    public void ConnectToRandomNode(NeuralNetwork neuralNetwork)
    {
        neuralNetwork.ConnectNodeToRandom(this);
    }

    public void DisconnectFromRandomNode(NeuralNetwork neuralNetwork)
    {
        if (outgoingEdges.Count != 0)
        {
            neuralNetwork.RemoveEdge(outgoingEdges[UnityEngine.Random.Range(0, outgoingEdges.Count)]);
        }
    }
}
