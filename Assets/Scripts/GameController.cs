using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public EnemyAI[] enemies;
    public bool seeingPlayer;
    public StateMachine<GameController> StateMachine { get; private set; }
    

    //Variáveis da câmera
    private Camera mainCamera;
    private GameObject player;
    public LayerMask wallsLayer;

    //Variáveis do menu de pause
    static bool gameIsPaused = false;
    public GameObject pauseMenuUI;

    Scene scene;
    private int sceneIndex;

    private void Awake()
    {
        enemies = FindObjectsOfType<EnemyAI>();
        mainCamera = FindObjectOfType<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        scene = SceneManager.GetActiveScene();
        sceneIndex = scene.buildIndex;
        
        
    }
    private void Start()
    {
        PlayerPrefs.SetInt("SavedLevel", sceneIndex);
    }


    private void Update()
    {
        seeingPlayer = EnemiesSeeingPlayer();
        GamePauseLogic();
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



    //FUNÇÕES DO MENU DE PAUSE
    void GamePauseLogic()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void LoadMainMenu()
    {
        //SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    
}
