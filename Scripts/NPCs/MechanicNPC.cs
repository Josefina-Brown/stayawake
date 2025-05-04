using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MechanicNPC : MonoBehaviour
{
    public float searchRadius = 15f;         // Distancia máxima para empezar a seguir las máquinas rotas
    public float stopDistance = 3f;          // Distancia mínima a mantener de la máquina
    public float wanderRadius = 10f;         // Radio para deambular
    public float wanderTimer = 5f;           // Tiempo para deambular antes de elegir un nuevo destino
    public float repairTime = 5f;            // Tiempo que el NPC tarda en reparar la máquina (en segundos)

    private NavMeshAgent agent;
    private float timer;
    private float repairTimer;               // Temporizador para la reparación
    private GameObject[] arcadeMachines;     // Lista de las máquinas de arcade en la escena
    private bool isRepairing = false;        // Indica si el NPC está reparando una máquina

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        repairTimer = repairTime;

        // Buscar todas las máquinas de arcade en la escena con el tag "Arcade"
        arcadeMachines = GameObject.FindGameObjectsWithTag("ArcadeMachine");
    }

    void Update()
    {
        GameObject closestMachine = GetClosestBrokenMachine();
        
        if (closestMachine != null)
        {
            // Si encontramos una máquina rota, nos dirigimos hacia ella
            float distanceToMachine = Vector3.Distance(transform.position, closestMachine.transform.position);
            if (distanceToMachine > stopDistance)
            {
                agent.SetDestination(closestMachine.transform.position);
                isRepairing = false;  // Si estamos moviéndonos, no estamos reparando
            }
            else
            {
                // Comenzamos a reparar si estamos cerca de la máquina
                if (!isRepairing)
                {
                    Debug.Log("Comenzando la reparación de la máquina...");
                    isRepairing = true;
                    repairTimer = repairTime;
                }

                // Si estamos reparando, contamos el tiempo
                if (isRepairing)
                {
                    repairTimer -= Time.deltaTime;

                    // Si hemos terminado de reparar
                    if (repairTimer <= 0f)
                    {
                        ArcadeMachine arcade = closestMachine.GetComponent<ArcadeMachine>();
                        if (arcade != null && arcade.currentState == ArcadeMachine.ArcadeState.Broken)
                        {
                            arcade.RepairMachine();  // Reparar la máquina
                            Debug.Log("La máquina ha sido reparada.");
                        }
                        isRepairing = false;  // Terminamos de reparar la máquina
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
        }
    }

    // Método para encontrar la máquina rota más cercana
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

    // Método para generar un punto aleatorio dentro de un radio
    Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

        return navHit.position;
    }
}
