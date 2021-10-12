using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// More or less alright
/// </summary>
public class OrganismEditorWindow : EditorWindow
{   
    Organism organism;

    public bool usePoints;

    private float _scale =1f;

    public int _xMargin = 50;
    public int _yMargin = 50;

    public float xMargin
    {
        get {  return _xMargin; }
        set {  _xMargin = (int)value; DrawNodes(); }
    }

    public float yMargin
    {
        get { return _yMargin; }
        set { _yMargin = (int)value; DrawNodes(); }
    }

    public float scale
    {
        get { return _scale; }
        set {  _scale = value; DrawNodes(); }
    }

    List<VisualNode> nodes;

    List<Rect> edgeBeginnings;
    List<Rect> edgeEnds;
    List<Color> edgeColours;

    [MenuItem("Window/Organism Editor Window")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(OrganismEditorWindow));
    }

    private void OnGUI()
    {
        if (Selection.activeGameObject)
        {
            organism = Selection.activeGameObject.GetComponent<Organism>();
        }

        if (organism!=null)
        {
            if (organism.brain != null)
            {
                GUILayout.Label(organism.brain.ToString(), GUILayout.MaxWidth(400));
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Neural Network", GUILayout.MaxWidth(180)))
            {
                organism.brain = new NeuralNetwork(organism.inputs, organism.outputs, organism.brainOptions);
                DrawNodes();
            }

            if (GUILayout.Button("Serialize", GUILayout.MaxWidth(180)))
            {
                organism.brain.Serialize("");
            }

            if (GUILayout.Button("Reset ID's", GUILayout.MaxWidth(180)))
            {
                organism.brain.ResetId();
            }

            if (GUILayout.Button("Mutate Random Node", GUILayout.MaxWidth(180)))
            {
                organism.brain.MutateRandomNode();
                DrawNodes();
            }

            if (GUILayout.Button("Mutate Random Edge", GUILayout.MaxWidth(180)))
            {
                organism.brain.MutateRandomEdge();
                DrawNodes();
            }

            if (organism.brain != null)
            {
                if (GUILayout.Button("Predict", GUILayout.MaxWidth(180)))
                {
                    organism.brain.Predict();
                    DrawNodes();
                }

                if (GUILayout.Button("Draw Nodes", GUILayout.MaxWidth(180)))
                {
                    DrawNodes();
                }

                if (nodes != null)
                {
                    Handles.BeginGUI();
                    for(int i=0;i<edgeBeginnings.Count;i++)
                    {
                        Handles.DrawBezier(edgeBeginnings[i].center, edgeEnds[i].center, 
                            new Vector2(edgeBeginnings[i].xMax + 50f, edgeBeginnings[i].center.y), 
                            new Vector2(edgeEnds[i].xMin - 50f, edgeEnds[i].center.y), 
                            edgeColours[i], null, 5f);
                    }
                    Handles.EndGUI();

                    BeginWindows();

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        GUI.Box(nodes[i].nodeRect, nodes[i].content);
                    }
                }

                scale = EditorGUILayout.Slider(scale, .3f, 3);

                yMargin = GUI.VerticalSlider(new Rect(nodes[nodes.Count-1].nodeRect.x+100,50,10,300), yMargin, 50.0f, 1000.0f);

                EditorGUILayout.EndHorizontal();
                EndWindows();
            }
        }
        else
        {
            GUILayout.Label("Please select GameObject containing Organism component");
        }
    }

    private void DrawNodes()
    {
        int xDelta = (int)(scale * 100);
        int yDelta = (int)(scale * 50);
        int xSize = (int)(scale * 50);
        int ySize = (int)(scale * 50);
        int layer = 0;

        nodes = new List<VisualNode>();
        edgeColours = new List<Color>();
        edgeBeginnings = new List<Rect>();
        edgeEnds = new List<Rect>();

        int positionX = (int)xMargin;

        foreach (List<Node> neuralLayer in organism.brain.GetLayers())
        {
            int positionY = (int)yMargin + layer * 5;
            foreach (Node node in neuralLayer)
            {
                nodes.Add(new VisualNode(node, new Rect(positionX, positionY, xSize, ySize),
                    new GUIContent("N:" + node.GetId() + "\n->" + node.GetValue().ToString("0.00"), "Click to delete[not implemented]")));

                positionY += yDelta;
            }
            positionX += xDelta;
            layer++;
        }

        foreach (VisualNode begginingNode in nodes)
        {
            foreach (VisualNode endNode in nodes)
            {
                Edge edge = begginingNode.node.GetEdgeWith(endNode.node);
                if (edge != null)
                {
                    edgeBeginnings.Add(begginingNode.nodeRect);
                    edgeEnds.Add(endNode.nodeRect);
                    if (edge.IsEnabled())
                    {
                        edgeColours.Add(new Color(0, 0, edge.GetWeight()/NeuralNetwork.maxValue));//Add weight?
                    }
                    else
                    {
                        edgeColours.Add(new Color(edge.GetWeight() / NeuralNetwork.maxValue, 0, 0));
                    }
                }
            }
        }
        
    }

    public struct VisualNode
    {
        public Node node;
        public Rect nodeRect;
        public GUIContent content;

        public VisualNode(Node node,Rect nodeRect, GUIContent content)
        {
            this.node = node;
            this.nodeRect = nodeRect;
            this.content = content;
        }
    }
}
