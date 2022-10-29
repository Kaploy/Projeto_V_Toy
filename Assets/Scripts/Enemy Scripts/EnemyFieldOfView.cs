using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EnemyFieldOfView : MonoBehaviour
{

    

    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    bool isChasing = false;
    public EnemyActions enemyActions;

    private void Start()
    {
        enemyActions = GetComponent<EnemyActions>();
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {

        // as ações são chamadas se o player é avistado ou perdido de vista
        if (!isChasing && canSeePlayer)
        {
            enemyActions.OnPlayerDetected();
            isChasing = true;
        }
        else if(isChasing && !canSeePlayer)
        {
            enemyActions.PlayerMissing();
            isChasing = false; 
        }
        else
        {

        }
    }

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
