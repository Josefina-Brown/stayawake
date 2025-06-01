using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]public class BullyNPC : MonoBehaviour
{
    public List<Transform> arcadePositions = new List<Transform>();
    public float damageRadius = 2f;
    public float damageDelay = 2f;
    public float damageChance = 0.5f; // Probabilidad de dañar la máquina (50%)
    public Animator animator;

    private NavMeshAgent agent;
    private Transform currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject[] arcades = GameObject.FindGameObjectsWithTag("ArcadeMachine");
        //foreach (GameObject arcade in arcades)
        //{
        //    arcadePositions.Add(arcade.transform);
        //}

        if (arcadePositions.Count > 0)
        {
            GoToNextArcade();
        }
    }

    void Update()
    {
        if (arcadePositions.Count == 0)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Solo si no estamos ya simulando un daño
            if (!isSimulating)
                StartCoroutine(DecideToDamage());
        }
    }

    private bool isSimulating = false;

    IEnumerator DecideToDamage()
    {
        isSimulating = true;

        // Espera un breve tiempo como si el NPC evaluara la máquina
        //yield return new WaitForSeconds(1f);

        if (Random.value <= damageChance)
        {
            yield return StartCoroutine(SimulateDamageAndThenApply());
        }
        else
        {
            GoToNextArcade(); // Se va sin dañar
        }

        isSimulating = false;
    }

    IEnumerator SimulateDamageAndThenApply()
    {
        animator.SetBool("isBreaking", true);
        yield return new WaitForSeconds(damageDelay);
        animator.SetBool("isBreaking", false);
        DamageArcade();
        GoToNextArcade();
    }

    void GoToNextArcade()
    {
        if (arcadePositions.Count == 0)
            return;

        List<Transform> readyArcades = new List<Transform>();
        foreach (Transform arcade in arcadePositions)
        {
            ArcadeMachine machine = arcade.GetComponent<ArcadeMachine>();
            if (machine != null && machine.currentState == ArcadeMachine.ArcadeState.Ready)
            {
                readyArcades.Add(arcade);
            }
        }

        if (readyArcades.Count > 0)
        {
            int index = Random.Range(0, readyArcades.Count);
            currentTarget = readyArcades[index];
            agent.SetDestination(currentTarget.position);
        }
    }

    void DamageArcade()
    {
        Collider[] arcadesInRange = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider arcade in arcadesInRange)
        {
            if (arcade.CompareTag("ArcadeMachine"))
            {
                ArcadeMachine machine = arcade.GetComponent<ArcadeMachine>();
                if (machine != null)
                {
                    machine.DamageMachine();
                }
            }
        }
    }
}
