using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class NeuralNetwork : ISerializationCallbackReceiver
{
    public static float maxValue = 2f;
    public static float maxMutationAddValue = 0.1f;
    //Those fields WILL NOT be serialized by Unity automatically
    private List<Node> nodes;
    private List<Edge> edges;
    private List<Node> inputs;
    private List<Node> outputs;

    //This field WILL be serialized
    [SerializeField]
    static int nodeId = 0;//Each new node receives an unique Id number, so later when we can see their common parts and line up their genotypes
    [SerializeField]
    static int edgeId = 0;//Same as with nodes

    //Additional data for serialization
    [SerializeField]
    private List<SerializedNode> serializedNodes;
    [SerializeField]
    private List<SerializedEdge> serializedEdges;

    //Variables for Coroutines handling
    public CoroutineManager coroutineManager;

    /// <summary>
    /// Representation of connection of a neural network <br/>
    /// with focus on possibility to mutate new edges, nodes etc.
    /// </summary>
    public NeuralNetwork(List<string> inputNames, List<string> outputNames, string options):this()
    {
        Debug.Log("NeuralNetwork|NeuralNetwork(" + inputNames.Count + ", " + outputNames.Count + ")");

        coroutineManager = GameObject.FindObjectOfType<CoroutineManager>();

        if (coroutineManager == null) 
        {
            Debug.Log("NeuralNetwork|CoroutineManager is missing");
        }

        int tempId = 0;//We create this Id here, so input and output nodes have same id's across all networks

        foreach(string input in inputNames)
        {
            Node newNode = new InputNode(tempId++, 1, input);
            nodes.Add(newNode);
            inputs.Add(newNode);
        }

        foreach (string output in outputNames)
        {
            Node newNode = new OutputNode(tempId++, 1, output);
            nodes.Add(newNode);
            outputs.Add(newNode);
        }

        nodeId = tempId;//Nodes created after that will always have an unique id

        if (options.Contains("r"))//"r" argument stand for randomly connected input and output
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                ConnectNodes(inputs[i], outputs[UnityEngine.Random.Range(0, outputNames.Count)], .5f);
            }
        }
    }

    public NeuralNetwork(NeuralNetwork strongerParent, NeuralNetwork weakerParent):this()
    {
        foreach(Node node in strongerParent.GetNodes())
        {
            nodes.Add(DuplicateNode(node));
        }

        //Edges are ordered in increasing order in terms of id, so we can go over strongerEdges just once
        List<Edge> strongerEdges = strongerParent.GetEdges();
        List<Edge> weakerEdges = weakerParent.GetEdges();

        int strongerIndex = 0;
        int weakerIndex = 0;

        while(strongerIndex < strongerEdges.Count)
        {
            if(weakerIndex < weakerEdges.Count && strongerEdges[strongerIndex].GetId() > weakerEdges[weakerIndex].GetId() )//There might be a node in weakerEdges corresponding to strongerEdges[i]
            {
                ++weakerIndex;
                if (UnityEngine.Random.Range(0, 2) == 0)//Use connection from weaker parent
                {
                    Debug.Log("NeuralNetwork|Taking connection purely from weaker parent from id:" + weakerEdges[weakerIndex].GetFromNode().GetId() + " to:" + weakerEdges[weakerIndex].GetToNode().GetId());
                    Node from = nodes.Find(x => x.GetId() == weakerEdges[weakerIndex].GetFromNode().GetId());
                    Node to = nodes.Find(x => x.GetId() == weakerEdges[weakerIndex].GetToNode().GetId());

                    if (from == null)
                    {
                        from = DuplicateNode(weakerEdges[weakerIndex].GetFromNode());
                        nodes.Add(from);
                    }

                    if (to == null)
                    {
                        to = DuplicateNode(weakerEdges[weakerIndex].GetToNode());
                        nodes.Add(to);
                    }

                    //Recreate connection as it was in weaker parent
                    ConnectNodes(weakerEdges[weakerIndex].GetId(), from, to,
                        weakerEdges[weakerIndex].GetWeight(),
                        weakerEdges[weakerIndex].IsEnabled());
                }
            }
            else if(weakerIndex < weakerEdges.Count && strongerEdges[strongerIndex].GetId() == weakerEdges[weakerIndex].GetId())//We've got a match
            {
                Debug.Log("NeuralNetwork|Chosing randomly weight of connection:" + weakerEdges[weakerIndex].GetFromNode().GetId() + " to:" + weakerEdges[weakerIndex].GetToNode().GetId());
                if (UnityEngine.Random.Range(0, 2) == 0)//Use connection from
                {
                    Node from = nodes.Find(x => x.GetId() == strongerEdges[strongerIndex].GetFromNode().GetId());
                    Node to = nodes.Find(x => x.GetId() == strongerEdges[strongerIndex].GetToNode().GetId());

                    if (from == null)
                    {
                        from = DuplicateNode(strongerEdges[strongerIndex].GetFromNode());
                        nodes.Add(from);
                    }

                    if (to == null)
                    {
                        to = DuplicateNode(strongerEdges[strongerIndex].GetToNode());
                        nodes.Add(to);
                    }

                    //Recreate connection as it was in stronger parent
                    ConnectNodes(strongerEdges[strongerIndex].GetId(), from, to,
                        strongerEdges[strongerIndex].GetWeight(),
                        strongerEdges[strongerIndex].IsEnabled());
                }
                else//Copy connection from weaker parent
                {
                    Node from = nodes.Find(x => x.GetId() == weakerEdges[weakerIndex].GetFromNode().GetId());
                    Node to = nodes.Find(x => x.GetId() == weakerEdges[weakerIndex].GetToNode().GetId());

                    if (from == null)
                    {
                        from = DuplicateNode(weakerEdges[weakerIndex].GetFromNode());
                        nodes.Add(from);
                    }

                    if (to == null)
                    {
                        to = DuplicateNode(weakerEdges[weakerIndex].GetToNode());
                        nodes.Add(to);
                    }

                    //Recreate connection as it was in weaker parent
                    ConnectNodes(weakerEdges[weakerIndex].GetId(), from, to,
                        weakerEdges[weakerIndex].GetWeight(),
                        weakerEdges[weakerIndex].IsEnabled());
                }

                ++weakerIndex;
                ++strongerIndex;
            }
            else
            {
                Debug.Log("NeuralNetwork|Taking connection purely from stronger parent from id:" + strongerEdges[strongerIndex].GetFromNode().GetId() + " to:" + strongerEdges[strongerIndex].GetToNode().GetId());
                //Initialize nodes necessary for the connection
                Node from = nodes.Find(x => x.GetId() == strongerEdges[strongerIndex].GetFromNode().GetId());
                Node to = nodes.Find(x => x.GetId() == strongerEdges[strongerIndex].GetToNode().GetId());

                if (from == null)
                {
                    from = DuplicateNode(strongerEdges[strongerIndex].GetFromNode());
                    nodes.Add(from);
                }

                if (to == null)
                {
                    to = DuplicateNode(strongerEdges[strongerIndex].GetToNode());
                    nodes.Add(to);
                }

                //Recreate connection as it was in stronger parent
                ConnectNodes(strongerEdges[strongerIndex].GetId(), from, to,
                    strongerEdges[strongerIndex].GetWeight(),
                    strongerEdges[strongerIndex].IsEnabled());

                ++strongerIndex;
            }
        }
        
        inputs = new List<Node>(nodes.FindAll(x => x.GetType() == typeof(InputNode)));
        outputs = new List<Node>(nodes.FindAll(x => x.GetType() == typeof(OutputNode)));
    }

    public NeuralNetwork()
    {
        Debug.Log("NeuralNetwork|Running parametrless contstructor");
        nodes = new List<Node>();
        edges = new List<Edge>();
        inputs = new List<Node>();
        outputs = new List<Node>();
    }

    public Node GetNode(int id)
    {
        return nodes.Find(x => x.GetId() == id);
    }

    public List<Node> GetNodes()
    {
        return nodes;
    }

    /// <summary>
    /// Layers for presentation
    /// </summary>
    /// <returns>List of layers containing nodes</returns>
    public List<List<Node>> GetLayers()
    {
        if (nodes == null)
        {
            return null;
        }

        List<List<Node>> layers = new List<List<Node>>();
        layers.Add(new List<Node>(inputs));

        List<Node> normalNodes = new List<Node>(nodes);
        normalNodes.RemoveAll(x => x.GetType() == typeof(InputNode) || x.GetType() == typeof(OutputNode));

        int currentLayer = 1;
        layers.Add(new List<Node>());

        for (int i=0;i<normalNodes.Count;i++)
        {
            if (layers[currentLayer].Exists(x=>x.IsConnectedWith(normalNodes[i])||nodes[i].IsConnectedWith(x)))
            {
                currentLayer++;
                layers.Add(new List<Node>());
            }
            layers[currentLayer].Add(normalNodes[i]);
        }

        layers.Add(new List<Node>(outputs));

        return layers;
        
    }

    public List<Edge> GetEdges()
    {
        return edges;
    }

    ///<summary>
    ///Normalize values, so we do not end up with some crazy values or even worse - NaN's
    ///</summary>
    public static float NormalizeNodeValue(float value)
    {
        return 2*maxValue / Mathf.PI * Mathf.Atan(value);
    }

    /// <summary>
    /// Reset id's for ALL NeuralNetworks
    /// </summary>
    public void ResetId()
    {
        edgeId = 0;
        nodeId = 0;
    }

    ///<summary>
    ///Insert node in right place in list which corresponds to it's layer 
    ///</summary>
    public void CreateNewNode(float value)
    {
        
    }

    private Node DuplicateNode(Node node)
    {
        if(node.GetType() == typeof(InputNode))
        {
            InputNode inputNode = (InputNode)node;
            return new InputNode(inputNode.GetId(), inputNode.GetValue(), inputNode.GetName());
        }
        else if(node.GetType() == typeof(OutputNode))
        {
            OutputNode outputNode = (OutputNode)node;
            return new OutputNode(outputNode.GetId(), outputNode.GetValue(), outputNode.GetName());
        }
        else
        {
            return new Node(node.GetId());
        }
    }

    /// <summary>
    /// Create a new connection between two nodes
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="weight"></param>
    public void ConnectNodes(Node from, Node to, float weight)
    {
        Edge newEdge = new Edge(edgeId++, from, to, weight);
        from.AddOutgoingEdge(newEdge);
        edges.Add(newEdge);
    }

    /// <summary>
    /// This overload of ConnectNodes should only be used if we want to recreate connection, NOT to create a new connection
    /// </summary>
    /// <param name="edgeId"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="weight"></param>
    /// <param name="enabled"></param>
    public void ConnectNodes(int edgeId,Node from, Node to, float weight, bool enabled)
    {
        Edge newEdge = new Edge(edgeId, from, to, weight);
        from.AddOutgoingEdge(newEdge);
        newEdge.SetEnabled(enabled);
        edges.Add(newEdge);
    }

    public void ConnectNodeToRandom(Node node)
    {
        int nodeIndex = nodes.FindIndex(x => x == node);
        Node targetNode = nodes[UnityEngine.Random.Range(nodeIndex+1, nodes.Count)];
        if (!node.IsConnectedWith(targetNode) && targetNode.GetType()!=typeof(InputNode))
        {
            ConnectNodes(node, targetNode, UnityEngine.Random.Range(-NeuralNetwork.maxValue, NeuralNetwork.maxValue));
        }
    }

    private int FindNextIndex(Node node)
    {
        bool inInputsLayer = true;
        bool foundNode = false;

        for(int i=0;i<nodes.Count;i++)
        {
            if(nodes[i].GetType()!=typeof(InputNode))
            {
                inInputsLayer = false;
            }

            if (!inInputsLayer && foundNode)
            {
                return i;
            }

            if (nodes[i]==node)
            {
                foundNode = true;
            }
        }
        return nodes.Count;
    }

    /// <summary>
    /// Split edge with a new node: <br/>
    /// 1)Create new node<br/>
    /// 2)Connect edge.from to new node with weight 1 <br/>
    /// 3)Connect new node to edge.to with weight edge.weight <br/>
    /// 4)Deactivate edge <br/>
    /// </summary>
    /// <param name="edge">Edge to split</param>
    public void SplitWithNewNode(Edge edge)
    {
        Node newNode = new Node(nodeId++);
        ConnectNodes(edge.GetFromNode(), newNode, 1);
        ConnectNodes(newNode, edge.GetToNode(), edge.GetWeight());
        edge.SetEnabled(false);

        nodes.Insert(FindNextIndex(edge.GetFromNode()), newNode);
    }

    public void MutateRandomEdge()
    {
        edges[UnityEngine.Random.Range(0, edges.Count)].Mutate(this);
    }

    public void MutateRandomNode()
    {
        nodes[UnityEngine.Random.Range(0, nodes.Count-outputs.Count)].Mutate(this);
    }

    public void RemoveEdge(Edge edge)
    {
        edges.Remove(edge);
        edge.GetFromNode().RemoveOutgoingEdge(edge);
    }

    ///<summary>
    ///We fire inputs which later fire other nodes.
    ///</summary>
    public void Predict()
    {
        foreach(Node node in nodes)
        {
            if(node.GetType()!=typeof(InputNode))
            {
                node.SetValue(0);
            }
        }


        foreach(Node node in nodes)
        {
            node.InfluenceConnectedNodes(this);
        }
    }

    public void OnBeforeSerialize()
    {
        serializedNodes = new List<SerializedNode>();
        serializedEdges = new List<SerializedEdge>();

        if (nodes != null && edges != null)
        {
            foreach (Node node in nodes)
            {
                serializedNodes.Add(new SerializedNode(node));
            }

            foreach (Edge edge in edges)
            {
                serializedEdges.Add(new SerializedEdge(edge));
            }
        }
    }

    public void OnAfterDeserialize()
    {
        //There is no nodes in list, so we must create them
        //Later on we create edges and
        //add those edges connections to the nodes
        //Sanity check
        Debug.Log("NeuralNetwork|serializedNodes.count:"+serializedNodes.Count);

        nodes.Clear();
        edges.Clear();
        inputs.Clear(); 
        outputs.Clear();

        foreach(SerializedNode serializedNode in serializedNodes)
        {
            Node newNode;
            if (serializedNode.isInput)
            {
                newNode = new InputNode(serializedNode.id, 1, serializedNode.name);
                inputs.Add(newNode);
            }
            else if(serializedNode.isOutput)
            {
                newNode = new OutputNode(serializedNode.id, 1, serializedNode.name);
                outputs.Add(newNode);
            }
            else
            {
                newNode = new Node(serializedNode.id, 1);
            }

            if(serializedNode.id>nodeId)//If any network has node with id bigger than actual nodeId it will change it for all
            {
                nodeId = serializedNode.id + 1;
            }

            nodes.Add(newNode);
        }

        //Nodes are set up properly excluding their outgoingEdges

        Debug.Log("NeuralNetwork|serializedEdges.count:" + serializedEdges.Count);

        foreach (SerializedEdge serializedEdge in serializedEdges)
        {
            Node from = GetNode(serializedEdge.fromId);
            Node to = GetNode(serializedEdge.toId);

            Edge newEdge = new Edge(serializedEdge.id, from, to, serializedEdge.weight, serializedEdge.enabled);
            from.AddOutgoingEdge(newEdge);

            if(serializedEdge.id>edgeId)
            {
                edgeId = serializedEdge.id + 1;
            }

            edges.Add(newEdge);
        }
    }

    public void Serialize(string path)
    {
        //OnBeforeSerialize();
        //XmlSerializer serializer = new XmlSerializer(typeof(NeuralNetwork));
        //StreamWriter writer = new StreamWriter(path+"NN.xml");
        //serializer.Serialize(writer.BaseStream, this);
        //writer.Close();
    }

    public override string ToString()
    {
        if (nodes != null && inputs != null && outputs != null)
        {
            return "NeuralNetwork|Nodes:" + nodes.Count + ", Inputs:" + inputs.Count + ", Outputs:" + outputs.Count;
        }
        else
        {
            return "NeuralNetwork|Has not been initialized properly yet!";
        }
    }

    [System.Serializable]
    public class SerializedNode
    {
        public int id;
        public bool isInput;
        public bool isOutput;
        public string name;
        //public float value;//It is not really needed

        public SerializedNode(Node node)
        {
            this.id = node.GetId();
            this.isInput = node.GetType() == typeof(InputNode);
            this.isOutput = node.GetType() == typeof(OutputNode);
            
            if(this.isInput)
            {
                InputNode inputNode = node as InputNode;
                this.name = inputNode.name;
            }

            if (this.isOutput)
            {
                OutputNode outputNode = node as OutputNode;
                this.name = outputNode.name;
            }
        }
    }

    [System.Serializable]
    public class SerializedEdge
    {
        public int id;
        public bool enabled;
        public int fromId;
        public int toId;
        public float weight;

        public SerializedEdge(Edge edge)
        {
            this.id = edge.GetId();
            this.enabled = edge.IsEnabled();
            this.fromId = edge.GetFromId();
            this.toId = edge.GetToId();
            this.weight = edge.GetWeight();
        }
    }
}
