using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public class EnemyAI : MonoBehaviour
{
    // variaveis da state machine
    public enum State { 
        patrolling,
        detecting,
        chasing,
        resetting}

    public State state;
    private GameController gameController;

    // variaveis da movimenta��o
    NavMeshAgent agent;
    public Transform[] waypoints;
    int waypointIndex;
    Vector3 target;
    public GameObject player;
    Vector3 playerPosition;

    // variaveis do padr�o de reset de patrulha
    public float resetTimer;
    public float breakTimer = 1;
    public bool resetStarted;

    // eventos
    

    // variaveis do fov

    public float radius;
    [Range(0, 360)]
    public float angle;

    public PlayerController playerController;
    

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    // outras vari�veis da IA
    public float detectionTime = 2f;
    bool detectedPlayer = false;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        state = State.patrolling;
        
    }

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        // Inicia as fun��es do fov
        
        StartCoroutine(FOVRoutine());

        // Inicia pathfinding da movimenta��o

        agent = GetComponent<NavMeshAgent>();

        // Quando a StateMachine estiver implementada, trocar isso aqui pra entrada do state Roaming
        UpdateDestination();

    }

    

    void Update()
    {
        playerPosition = playerController.transform.position;

        switch (state)
        {
            
            case State.chasing:
                agent.speed = 5;

                agent.SetDestination(playerController.transform.position);

                if (gameController.seeingPlayer == false)
                {
                    Debug.Log("missing player");
                    state = State.resetting;
                }
                else
                {
                    
                }

                break;

            case State.resetting:
                if (!resetStarted)
                {
                    // aqui tamb�m precisa de uma fun��o que passe pelo game controller
                    resetStarted = true;
                    ResetPattern();
                    
                }
                break;
            case State.detecting:
                
                agent.speed = 1;
                // precisa virar o inimigo uma vez na dire��o do player tamb�m
                DetectionPattern();
                
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

    void DetectionPattern()
    {
        if (canSeePlayer && !detectedPlayer)
        {
            detectionTime -= Time.deltaTime;
            if (detectionTime <= 0)
            {
                detectedPlayer = true;
                detectionTime = 2f;
                // chama todos inimigos ao mesmo tempo
                gameController.ChasePlayer();

            }
        }
        else
        {
            detectionTime = 2f;
            state = State.patrolling;
        }
    }
    //c�digo de resetar o padr�o de patrulha ao perder o player de vista
    private void ResetPattern()
    {
        
        state = State.patrolling;
        UpdateDestination();
        detectedPlayer = false;
        resetStarted = false;
        
    }

    public void StartChasing()
    {
        state = State.chasing;
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

    public void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(playerPosition);
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

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask) && playerController.playerVisible == true)
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
