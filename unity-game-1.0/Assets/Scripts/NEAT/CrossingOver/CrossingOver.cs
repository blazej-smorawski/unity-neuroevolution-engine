using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CrossingOver
{
    [Header("Crossing Over Properties")]
    [Range(0.1f, 100.0f)]
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

        foreach (float distance in distances)
        {
            averageWeightsDistance += distance;
        }
        averageWeightsDistance /= (float)(distances.Count);

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
        List<List<NeuralNetwork>> species = new List<List<NeuralNetwork>>();
        species.Add(new List<NeuralNetwork>());
        List<int> speciesCount = new List<int> { 0 };
        List<float> adjustedFitnessesSums = new List<float>() { 0 };
        int speciesIndex = 0;

        //We group neuralNetworks in to species
        foreach (NeuralNetwork neuralNetwork in neuralNetworks)
        {
            if (species[speciesIndex].Find(x => CalculateDistanceBetweenNetworks(x, neuralNetwork) > maximumDistance) != null) // There is network in species which is not compatible with neuralNetwork
            {
                ++speciesIndex;// We create a new species
                species.Add(new List<NeuralNetwork>());
                speciesCount.Add(0);
                adjustedFitnessesSums.Add(0);
            }

            neuralNetwork.speciesIndex = speciesIndex;
            ++speciesCount[speciesIndex];
            species[speciesIndex].Add(neuralNetwork);
        }

        // Networks are split into species, now we calculate adjustedFitness
        foreach (NeuralNetwork neuralNetwork in neuralNetworks)
        {
            neuralNetwork.adjustedFitness = neuralNetwork.fitness / (float)(speciesCount[neuralNetwork.speciesIndex]);
            adjustedFitnessesSums[neuralNetwork.speciesIndex] += neuralNetwork.adjustedFitness;
        }

        neuralNetworks.Sort((x, y) => y.adjustedFitness.CompareTo(x.adjustedFitness));

        List<NeuralNetwork> newNeuralNetworks = new List<NeuralNetwork>();
        int networksToReproduceCount = (int)(neuralNetworks.Count * networksReproductionFraction);
        int reproductionTokens = neuralNetworks.Count;
        int networkIndex = 0;

        for(int i = 0; i < reproductionTokens; i++)
        {
            NeuralNetwork neuralNetwork = neuralNetworks[networkIndex++ % networksToReproduceCount];//We breed this network with a random network from it's species
            newNeuralNetworks.Add(new NeuralNetwork(neuralNetwork, species[neuralNetwork.speciesIndex][UnityEngine.Random.Range(0, species[neuralNetwork.speciesIndex].Count)]));
        }

        Debug.Log("CrossingOver|Species count: " + species.Count);
        return newNeuralNetworks;
    }
}
