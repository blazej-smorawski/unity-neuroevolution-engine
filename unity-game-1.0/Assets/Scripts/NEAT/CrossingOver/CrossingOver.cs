using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CrossingOver
{
    [Header("Crossing Over Properties")]
    [Range(0.1f, 10.0f)]
    public int eliteCount = 7;
    public float maximumDistance = 5f;// Surely needs some tweaking
    public float excessCoefficient = 1f;
    public float disjointCoefficient = 1f;
    public float weightsDifferenceCoefficient = 0.3f;
    public float networksReproductionFraction = 0.5f;// represents fraction of NeuralNetoworks that will be reproduced

    public CrossingOver()
    {

    }

    private float CalculateDistanceBetweenNetworks(NeuralNetwork n1, NeuralNetwork n2)
    {
        int excess = 0;
        int disjoint = 0;
        float averageWeightsDistance = 0f;
        List<float> distances = new List<float>();
        List<Edge> n1Edges = n1.GetEdges();
        List<Edge> n2Edges = n2.GetEdges();

        int N = Mathf.Max(n1Edges.Count, n2Edges.Count);
        int n1Index = 0;
        int n2Index = 0;

        if(N==0)
        {
            return 0;
        }

        while (n1Index < n1Edges.Count)
        {
            if (n2Index < n2Edges.Count && n1Edges[n1Index].GetId() > n2Edges[n2Index].GetId())// There might be a node in weakerEdges corresponding to strongerEdges[i]
            {
                ++n2Index;
                ++disjoint;
            }
            else if (n2Index < n2Edges.Count && n1Edges[n1Index].GetId() == n2Edges[n2Index].GetId())// We've got a match
            {
                distances.Add(Mathf.Abs(n1Edges[n1Index].GetWeight() - n2Edges[n2Index].GetWeight()));
                ++n2Index;
                ++n1Index;
            }
            else// n1Edges[n1Index] != n2Edges[n2Index] 
            {
                ++n1Index;
                ++disjoint;

                if (n2Index >= n2Edges.Count)// We reached end of n2
                {
                    ++excess;
                }
            }
        }

        while (n2Index < n2Edges.Count)// If we haven't reached end of n2 it means we have some excess edges
        {
            ++excess;
            ++n2Index;
        }

        if (distances.Count > 0)
        {
            foreach (float distance in distances)
            {
                averageWeightsDistance += distance;
            }
            averageWeightsDistance /= (float)(distances.Count);
        }
        else
        {
            averageWeightsDistance = 0;
        }

        return
            (excessCoefficient * excess / N) +
            (disjointCoefficient * disjoint / N) +
            (weightsDifferenceCoefficient * averageWeightsDistance);
    }

    /// <summary>
    /// Calculate fitness taking species into account. Species property of NeuralNetwork MUST be set before!
    /// </summary>
    private int ShareSpecies(NeuralNetwork n1, NeuralNetwork n2)
    {
        // Version that do not require having species set
        // return CalculateDistanceBetweenNetworks(n1, n2)<maximumDistance ? 1 : 0;
        return n1.speciesIndex == n2.speciesIndex ? 1 : 0;
    }

    public List<NeuralNetwork> CrossOverNeuralNetworks(in List<NeuralNetwork> neuralNetworks)// in works like const in C++
    {
        List<List<NeuralNetwork>> speciesList = new List<List<NeuralNetwork>>();
        List<int> speciesCount = new List<int>();
        List<float> adjustedFitnessesSums = new List<float>(); // Sums for each species
        float adjustedFitnessesSum = 0; // Sum for all networks

        neuralNetworks.Sort((x, y) => y.fitness.CompareTo(x.fitness));
        for (int i = 0; i< eliteCount; i++)
        {
            neuralNetworks[i].elite = true;
        }
        for(int i= eliteCount; i<neuralNetworks.Count; i++)
        {
            neuralNetworks[i].elite = false;
        }

        // We group neuralNetworks in to species
        foreach (NeuralNetwork neuralNetwork in neuralNetworks)
        {
            int speciesIndex;
            NeuralNetwork sibling = null;
            for(speciesIndex = 0; speciesIndex < speciesList.Count; speciesIndex++)
            {
                // NeuralNetwork close to the neuralNetwork
                sibling = speciesList[speciesIndex].Find(x => CalculateDistanceBetweenNetworks(x, neuralNetwork) < maximumDistance);
                if(sibling != null)
                {
                    break;
                }
            }

            if (sibling == null) // There is no network sibling network which means that we must create a new species
            {
                speciesList.Add(new List<NeuralNetwork>());
                speciesCount.Add(0);
                adjustedFitnessesSums.Add(0);
                speciesIndex = speciesList.Count-1; // New species is last in the list
            }
            else
            {
                speciesIndex = sibling.speciesIndex;
            }

            neuralNetwork.speciesIndex = speciesIndex;
            ++speciesCount[speciesIndex];
            speciesList[speciesIndex].Add(neuralNetwork);
        }

        // Networks are split into species, now we calculate adjustedFitness
        foreach (NeuralNetwork neuralNetwork in neuralNetworks)
        {
            neuralNetwork.adjustedFitness = neuralNetwork.fitness / (float)(speciesCount[neuralNetwork.speciesIndex]);
            adjustedFitnessesSums[neuralNetwork.speciesIndex] += neuralNetwork.adjustedFitness;
            adjustedFitnessesSum += neuralNetwork.adjustedFitness;
        }

        // Remove weakest organisms in each species
        int networksToReproduceCount = (int)(neuralNetworks.Count * networksReproductionFraction);

        int toRemoveLeft = networksToReproduceCount;
        int networksLeft = neuralNetworks.Count;
        int removed = 0;
        for (int i = 0; i < speciesList.Count; i++)
        {
            speciesList[i].Sort((x, y) => y.adjustedFitness.CompareTo(x.adjustedFitness));

            int removeCount = (int)Mathf.Round(toRemoveLeft * speciesList[i].Count / networksLeft);
            toRemoveLeft -= removeCount;
            networksLeft -= speciesList[i].Count;

            for (int j = 0; j < removeCount; j++)
            {
                NeuralNetwork toRemove = speciesList[i][speciesList[i].Count - 1]; // Remove last
                if (!toRemove.elite)
                {
                    neuralNetworks.Remove(toRemove);
                    speciesList[i].Remove(toRemove);
                    removed++;
                }
            }
        }
        speciesList.RemoveAll(species => species.Count == 0);
        Debug.Log("Removed: " + removed);

        // Recalculate adjusted fitness
        adjustedFitnessesSum = 0;
        for (int i=0;i<adjustedFitnessesSums.Count; i++)
        {
            adjustedFitnessesSums[i] = 0;
        }
        foreach (NeuralNetwork neuralNetwork in neuralNetworks)
        {
            neuralNetwork.adjustedFitness = neuralNetwork.fitness / (float)(speciesCount[neuralNetwork.speciesIndex]);
            adjustedFitnessesSums[neuralNetwork.speciesIndex] += neuralNetwork.adjustedFitness;
            adjustedFitnessesSum += neuralNetwork.adjustedFitness;
        }

        // Calculate reproduction tokens and reproduce
        int toAddLeft = removed;
        networksLeft = neuralNetworks.Count;
        for (int i = 0; i < speciesList.Count; i++)
        {
            speciesList[i].Sort((x, y) => y.adjustedFitness.CompareTo(x.adjustedFitness));

            int addCount;
            if(adjustedFitnessesSum != 0)
            {
                addCount = (int)Mathf.Round(toAddLeft * adjustedFitnessesSums[i] / adjustedFitnessesSum);
                adjustedFitnessesSum -= adjustedFitnessesSums[i];
                toAddLeft -= addCount;
                networksLeft -= speciesList[i].Count;
            }
            else
            {
                addCount = (int)Mathf.Round(toAddLeft * speciesList[i].Count / networksLeft);
                toAddLeft -= addCount;
                networksLeft -= speciesList[i].Count;
            }

            for (int j = 0; j < addCount; j++)
            {
                NeuralNetwork p1 = speciesList[i][Random.Range(0, speciesList[i].Count)];
                NeuralNetwork p2 = speciesList[i][Random.Range(0, speciesList[i].Count)];
                neuralNetworks.Add(new NeuralNetwork(p1, p2));
            }
        }

        foreach ( NeuralNetwork neuralNetwork in neuralNetworks )
        {
            neuralNetwork.fitness = 0;
            neuralNetwork.adjustedFitness = 0;
        }
        //Debug.Break();
        Debug.Log("CrossingOver|Networks count: " + neuralNetworks.Count + " Species count: " + speciesList.Count);
        return neuralNetworks;
    }
}
