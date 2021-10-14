using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overseer : MonoBehaviour
{
    [Header("Generation Info")]
    public float averageFitness = 0;
    public int generationNumber = 0;
    [Header("Spawning Properties")]
    public GameObject organismPrefab;
    public List<Organism> organisms;
    public List<Organism> newOrganisms;
    public int generationOrganismsCount;
    public float spawnOffset = 10f;
    public float generationTime = 10f;
    public float generationLeftTime;
    [Header("Mutation Properties")]
    [Range(0.0f, 100.0f)]
    public float mutationChance = 5f;
    [Range(0.0f, 100.0f)]
    public float edgeMutationCoefficient = 50f;//50 means 50% chance for edge mutation and 50% for node mutation

    public void spawnInitialGeneration()
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
                spawnedOrganism.brain = new NeuralNetwork(spawnedOrganism.brain, spawnedOrganism.brain);
                organisms.Add(spawnedOrganism);
            }
        }
        mutateOrganisms(organisms);
    }

    public void mutateOrganisms(List<Organism> organisms)
    {
        foreach (Organism organism in organisms)
        {
            organism.brain.Mutate(mutationChance, edgeMutationCoefficient);
        }
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
            mutateOrganisms(organisms);
        }
    }

    IEnumerator PauseCoroutine(float time)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }

    public void Start()
    {
        generationLeftTime = generationTime;
        spawnInitialGeneration();
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
                    //GameObject kid = organisms[i].Reproduce(organisms[j], new Vector3(i * spawnOffset, 0, j * spawnOffset));
                    GameObject kid = Instantiate(organismPrefab, new Vector3(i * spawnOffset, 0, j * spawnOffset), transform.rotation);
                    kidOrganism = kid.GetComponent<Organism>();

                    if (organisms[i].fitness > organisms[j].fitness)
                    {
                        kidOrganism.brain = new NeuralNetwork(organisms[i].brain, organisms[j].brain);
                    }
                    else
                    {
                        kidOrganism.brain = new NeuralNetwork(organisms[j].brain, organisms[i].brain);
                    }

                    kidOrganism.RestartEvaluators();
                    kidOrganism.Reset();
                    newOrganisms.Add(kidOrganism);
                }
            }

            averageFitness = 0;
            foreach (Organism organism in organisms)
            {
                averageFitness += organism.fitness;
            }

            if (organisms.Count != 0)
            {
                averageFitness /= organisms.Count;
            }

            foreach (Organism organism in organisms)
            {
                Destroy(organism.gameObject);
            }

            generationLeftTime = generationTime;
            organisms = newOrganisms;
            newOrganisms = new List<Organism>();
            mutateOrganisms(organisms);
            StartCoroutine(PauseCoroutine(1f));
        }
        else 
        {
            generationLeftTime -= Time.deltaTime;
        }
    }
}
