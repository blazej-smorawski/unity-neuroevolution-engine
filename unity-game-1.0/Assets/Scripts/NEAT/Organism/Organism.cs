using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organism : MonoBehaviour
{
    [Header("Brain")]
    public bool useOutput = true;
    public NeuralNetwork brain = null;//BRAIN MUST BE SETUP BEFORE RUNNING THE GAME!!!
    public List<string> inputs;
    public List<string> outputs;
    public string brainOptions;
    [Header("Reproduction")]
    public float reproductionCooldown = 1f;
    public float timeLeftToReproduction = 1f;
    [Header("Statistics")]
    public float fitness;
    [Header("Evaluators")]
    public List<FitnessEvaluator> fitnessEvaluators;
    public List<InputEvaluator> inputEvaluators;
    public List<Resetter> resetters;
    [Header("Actions")]
    public float speedMultiplier = 2f;
    public float maxMovementSpeed = 10f;
    public float rotationSpeed = 1f;
    public float acceleration = .2f;
    public float deadzone = 0.05f;
    public float horizontal;
    public float vertical;
    public Vector3 direction;
    public Rigidbody rigidbody;
    public Transform movingTransform;

    public void OnCollisionEnter(Collision collision)
    {
        Organism enteringOrganism = collision.gameObject.GetComponent<Organism>();
        if (enteringOrganism != null && CanReproduce() && enteringOrganism.CanReproduce())
        {
            //Reproduce(enteringOrganism);
        }
    }

    private void FixedUpdate()
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
    
    public void RestartEvaluators()
    {
        fitnessEvaluators.ForEach(x => x.Restart());
    }

    public void ReadOutput()
    {
        if (useOutput)
        {
            brain.Predict();
            AccelerateOrganism(brain.GetOutput("Horizontal"), brain.GetOutput("Vertical"), brain.GetOutput("Acceleration"));
        }

        MoveOrganism();
    }

    public void MoveOrganism()
    {
        direction = new Vector3(horizontal, 0f, vertical);
        //targetWalkBlendSpeed = direction.magnitude;//Target speed=<0,1> because it is only affecting blend tree and blends between idle(0) and run(1) with walk in between those two
        if (rigidbody)
        {
            rigidbody.position += direction.normalized * speedMultiplier * Time.deltaTime;
        }
        else
        {
            movingTransform.position += direction.normalized * speedMultiplier * Time.deltaTime;
        }
        
        if(direction!=Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction,Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed*Time.deltaTime);
        }
    }

    private void AccelerateOrganism(float targetHorizontal, float targetVertical, float newAcceleration)
    {
        acceleration = newAcceleration;
        targetHorizontal = Mathf.Max(maxMovementSpeed, targetHorizontal);
        targetHorizontal = Mathf.Max(maxMovementSpeed, targetVertical);

        if (horizontal<targetHorizontal*(1f-deadzone))
        {
            horizontal += acceleration * Time.deltaTime;
        }

        if (horizontal > targetHorizontal * (1f + deadzone))
        {
            horizontal -= acceleration * Time.deltaTime;
        }

        if (vertical < targetVertical * (1f - deadzone))
        {
            vertical += acceleration * Time.deltaTime;
        }

        if (vertical > targetVertical * (1f + deadzone))
        {
            vertical -= acceleration * Time.deltaTime;
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

    public GameObject Reproduce(Organism enteringOrganism)
    {
        return Reproduce(enteringOrganism, transform.position);
    }

    public GameObject Reproduce(Organism enteringOrganism,Vector3 position)
    {
        StartReproductionCooldown();
        enteringOrganism.StartReproductionCooldown();

        GameObject kid = Instantiate(gameObject, position, transform.rotation);
        Organism kidOrganism = kid.GetComponent<Organism>();
        kidOrganism.fitness = 0;

        if (fitness < enteringOrganism.fitness)
        {
            kidOrganism.brain = new NeuralNetwork(enteringOrganism.brain, brain);
        }
        else
        {
            kidOrganism.brain = new NeuralNetwork(brain, enteringOrganism.brain);
        }

        kidOrganism.StartReproductionCooldown();
        return kid;
    }

    public void Reset()
    {
        foreach(Resetter resetter in resetters)
        {
            resetter.Reset();
        }
    }
}
