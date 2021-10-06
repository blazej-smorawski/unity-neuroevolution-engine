using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class OrganismEditorWindow : EditorWindow
{
    Rect windowRect = new Rect(50 + 50, 50, 50, 50);
    Rect windowRect2 = new Rect(50, 50, 50, 50);

    Organism organism;
    NeuralNetwork neuralNetwork;
    int inputsAmount = 1, outputsAmount = 1;

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
                organism.brain = new NeuralNetwork(inputsAmount, outputsAmount);
            }

            if (organism.brain != null)
            {
                Handles.BeginGUI();
                Handles.DrawBezier(windowRect.center, windowRect2.center, new Vector2(windowRect.xMax + 50f, windowRect.center.y), new Vector2(windowRect2.xMin - 50f, windowRect2.center.y), Color.red, null, 5f);
                Handles.EndGUI();

                BeginWindows();
                windowRect = GUI.Window(0, windowRect, WindowFunction, "Box1");
                windowRect2 = GUI.Window(1, windowRect2, WindowFunction, "Box2");

                EndWindows();
            }
        }
        else
        {
            GUILayout.Label("Please select GameObject containing Organism component");
        }
    }
    void WindowFunction(int windowID)
    {
        GUI.DragWindow();
    }
}
