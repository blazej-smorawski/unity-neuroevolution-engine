using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organism : MonoBehaviour
{
    [Header("Brain")]
    public NeuralNetwork brain = null;//BRAIN MUST BE SETUP BEFORE RUNNING THE GAME!!!
    public List<string> inputs;
    public List<string> outputs;
    public string brainOptions;
    [Header("Reproduction")]
    public float reproductionCooldown = 100f;
    public float timeLeftToReproduction = 10f;
    [Header("Statistics")]
    public float fitness;

    public void OnCollisionEnter(Collision collision)
    {
        Organism enteringOrganism = collision.gameObject.GetComponent<Organism>();
        if (enteringOrganism != null && CanReproduce() && enteringOrganism.CanReproduce())
        {
            StartReproductionCooldown();
            enteringOrganism.StartReproductionCooldown();

            GameObject kid = Instantiate(gameObject, transform.position, transform.rotation);
            Organism kidOrganism = kid.GetComponent<Organism>();
            kidOrganism.brain = new NeuralNetwork(enteringOrganism.brain, brain);//Right now more "active" organism is the stronger one

            kidOrganism.StartReproductionCooldown();
        }

        if(CanReproduce())
        {
            Debug.Log("Can reproduce");
        }
    }

    private void Update()
    {
        if(timeLeftToReproduction!=0)
        {
            timeLeftToReproduction -= Time.deltaTime;

            if(timeLeftToReproduction < 0f)
            {
                timeLeftToReproduction = 0f;
            }
        }
    }
    
    public bool CanReproduce()
    {
        return timeLeftToReproduction == 0f;
    }

    public void StartReproductionCooldown()
    {
        timeLeftToReproduction = reproductionCooldown;
    }
}
