using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MechanicNPC : MonoBehaviour
{
    public float searchRadius = 15f;
    public float stopDistance = 3f;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float repairTime = 5f;

    private NavMeshAgent agent;
    private float timer;
    private float repairTimer;
    private GameObject[] arcadeMachines;
    public bool isRepairing = false;

    public Animator animator;    // Referencia al Animator

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        repairTimer = repairTime;

        arcadeMachines = GameObject.FindGameObjectsWithTag("ArcadeMachine");

        //animator = GetComponent<Animator>(); // Asumiendo el Animator está en el mismo GameObject
        if (animator == null)
        {
            Debug.LogWarning("Animator no encontrado en MechanicNPC.");
        }
    }
    void Update()
    {
        GameObject closestMachine = GetClosestBrokenMachine();

        if (closestMachine != null)
        {
            float distanceToMachine = Vector3.Distance(transform.position, closestMachine.transform.position);
            if (distanceToMachine > stopDistance)
            {
                agent.SetDestination(closestMachine.transform.position);
            }
            else
            {
                // Ya está cerca
                if (!isRepairing)
                {
                    Debug.Log("Comenzando la reparación de la máquina...");
                    isRepairing = true;
                    repairTimer = repairTime;
                    SetIsFix(true); // Activar animación de reparación al comenzar
                }

                if (isRepairing)
                {
                    repairTimer -= Time.deltaTime;
                    if (repairTimer <= 0f)
                    {
                        ArcadeMachine arcade = closestMachine.GetComponent<ArcadeMachine>();
                        if (arcade != null && arcade.currentState == ArcadeMachine.ArcadeState.Broken)
                        {
                            arcade.RepairMachine();
                            Debug.Log("La máquina ha sido reparada.");
                        }

                        isRepairing = false;
                        
                    }
                }
            }
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
                agent.SetDestination(newPos);
                timer = 0;
            }

            isRepairing = false;
        }

        animator.SetBool("isFix", isRepairing);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }



    void SetIsFix(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("isFix", value);
        }
    }

    GameObject GetClosestBrokenMachine()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var machine in arcadeMachines)
        {
            ArcadeMachine arcade = machine.GetComponent<ArcadeMachine>();
            if (arcade != null && arcade.currentState == ArcadeMachine.ArcadeState.Broken)
            {
                float distanceToMachine = Vector3.Distance(transform.position, machine.transform.position);
                if (distanceToMachine < minDistance)
                {
                    minDistance = distanceToMachine;
                    closest = machine;
                }
            }
        }

        return closest;
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

        return navHit.position;
    }
}
