using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Overseer : MonoBehaviour
{
    [Header("World Properties")]
    public float timeScale = 1f;

    [Header("Generation Info")]
    public string recordName = "";
    private string recordUniqueName = "";
    public float averageFitness = 0;
    public float previousAverageFitness = 0;
    public float bestPreviousAverageFitness = 0;
    public int generationNumber = 0;
    public int speciesCount = 0;

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

    ///<summary>
    ///Spawn first generation of organisms, set organismPrefab brain to new NeuralNetwork if needed
    ///</summary>
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
                /*
                 * Trigger all spawners that spawn an object for every spawned organism.
                 * For example, spawning food for orcs
                 */
                TriggerAtPositionSpawners(new Vector3(i * spawnOffset, 0, j * spawnOffset));
                spawnedOrganism = spawnedOrganismGameObject.GetComponent<Organism>();

                /*
                 * Generate a new NeuralNetwork
                 * TODO: I don't like this, but right now I haven't got
                 * any idea on how to make it better, because the
                 * serialization is not working the way i'd like to
                 */
                if (startNeuralNetworkName != "")
                {
                    spawnedOrganism.DeserializeBrain("Serialized Networks\\", startNeuralNetworkName);
                }
                else
                {
                    /*
                     * We have to be careful with code such as this,
                     * because there is a risk that id's of nodes will
                     * be misaligned i.e input/output nodes across all networks
                     * will have different id's even though they are the same
                     * nodes - serving the same purpose
                     */
                    spawnedOrganism.GenerateNeuralNetwork();
                }

                organisms.Add(spawnedOrganism);
            }
        }

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

    /// <summary>
    /// Reset id's for organism's NeuralNetwork
    /// </summary>
    /// <param name="organism"></param>
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
        if (crossingOver == null)
        {
            crossingOver = new CrossingOver();
        }
        if (recordName != "")
        {
            int index = 0;
            string newRecordUniqueName = recordName+index;
            while (File.Exists(GetFitnessRecordPath(recordName+index)))
            {
                index++;
                newRecordUniqueName = recordName + index;
            }
            recordUniqueName = newRecordUniqueName;
        }
        generationLeftTime = generationTime;
        Time.timeScale = timeScale;
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

    private string GetFitnessRecordPath(string name)
    {
        return "Fitness Records\\" + name + ".txt";
    }

    private void WriteRecords()
    {
        if (recordName != "")
        {
            File.AppendAllLines(GetFitnessRecordPath(recordUniqueName), new[] { averageFitness.ToString() });
        }
    }

    private void SpawnNextGeneration()
    {
        DestroySpawnedObjects(atPositionSpawners);
        DestroySpawnedObjects(spawners);
        CalculateAverageFitness(organisms);
        WriteRecords();

        List<NeuralNetwork> networks = new List<NeuralNetwork>();
        foreach (Organism organism in organisms)
        {
            /*
             * We put all networks in a list
             */
            networks.Add(organism.neuralNetwork);
        }
        /*
         * Now we swap it with a new list which contains
         * newly reproduced networks
         */
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
