//#define EDGE_DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will not be serialized on it's own. ALL serialization should be handled by NeuralNetwork
/// </summary>
public class Edge : Mutational
{
    private int id;
    private bool enabled;
    private Node from;
    private Node to;
    private float weight;

    //Mutations
    public delegate void MutationDelegate(NeuralNetwork neuralNetwork);
    private List<MutationDelegate> mutations;
    private List<float> mutationChances;

    /// <summary>
    /// Representation of connection to a node. It should be rewritten, but it's quite challenging <br/>
    /// due to the fact that it has to be serializable. Using referene to targetNode looks and works <br/>
    /// great but it creates circular dependency, so :( <br/>
    /// </summary>
    public Edge(int id,Node from, Node to, float weight, bool enabled=true)
    {
        this.id = id;
        this.enabled = enabled;
        this.from = from;
        this.to = to;   
        this.weight = weight;
        this.mutations = new List<MutationDelegate>() { splitEdge, toggleEdge, setWeight, addWeight, };
        this.mutationChances = new List<float>() { 5,10,5,80 };
    }

    public int GetId()
    {
        return id;
    }
    
    public int GetFromId()
    {
        return from.GetId();
    }

    public int GetToId()
    {
        return to.GetId();
    }

    public Node GetFromNode()
    {
        return from;
    }

    public Node GetToNode()
    {
        return to;
    }

    public float GetWeight()
    {
        return weight;
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    public bool IsEnabled()
    {
        return enabled;
    }

    public bool EndsIn(Node node)
    {
        return to == node;
    }

    /// <summary>
    /// Split into new node or deactivate this connection
    /// </summary>
    public void Mutate(NeuralNetwork neuralNetwork)
    {
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        float sum = 0;
        for (int i = 0; i < mutationChances.Count; i++)
        {
            sum+=mutationChances[i];
            if (sum >= randomNumber)
            {
                mutations[i](neuralNetwork);//We trigger a random mutations
                return;
            }
        }
    }

    public void splitEdge(NeuralNetwork neuralNetwork)
    {
        if (enabled)
        {
#if DEBUG_PLUS
            Debug.Log("NeuralNetwork|New Node:" + from.GetId() + "-*-" + to.GetId());
#endif
            neuralNetwork.SplitWithNewNode(this);
        }
    }

    /// <summary>
    /// It removes the edge from all structures it's in.<br/>
    /// It should be checked if it works better than just toggling<br/>
    /// bool enabled.<br/>
    /// </summary>
    /// <param name="neuralNetwork"></param>
    public void toggleEdge(NeuralNetwork neuralNetwork)
    {
        //enabled = !enabled;
        neuralNetwork.RemoveEdge(this);
        from = null;
        enabled = false;
    }

    public void addWeight(NeuralNetwork neuralNetwork)
    {
        weight+=UnityEngine.Random.Range(-NeuralNetwork.maxMutationAddValue, NeuralNetwork.maxMutationAddValue);
    }

    public void setWeight(NeuralNetwork neuralNetwork)
    {
        weight = UnityEngine.Random.Range(-NeuralNetwork.maxValue, NeuralNetwork.maxValue);
    }

    /// <summary>
    /// We fire connection to node with to Node additively.
    /// </summary>
    public void FireConnection(NeuralNetwork neuralNetwork)
    {
        if (enabled)
        {
            to.AddValue(from.GetValue() * weight);
            //to.InfluenceConnectedNodes(neuralNetwork);//One activation activates other neurons
        }
    }
}
