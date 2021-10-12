using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputNode : Node
{
    public string name;

    public OutputNode(int id, float value, string name) : base(id, value)
    {
        this.name = name;
    }
    public string GetName()
    {
        return name;
    }
}
