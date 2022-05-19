using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Organism : MonoBehaviour
{
    [Header("Brain")]
    public bool useOutput = true;
    public NeuralNetwork neuralNetwork = null;//BRAIN MUST BE SETUP BEFORE RUNNING THE GAME!!!
    public List<string> inputs;
    public List<string> outputs;
    public string neuralNetworkOptions;
    [Header("Reproduction")]
    public float reproductionCooldown = 1f;
    public float timeLeftToReproduction = 1f;
    [Header("Statistics")]
    public bool dead = false;
    public float food = 100f;
    public float foodDecreaseRate = 1f;
    [Header("Evaluators")]
    public List<FitnessEvaluator> fitnessEvaluators;
    public List<InputEvaluator> inputEvaluators;
    public List<Resetter> resetters;
    [Header("Actions")]
    public float animationSpeedMultiplier = 0.1f;
    public float rotationSpeed = 1f;
    public float acceleration = .2f;
    public float deadzone = 0.05f;
    public float horizontal;
    public float vertical;
    public Vector3 direction;
    public Transform movingTransform;
    private Animator animator;
    [Header("Overrides")]
    public bool overrideInputs = false;
    public float horizontalOverride = -10.0f;
    public float verticalOverride = -10.0f;

    public void OnCollisionEnter(Collision collision)
    {
        if (!dead)
        {
            Organism enteringOrganism = collision.gameObject.GetComponent<Organism>();
            if (enteringOrganism != null && CanReproduce() && enteringOrganism.CanReproduce())
            {
                Reproduce(enteringOrganism);
            }

            Interactable interactableObject = collision.gameObject.GetComponent<Interactable>();
            if (interactableObject != null)
            {
                interactableObject.Interact(this);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            foreach (FitnessEvaluator evaluator in fitnessEvaluators)
            {
                evaluator.UpdateEvalutator(this);
            }

            foreach (InputEvaluator evaluator in inputEvaluators)
            {
                evaluator.UpdateEvalutator(this);
            }

            ReadOutput();

            if (timeLeftToReproduction != 0)
            {
                timeLeftToReproduction -= Time.fixedDeltaTime;

                if (timeLeftToReproduction < 0f)
                {
                    timeLeftToReproduction = 0f;
                }
            }

            //Update stats
            food -= foodDecreaseRate * Time.fixedDeltaTime;
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
            neuralNetwork.Predict();

            float forward = Mathf.Max(neuralNetwork.GetOutput("Forward") / 10, 0);
            float backward = Mathf.Max(neuralNetwork.GetOutput("Backward") / 10, 0);
            float left = Mathf.Max(neuralNetwork.GetOutput("Left") / 10, 0);
            float right = Mathf.Max(neuralNetwork.GetOutput("Right") / 10, 0);

            float targetVertical = forward > backward ? forward : -backward;
            float targetHorizontal = right > left ? right : -left;

            AccelerateOrganism(targetHorizontal, targetVertical, acceleration);
        }

        MoveOrganism();
    }

    public void MoveOrganism()
    {
        direction = movingTransform.forward * vertical;
        direction = Quaternion.AngleAxis(-rotationSpeed * horizontal * Time.deltaTime, Vector3.up) * direction;

        movingTransform.position += direction * Time.deltaTime;
        

        if (animator!=null && direction.magnitude > 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("speed", direction.magnitude * animationSpeedMultiplier);

        }

        if(direction!=Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction,Vector3.up);

            movingTransform.rotation = Quaternion.RotateTowards(movingTransform.rotation, toRotation, rotationSpeed*Time.deltaTime);
        }
    }

    private void AccelerateOrganism(float targetHorizontal, float targetVertical, float newAcceleration)
    {
        acceleration = newAcceleration;

        if (!overrideInputs)
        {
            //targetHorizontal = Mathf.Min(maxMovementSpeed, targetHorizontal);
            //targetVertical = Mathf.Min(maxMovementSpeed, targetVertical);
        }
        else
        {
            targetHorizontal = horizontalOverride;
            targetVertical = verticalOverride;
        }

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

        if (neuralNetwork.fitness < enteringOrganism.neuralNetwork.fitness)
        {
            kidOrganism.neuralNetwork = new NeuralNetwork(enteringOrganism.neuralNetwork, neuralNetwork);
        }
        else
        {
            kidOrganism.neuralNetwork = new NeuralNetwork(neuralNetwork, enteringOrganism.neuralNetwork);
        }

        kidOrganism.neuralNetwork.fitness = 0;
        kidOrganism.StartReproductionCooldown();
        return kid;
    }

    public void SerializeBrain(string path, string name)
    {
        string json = JsonUtility.ToJson(neuralNetwork);
        StreamWriter writer = File.CreateText(path+ name +"_neuralnetowork.json");
        writer.WriteLine(json);
        writer.Close();
    }

    public void DeserializeBrain(string path, string name)
    {
        string json = File.ReadAllText(path + name + "_neuralnetowork.json");
        neuralNetwork = JsonUtility.FromJson<NeuralNetwork>(json);
    }

    public void GenerateNeuralNetwork()
    {
        neuralNetwork = new NeuralNetwork(inputs, outputs, neuralNetworkOptions);
    }

    public void Reset()
    {
        foreach(Resetter resetter in resetters)
        {
            resetter.Reset();
        }
    }

    public void Start()
    {
        SetVariables();
    }

    private void SetVariables()
    {
        animator = gameObject.GetComponent<Animator>();
    }
}
