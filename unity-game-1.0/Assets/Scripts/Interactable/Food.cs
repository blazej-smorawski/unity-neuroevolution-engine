//#define FOOD_DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour,Interactable
{
    public float foodAmount = 20f;
    public float addFitness = 1f;
    public float lifetimeCoefficient = 0.025f;//after 40 secs we got 0
    private float lifetime;
    public void Interact(Organism organism)
    {
#if FOOD_DEBUG
        Debug.Log("Interactable|Food eaten by: " + organism.name);
#endif
        organism.food += foodAmount;
        organism.neuralNetwork.fitness += addFitness - (lifetime * lifetimeCoefficient);
        Destroy(gameObject);
    }

    public void Update()
    {
        lifetime += Time.deltaTime;
    }
}
