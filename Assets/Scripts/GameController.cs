using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public EnemyAI[] enemies;
    public bool seeingPlayer;
    public StateMachine<GameController> StateMachine { get; private set; }
    

    //Variáveis da câmera
    private Camera mainCamera;
    private GameObject player;
    public LayerMask wallsLayer;

    private void Awake()
    {
        enemies = FindObjectsOfType<EnemyAI>();
        mainCamera = FindObjectOfType<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Start()
    {



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

   /* void WallTransparency()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(player.transform.position);

        if (Physics.Raycast(ray, out hit, wallsLayer))
        {
            if (hit.collider != null)
            {
             
            }
    }

    
    */

    // talvez mover isso para outro código "Enemy Manager" ou "Enemy Controller"
    public void ChasePlayer()
    {
        

        foreach (EnemyAI enemy in enemies)
        {
            enemy.StartChasing();    
        }
    }

    
}
