using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mutator
{
    [Header("Mutation Properties")]
    [Range(0.0f, 100.0f)]
    public float mutationChance = 5f;
    [Range(0.0f, 100.0f)]
    public float edgeMutationCoefficient = 50f;//50 means 50% chance for edge mutation and 50% for node mutation
    [Range(1.0f, 1.5f)]
    public float fitnessChangeMultiplier = 1.05f;//Changes of fitness between populations that differ less that fitnessChangeMargin will not affect mutationChance
    [Range(0.0f, 1.0f)]
    public float minimumFitnessChange = 0.1f;

    public Mutator()
    {

    }

    public void MutatePopulation(List<Organism> organisms,float previousAverageFitness,float averageFitness, float bestPreviousAverageFitness)
    {
        if (Mathf.Abs(averageFitness - previousAverageFitness) > minimumFitnessChange)
        {
            if (averageFitness > fitnessChangeMultiplier * bestPreviousAverageFitness)
            {
                mutationChance = Mathf.Sqrt(mutationChance);//Quite aggressive decrease in mutation step
            }
            else if(averageFitness > fitnessChangeMultiplier * previousAverageFitness)
            {
                mutationChance = Mathf.Pow(mutationChance, Mathf.Pow(2f, 3f / 4f)/2f);
            }
            else if (previousAverageFitness > fitnessChangeMultiplier * averageFitness)//It was better before
            {
                mutationChance = Mathf.Pow(mutationChance, Mathf.Pow(2f, 1f / 4f));//If population fails 4 times and succedes once, we come back to mutationChance from before those attempts
            }
            //if it did not increase nor decrease we do not alter mutationChance
        }


        foreach(Organism organism in organisms)
        {
            MutateOrganism(organism);
        }
    }

    public void MutateOrganism(Organism organism)
    {
        organism.neuralNetwork.Mutate(mutationChance, edgeMutationCoefficient);
    }
}
