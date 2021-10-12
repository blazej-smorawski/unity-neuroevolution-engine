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
    [Header("Evaluators")]
    public List<FitnessEvaluator> fitnessEvaluators;
    public List<InputEvaluator> inputEvaluators;
    [Header("Actions")]
    public float movementSpeed;
    public float rotationSpeed;
    public Vector3 direction;
    public Vector3 rotationOffset;

    public void OnCollisionEnter(Collision collision)
    {
        Organism enteringOrganism = collision.gameObject.GetComponent<Organism>();
        if (enteringOrganism != null && CanReproduce() && enteringOrganism.CanReproduce())
        {
            Reproduce(enteringOrganism);
        }
    }

    private void Update()
    {
        foreach(FitnessEvaluator evaluator in fitnessEvaluators)
        {
            evaluator.UpdateEvalutator(this);
        }

        foreach(InputEvaluator evaluator in inputEvaluators)
        {
            evaluator.UpdateEvalutator(this);
        }

        ReadOutput();

        if(timeLeftToReproduction!=0)
        {
            timeLeftToReproduction -= Time.deltaTime;

            if(timeLeftToReproduction < 0f)
            {
                timeLeftToReproduction = 0f;
            }
        }
    }
    
    public void ReadOutput()
    {
        brain.Predict();
        MoveOrganism(brain.GetOutput("Horizontal"), brain.GetOutput("Vertical"));
    }

    public void MoveOrganism(float horizontal,float vertical)
    {
        direction = new Vector3(horizontal, 0f, vertical);
        //targetWalkBlendSpeed = direction.magnitude;//Target speed=<0,1> because it is only affecting blend tree and blends between idle(0) and run(1) with walk in between those two
        transform.position += direction.normalized * movementSpeed * Time.deltaTime;
        rotationOffset = new Vector3(0,1,0);//Move in world space???

        rotationOffset.y = 0f;//We do not want rotation in planes other than XZ
        transform.forward += Vector3.Lerp(transform.forward, rotationOffset, Time.deltaTime * rotationSpeed);//Character is rotated towards specific axis
    }

    public bool CanReproduce()
    {
        return timeLeftToReproduction == 0f;
    }

    public void StartReproductionCooldown()
    {
        timeLeftToReproduction = reproductionCooldown;
    }

    public void Reproduce(Organism enteringOrganism)
    {
        StartReproductionCooldown();
        enteringOrganism.StartReproductionCooldown();

        GameObject kid = Instantiate(gameObject, transform.position, transform.rotation);
        Organism kidOrganism = kid.GetComponent<Organism>();
        kidOrganism.brain = new NeuralNetwork(enteringOrganism.brain, brain);//Right now more "active" organism is the stronger one

        kidOrganism.StartReproductionCooldown();
    }
}
