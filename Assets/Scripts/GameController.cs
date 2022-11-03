using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<EnemyAI> enemies = new List<EnemyAI>();
    public GameObject enemy;
    
    public StateMachine<GameController> StateMachine { get; private set; }

    private void Start()
    {
        
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.instance);
    }

    private void Update()
    {
        StateMachine.Execute();
    }

    // talvez mover isso para outro código "Enemy Manager" ou "Enemy Controller"
    public void ChasePlayer()
    {
        Debug.Log("Chasing player");

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(enemy != null)
            {
                enemy.GetComponent<EnemyAI>().StartChasing();
            }
              
        }
    }
}
