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
        resetting,
        attacking}

    public State state;
    private GameController gameController;

    // variaveis da movimentação
    NavMeshAgent agent;
    public Transform[] waypoints;
    int waypointIndex;
    Vector3 target;
    public GameObject player;
    Vector3 playerPosition;
    float patrolDistance;
    float idleTime = 1f;

    // variaveis do padrão de reset de patrulha
    public float resetTimer;
    public float breakTimer = 1;
    public bool resetStarted;

    // variaveis da animação
    Animator enemyAnimator;

    // eventos
    

    // variaveis do fov
    public float radius;
    [Range(0, 360)]
    public float angle;
    public PlayerController playerController;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;

    //variáveis de combate
    public float attackRange = 2f;
    public float enemyHp = 10f;
    public Transform weaponBase;
    public Transform weaponPoint;
    public LayerMask playerLayer;

    // outras variáveis da IA
    public float detectionTime = 2f;
    bool detectedPlayer = false;
    public float chaseTime = 3f;
    Collider playerCollider;


    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        enemyAnimator = GetComponent<Animator>();
        state = State.patrolling;

        //pega o colisor do player para funções de detecção
        playerCollider = playerController.GetComponent<BoxCollider>();

    }

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        // Inicia as funções do fov
        
        StartCoroutine(FOVRoutine());

        // Inicia pathfinding da movimentação

        agent = GetComponent<NavMeshAgent>();

        // Quando a StateMachine estiver implementada, trocar isso aqui pra entrada do state Roaming
        UpdateDestination();

    }

    

    void Update()
    {
        playerPosition = playerController.transform.position;

        if (enemyHp <= 0)
        {
            Destroy(gameObject);
        }

       

        switch (state)
        {
            
            case State.chasing:
                agent.speed = 5;

                ChasePattern();

                break;

            case State.attacking:

                EnemyAttack();


                break;

            case State.resetting:
                if (!resetStarted)
                {
                    // aqui também precisa de uma função que passe pelo game controller
                    resetStarted = true;
                    ResetPattern();
                    
                }
                break;
            case State.detecting:
                
                agent.speed = 1;
                // precisa virar o inimigo uma vez na direção do player também
                DetectionPattern();
                
                break; 

            default:
            case State.patrolling:
                
                agent.speed = 2f;
                if (canSeePlayer)
                {
                    state = State.detecting;
                }

                if (Vector3.Distance(transform.position, target) < 1)
                {
                    //Aqui sempre precisa ter o IterateWaypointIndex e o UpdateDestination de alguma maneira
                    
                    StartCoroutine(PatrolBreak());
                    IterateWaypointIndex();
                    UpdateDestination();
                }

                //tornar isso aqui um método próprio ou colocar essas animações em uma BLEND TREE
                if (!agent.isStopped)
                {

                    enemyAnimator.SetInteger("currentStateNumber", 1);
                }
                else
                {
                    enemyAnimator.SetInteger("currentStateNumber", 0);
                }
                break;    
                

                
                
        }

        
        
    }

    //Se o player se aproximar demais, o inimigo ataca imediatamente: ESSE SISTEMA PRECISA SER MELHORADO E O BOX COLLIDER TEM QUE SER UM CONE

    private void OnTriggerEnter(Collider playerCollider)
    {

       if(state == State.patrolling && playerController.playerVisible == true)
        {
            gameController.ChasePlayer();
        }
       else if(state == State.resetting)
        {
            gameController.ChasePlayer();
        }
        else if(state == State.detecting)
        {
            gameController.ChasePlayer();
        }
        else
        {

        }
       
    }

    //padrões de cada state:


    void ChasePattern()
    {
        agent.SetDestination(playerController.transform.position);
        enemyAnimator.SetInteger("currentStateNumber", 2);
        patrolDistance = Vector3.Distance(playerController.transform.position, transform.position);

        if (gameController.seeingPlayer == false)
        {
            chaseTime -= Time.deltaTime;
            if(chaseTime <= 0f)
            {
                state = State.resetting;
            }
            
        }
        else
        {
            
        }
        
        if(patrolDistance <= attackRange)
        {
            enemyAnimator.SetInteger("currentStateNumber", 3);
        }
    }

    public void TakeDamage()
    {
        if(state == State.patrolling)
        {
            enemyHp =- 10f;
        }
        else if(state == State.attacking)
        {
            enemyHp =- 5f;
        }
        else
        {
            enemyHp =- 5f;
            state = State.chasing;
        }

        
    }

    void EnemyAttack()
    {
        
        Collider[] playerHit = Physics.OverlapCapsule(weaponBase.position, weaponPoint.position, attackRange, playerLayer);

        foreach (Collider player in playerHit)
        {
            player.GetComponent<PlayerController>().TakeDamage();
        }
    }

    void AttackStarted()
    {
        state = State.attacking;
    }

    void AttackEnded()
    {   
            state = State.chasing; 
    }
    //códigos da movimentação de patrulha
    void UpdateDestination()
    {
        target = waypoints[waypointIndex].position;
        transform.forward = target;
        patrolDistance = Vector3.Distance(transform.position, waypoints[waypointIndex].position);
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

    IEnumerator PatrolBreak()
    {
        agent.isStopped = true;
        
        yield return new WaitForSeconds(idleTime);
        agent.isStopped = false;
        


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
    //código de resetar o padrão de patrulha ao perder o player de vista
    private void ResetPattern()
    {
        
        state = State.patrolling;
        UpdateDestination();
        detectedPlayer = false;
        resetStarted = false;
        chaseTime = 3f;
        
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
