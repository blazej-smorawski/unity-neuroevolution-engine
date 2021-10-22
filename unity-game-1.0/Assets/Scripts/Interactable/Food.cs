using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour,Interactable
{
    public float foodAmount = 20f;
    public float addFitness = 1f;
    public void Interact(Organism organism)
    {
        Debug.Log("Interactable|Food eaten by: " + organism.name);
        organism.food += foodAmount;
        organism.fitness += addFitness;
        Destroy(gameObject);
    }
}
