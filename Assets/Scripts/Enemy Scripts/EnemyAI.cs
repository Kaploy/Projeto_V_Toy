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
        detecting,
        chasing,
        resetting}

    private State state;
    private GameController gameController;
    // variaveis da movimentação
    NavMeshAgent agent;
    public Transform[] waypoints;
    int waypointIndex;
    Vector3 target;
    public GameObject player;

    // variaveis do padrão de reset de patrulha
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

    // outras variáveis da IA
    public float detectionTime = 2f;
    bool detectedPlayer = false;

    private void Awake()
    {
        state = State.patrolling;
    }

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        // Inicia as funções do fov
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());

        // Inicia pathfinding da movimentação

        agent = GetComponent<NavMeshAgent>();

        // Quando a StateMachine estiver implementada, trocar isso aqui pra entrada do state Roaming
        UpdateDestination();
    }

    

    void Update()
    {
        switch (state)
        {
            
            case State.chasing:
                agent.speed = 5;
                if (!canSeePlayer)
                {
                    state = State.resetting;
                }
                agent.SetDestination(player.transform.position);
                break;

            case State.resetting:
                if (!resetStarted)
                {
                    // aqui também precisa de uma função que passe pelo game controller
                    resetStarted = true;
                    StartCoroutine(ResetPattern());
                    
                }
                break;
            case State.detecting:
                agent.speed = 1;
                // precisa virar o inimigo uma vez na direção do player também
                if (canSeePlayer && !detectedPlayer)
                {
                    detectionTime -= Time.deltaTime;
                    if (detectionTime <= 0)
                    {
                        detectedPlayer = true;
                        // chama todos inimigos ao mesmo tempo
                        gameController.ChasePlayer();
                    }
                }
                else
                {
                    detectionTime = 2f;
                    state = State.patrolling;
                }
                
                break; 

            default:
            case State.patrolling:
                agent.speed = 3.5f;
                if (canSeePlayer)
                {
                    state = State.detecting;
                }
                if (Vector3.Distance(transform.position, target) < 1)
                {
                    IterateWaypointIndex();
                    UpdateDestination();
                }

                break;            
                
        }

        
        
    }

    //códigos da movimentação de patrulha
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

    //código de resetar o padrão de patrulha ao perder o player de vista
    IEnumerator ResetPattern()
    {
        yield return new WaitForSeconds(resetTimer);
        agent.isStopped = true;
        yield return new WaitForSeconds(breakTimer);
        state = State.patrolling;
        agent.isStopped = false;
        UpdateDestination();
        detectedPlayer = false;
        resetStarted = false;
        
    }

    public void StartChasing()
    {
        state = State.chasing;
    }

    //códigos do fov
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
