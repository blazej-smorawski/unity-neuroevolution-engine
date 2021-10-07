using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge : Mutational
{
    [SerializeField]
    private int nodeId;
    [SerializeField]
    private int targetNodeId;
    [SerializeField]
    private int nodeLayer;
    [SerializeField]
    private float weight;

    /// <summary>
    /// Representation of connection to a node. It should be rewritten, but it's quite challenging <br/>
    /// due to the fact that it has to be serializable. Using referene to targetNode looks and works <br/>
    /// great but it creates circular dependency, so :( <br/>
    /// </summary>
    public Edge(int nodeId, int targetNodeId, int nodeLayer, float weight)
    {
        this.nodeId = nodeId;
        this.targetNodeId = targetNodeId;
        this.nodeLayer = nodeLayer;
        this.weight = weight;
    }

    public bool EndsIn(Node endNode)
    {
        return targetNodeId == endNode.GetId();
    }
    //Triggered after reproduction of organism
    public void Mutate()
    {

    }

    public void SplitWithNewNode()
    {

    }

    /// <summary>
    /// We fire connection to node with targetNodeId which has to be in the next layer.
    /// </summary>
    public void FireConnection(NeuralNetwork neuralNetwork,Node from)
    {
        neuralNetwork.GetNodeById(targetNodeId,from.GetLayer()+1).AddValue(weight * from.GetValue());
    }
}
