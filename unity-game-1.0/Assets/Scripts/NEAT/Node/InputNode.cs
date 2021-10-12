using System.Collections;
using System.Collections.Generic;

public class InputNode : Node
{
    public string name;

    public InputNode(int id, float value, string name) : base(id, value)
    {
        this.name = name;
    }

    public string GetName()
    {
        return name;
    }
}
