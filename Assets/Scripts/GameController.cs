using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public EnemyAI[] enemies;
    public bool seeingPlayer;
    public StateMachine<GameController> StateMachine { get; private set; }


    private void Awake()
    {
        enemies = FindObjectsOfType<EnemyAI>();
    }
    private void Start()
    {

        
        //StateMachine = new StateMachine<GameController>(this);
        //tateMachine.ChangeState(FreeRoamState.instance);



    }

    private void Update()
    {
        seeingPlayer = EnemiesSeeingPlayer();
    }

    private bool EnemiesSeeingPlayer()
    {
        foreach (EnemyAI enemy in enemies)
        {
            if (enemy.canSeePlayer == true)
            {

                return true;
            }

        } 
        return false;
        
    }

    


    // talvez mover isso para outro código "Enemy Manager" ou "Enemy Controller"
    public void ChasePlayer()
    {
        

        foreach (EnemyAI enemy in enemies)
        {
            enemy.StartChasing();    
        }
    }

    
}
