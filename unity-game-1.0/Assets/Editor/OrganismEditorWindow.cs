using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class OrganismEditorWindow : EditorWindow
{
    Vector2 scrollPosition = Vector2.zero;

    Rect windowRect = new Rect(50 + 50, 50, 50, 50);

    Organism organism;
    NeuralNetwork neuralNetwork;

    int inputsAmount = 1, outputsAmount = 1;
    int xMargin = 100;
    int yMargin = 150;
    int xDelta = 150;
    int yDelta = 75;

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

        if (organism)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Inputs Amount", GUILayout.MaxWidth(140));
            inputsAmount = EditorGUILayout.IntField(inputsAmount, GUILayout.MaxWidth(40));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Outputs Amount", GUILayout.MaxWidth(140));
            outputsAmount = EditorGUILayout.IntField(outputsAmount, GUILayout.MaxWidth(40));
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate Neural Network", GUILayout.MaxWidth(180)))
            {
                organism.brain = new NeuralNetwork(inputsAmount, outputsAmount, "r");
            }

            if (typeof(NeuralNetwork).IsSerializable && typeof(Node).IsSerializable && typeof(Edge).IsSerializable)
            {
                GUILayout.Label("NeuralNetwork is serializable", GUILayout.MaxWidth(180));

                if (GUILayout.Button("Serialize", GUILayout.MaxWidth(180)))
                {
                    organism.brain.Serialize("");
                }
            }
            else
            {
                GUILayout.Label("NeuralNetwork is NOT serializable", GUILayout.MaxWidth(180));
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
                    //Handles.DrawBezier(windowRect.center, windowRect2.center, new Vector2(windowRect.xMax + 50f, windowRect.center.y), new Vector2(windowRect2.xMin - 50f, windowRect2.center.y), Color.red, null, 5f);
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
                EndWindows();
                //windowRect = GUI.Window(0, windowRect, WindowFunction, "Box1");
            }
        }
        else
        {
            GUILayout.Label("Please select GameObject containing Organism component");
        }
    }

    private void DrawNodes()
    {
        nodes = new List<VisualNode>();
        edgeColours = new List<Color>();
        edgeBeginnings = new List<Rect>();
        edgeEnds = new List<Rect>();

        int positionX = xMargin;

        foreach (NeuralLayer neuralLayer in organism.brain.layers)
        {
            int positionY = yMargin;
            foreach (Node node in neuralLayer.nodes)
            {
                //Rect newRect = new Rect(positionX, positionY, 50, 50);
                nodes.Add(new VisualNode(node, new Rect(positionX, positionY, 50, 50),
                    new GUIContent("Node:" + node.GetId() + "\n->" + node.GetValue(), "Click to delete[not implemented]")));

                positionY += yDelta;
            }
            positionX += xDelta;
        }

        foreach (VisualNode begginingNode in nodes)
        {
            foreach (VisualNode endNode in nodes)
            {
                if (begginingNode.node.IsConnectedWith(endNode.node))
                {
                    edgeBeginnings.Add(begginingNode.nodeRect);
                    edgeEnds.Add(endNode.nodeRect);
                    edgeColours.Add(Color.blue);//Add weight?
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
