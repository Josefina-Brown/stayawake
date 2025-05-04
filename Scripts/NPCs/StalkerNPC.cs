using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class StlakerNPC : MonoBehaviour
{
    public Animator animator;
    public float followRadius = 15f;         // Distancia máxima para empezar a seguir al jugador
    public float stopDistance = 3f;          // Distancia mínima a mantener
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public Transform player;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;

        if (player == null && GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }

    void Update()
    {

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < followRadius && distanceToPlayer > stopDistance)
        {
            // Sigue al jugador si está dentro del radio y fuera de la zona de parada
            agent.SetDestination(player.position);
        }
        else if (distanceToPlayer >= followRadius)
        {
            // Deambula si el jugador está lejos
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
        else
        {
            // Demasiado cerca, se detiene
            agent.ResetPath();
        }
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
