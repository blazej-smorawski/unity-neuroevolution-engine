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
    public int generationNumber = 0;

    [Header("Spawning Properties")]
    public string startNeuralNetworkName = "";
    public GameObject organismPrefab;
    public List<Organism> organisms;
    public List<Organism> newOrganisms;
    public int generationOrganismsCount;
    public float spawnOffset = 10f;
    public float generationTime = 10f;
    public float generationLeftTime;

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
                spawnedOrganism = spawnedOrganismGameObject.GetComponent<Organism>();

                if (startNeuralNetworkName != "")
                {
                    spawnedOrganism.DeserializeBrain("Serialized Networks\\", startNeuralNetworkName);
                }

                organisms.Add(spawnedOrganism);
            }
        }

        ResetOrganismNeuralNetwork(organisms[0]);
        mutator.MutatePopulation(organisms,previousAverageFitness,averageFitness);
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
            averageFitness += organism.fitness;
        }

        if (organisms.Count != 0)
        {
            averageFitness /= organisms.Count;
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
            generationNumber++;

            organisms.Sort((x, y) => y.fitness.CompareTo(x.fitness));

            Organism kidOrganism;

            for(int i=0;i<0.1f*generationOrganismsCount;i++)
            {
                for (int j = 0; j < 0.1f * generationOrganismsCount; j++)
                {
                    GameObject kid = organisms[i].Reproduce(organisms[j], new Vector3(i * spawnOffset, 0, j * spawnOffset));
                    kidOrganism = kid.GetComponent<Organism>();
                    kidOrganism.RestartEvaluators();
                    newOrganisms.Add(kidOrganism);
                }
            }

            foreach (Organism organism in organisms)
            {
                Destroy(organism.gameObject);
            }

            organisms = newOrganisms;
            newOrganisms = new List<Organism>();
            mutator.MutatePopulation(organisms,previousAverageFitness,averageFitness);
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
        generationLeftTime = generationTime;
        SpawnInitialGeneration();
    }

    public void Update()
    {
        if(generationLeftTime<=0)
        {
            generationNumber++;

            organisms.Sort((x, y) => y.fitness.CompareTo(x.fitness));

            Organism kidOrganism;

            for (int i = 0; i < 0.1f * generationOrganismsCount; i++)
            {
                for (int j = 0; j < 0.1f * generationOrganismsCount; j++)
                {
                    GameObject kid = Instantiate(organismPrefab, new Vector3(i * spawnOffset, 0, j * spawnOffset), transform.rotation);
                    kidOrganism = kid.GetComponent<Organism>();

                    if (organisms[i].fitness > organisms[j].fitness)
                    {
                        kidOrganism.neuralNetwork = new NeuralNetwork(organisms[i].neuralNetwork, organisms[j].neuralNetwork);
                    }
                    else
                    {
                        kidOrganism.neuralNetwork = new NeuralNetwork(organisms[j].neuralNetwork, organisms[i].neuralNetwork);
                    }

                    kidOrganism.RestartEvaluators();
                    kidOrganism.Reset();
                    newOrganisms.Add(kidOrganism);
                }
            }

            CalculateAverageFitness(organisms);

            foreach (Organism organism in organisms)
            {
                Destroy(organism.gameObject);
            }

            generationLeftTime = generationTime;
            organisms = newOrganisms;
            newOrganisms = new List<Organism>();
            mutator.MutatePopulation(organisms, previousAverageFitness, averageFitness);
            StartCoroutine(PauseCoroutine(1f));
        }
        else 
        {
            generationLeftTime -= Time.deltaTime;
        }
    }
}
