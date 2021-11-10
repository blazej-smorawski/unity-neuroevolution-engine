using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overseer : MonoBehaviour
{
    [Header("World Properties")]
    public float timeScale = 1f;

    [Header("Generation Info")]
    public float averageFitness = 0;
    public float previousAverageFitness = 0;
    public float bestPreviousAverageFitness = 0;
    public int generationNumber = 0;

    [Header("Spawning Properties")]
    public string startNeuralNetworkName = "";
    public GameObject organismPrefab;
    public int generationOrganismsCount;
    public int rowOrganismsCount = 10;
    public float spawnOffset = 10f;
    public float generationTime = 10f;
    public float generationLeftTime;
    public List<Organism> organisms;
    public List<Organism> newOrganisms;
    public List<Spawner> spawners;
    public List<Spawner> atPositionSpawners;

    [Header("Crossover Properties")]
    public CrossingOver crossingOver;

    [Header("Mutation Properties")]
    public Mutator mutator;

    public void SpawnInitialGeneration()
    {
        GameObject spawnedOrganismGameObject;
        Organism spawnedOrganism;

        organisms = new List<Organism>();
        newOrganisms = new List<Organism>();

        for (int i = 0; i < Mathf.Sqrt(generationOrganismsCount); i++) 
        {
            for (int j = 0; j < Mathf.Sqrt(generationOrganismsCount); j++)
            {
                spawnedOrganismGameObject = Instantiate(organismPrefab, new Vector3(i * spawnOffset, 0, j * spawnOffset), transform.rotation);
                TriggerAtPositionSpawners(new Vector3(i * spawnOffset, 0, j * spawnOffset));
                spawnedOrganism = spawnedOrganismGameObject.GetComponent<Organism>();

                if (startNeuralNetworkName != "")
                {
                    spawnedOrganism.DeserializeBrain("Serialized Networks\\", startNeuralNetworkName);
                }

                organisms.Add(spawnedOrganism);
            }
        }

        ResetOrganismNeuralNetwork(organisms[0]);
        mutator.MutatePopulation(organisms, previousAverageFitness, averageFitness, bestPreviousAverageFitness);
        TriggerSpawners();
    }

    /// <summary>
    /// Calculates averageFitness and sets previousAverageFitness
    /// </summary>
    /// <param name="organisms"></param>
    private void CalculateAverageFitness(List<Organism> organisms)
    {
        previousAverageFitness = averageFitness;
        averageFitness = 0;
        foreach (Organism organism in organisms)
        {
            averageFitness += organism.neuralNetwork.fitness;
        }

        if (organisms.Count != 0)
        {
            averageFitness /= organisms.Count;
        }

        if(previousAverageFitness > bestPreviousAverageFitness)
        {
            bestPreviousAverageFitness = previousAverageFitness;
        }
    }

    private void ResetOrganismNeuralNetwork(Organism organism)
    {
        organism.neuralNetwork.ResetId();
    }

    private IEnumerator EvolutionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(generationTime);
            SpawnNextGeneration();
        }
    }

    IEnumerator PauseCoroutine(float time)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = timeScale;
    }

    public void Start()
    {
        crossingOver = new CrossingOver();
        Time.timeScale = timeScale;
        generationLeftTime = generationTime;
        SpawnInitialGeneration();
    }

    private void TriggerSpawners()
    {
        foreach(Spawner spawner in spawners)
        {
            spawner.Spawn();
        }
    }

    private void DestroySpawnedObjects(List<Spawner> spawners)
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.DestroySpawnedObjects();
        }
    }

    private void TriggerAtPositionSpawners(Vector3 position)
    {
        foreach(Spawner spawner in atPositionSpawners)
        {
            spawner.SpawnAt(position);
        }
    }

    private void SpawnNextGeneration()
    {
        DestroySpawnedObjects(atPositionSpawners);
        DestroySpawnedObjects(spawners);

        List<NeuralNetwork> networks = new List<NeuralNetwork>();
        foreach (Organism organism in organisms)
        {
            networks.Add(organism.neuralNetwork);
        }
        networks = crossingOver.CrossOverNeuralNetworks(networks);

        generationNumber++;

        int organismNumber = 0;
        Organism kidOrganism;

        foreach (NeuralNetwork network in networks)
        {
            GameObject kid = Instantiate(organismPrefab, new Vector3(organismNumber % rowOrganismsCount * spawnOffset, 0, (int)(organismNumber / rowOrganismsCount) * spawnOffset), transform.rotation);
            TriggerAtPositionSpawners(new Vector3(organismNumber % rowOrganismsCount * spawnOffset, 0, (int)(organismNumber / rowOrganismsCount) * spawnOffset));
            kidOrganism = kid.GetComponent<Organism>();
            kidOrganism.neuralNetwork = network;
            kidOrganism.RestartEvaluators();
            kidOrganism.Reset();
            newOrganisms.Add(kidOrganism);
            ++organismNumber;
        }

        CalculateAverageFitness(organisms);

        foreach (Organism organism in organisms)
        {
            Destroy(organism.gameObject);
        }

        generationLeftTime = generationTime;
        organisms = newOrganisms;
        newOrganisms = new List<Organism>();
        mutator.MutatePopulation(organisms, previousAverageFitness, averageFitness, bestPreviousAverageFitness);

        TriggerSpawners();
        StartCoroutine(PauseCoroutine(1f));
    }

    public void Update()
    {
        if(generationLeftTime<=0)
        {
            SpawnNextGeneration();
        }
        else 
        {
            generationLeftTime -= Time.deltaTime;
        }
    }
}