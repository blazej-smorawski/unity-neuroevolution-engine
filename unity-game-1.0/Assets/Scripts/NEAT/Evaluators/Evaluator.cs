using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Evaluator : MonoBehaviour
{
    public abstract void UpdateEvalutator(Organism organism);

    public virtual void Restart()
    {

    }
}
