using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public class EnemyAI : MonoBehaviour
{
    // variaveis da state machine
    private enum State { 
        patrolling, 
        chasing,
        resetting}

    private State state;
  
    // variaveis da movimenta��o
    NavMeshAgent agent;
    public Transform[] waypoints;
    int waypointIndex;
    Vector3 target;
    public GameObject player;

    // variaveis do padr�o de reset de patrulha
    public float resetTimer;
    public float breakTimer = 1;
    bool resetStarted;

    // eventos
    

    // variaveis do fov

    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    

    private void Awake()
    {
        state = State.patrolling;
    }

    void Start()
    {
        // Inicia as fun��es do fov
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());

        // Inicia pathfinding da movimenta��o

        agent = GetComponent<NavMeshAgent>();

        // Quando a StateMachine estiver implementada, trocar isso aqui pra entrada do state Roaming
        UpdateDestination();
    }

    

    void Update()
    {
        switch (state)
        {
            
            case State.chasing:
                if (!canSeePlayer)
                {
                    state = State.resetting;
                }
                agent.SetDestination(player.transform.position);
                break;

            case State.resetting:
                if (!resetStarted)
                {
                    resetStarted = true;
                    StartCoroutine(ResetPattern());
                    
                }
                break;

            default:
            case State.patrolling:
                if (canSeePlayer)
                {
                    state = State.chasing;
                }
                if (Vector3.Distance(transform.position, target) < 1)
                {
                    IterateWaypointIndex();
                    UpdateDestination();
                }

                break;            
                
        }

        
        
    }

    //c�digos da movimenta��o de patrulha
    void UpdateDestination()
    {
        target = waypoints[waypointIndex].position;
        agent.SetDestination(target);
    }
    void IterateWaypointIndex()
    {
        waypointIndex++;
        if(waypointIndex== waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    //c�digo de resetar o padr�o de patrulha ao perder o player de vista
    IEnumerator ResetPattern()
    {
        yield return new WaitForSeconds(resetTimer);
        agent.isStopped = true;
        yield return new WaitForSeconds(breakTimer);
        state = State.patrolling;
        agent.isStopped = false;
        UpdateDestination();
        resetStarted = false;
        
    }

    

    //c�digos do fov
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    public void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
