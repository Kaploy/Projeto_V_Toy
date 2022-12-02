using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        freeroam,
        attacking
    }

    public State playerState;
    public static PlayerController instance;
    public CharacterController controller;
    public float speed = 6;
    public bool playerVisible = true;
    public Animator playerAnimator;

    [SerializeField] private LayerMask groundMask;
    private Camera mainCamera;

    //Variáveis de combate
    public float playerHP = 10;
    public Transform swordBase;
    public Transform swordPoint;
    public float attackRange;
    public LayerMask enemyLayer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    
    public void Update()
    {
        if(playerHP <= 0)
        {
            Destroy(gameObject);

        }

        switch (playerState)
        {
            case State.attacking:

                AttackPattern();


                break;
            default:
            case State.freeroam:

                FreeroamMovement();

                break;
        }
        
    }


    public void FreeroamMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var correctedDirection = matrix.MultiplyPoint3x4(direction);

        if (direction != Vector3.zero)
        {
            controller.Move(correctedDirection * speed * Time.deltaTime);
            transform.forward = correctedDirection;
            playerAnimator.SetBool("moving", true);
        }
        else
        {
            playerAnimator.SetBool("moving", false);
        }

        var (success, position) = GetMousePosition();

        if (Input.GetMouseButtonDown(0))
        {

            playerAnimator.SetTrigger("attack");

            //var faceDirection = position - transform.position;
            
            //transform.forward = faceDirection;
            
            
            
            
        }
    }

    public void StartAttacking()
    {
        
        playerState = State.attacking;
    }

    public void AttackPattern()
    {
        

        Collider[] hitEnemies = Physics.OverlapCapsule(swordBase.position, swordPoint.position, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyAI>().TakeDamage();
        }
    }

    public void TakeDamage()
    {
        playerHP = -3f;
    }

    IEnumerable DeathRoutine()
    {
        Destroy(gameObject);

        yield return new WaitForSeconds(3);
        //MOVER ESSE CÓDIGO PARA O GAMECONTROLLER E REMOVER O SCENEMANAGER
        SceneManager.LoadScene(0);

    }

    private void OnDrawGizmosSelected()
    {
        if(swordPoint == null)
            return;
        if (swordBase == null)
            return;
        
        Gizmos.DrawWireSphere(swordBase.position, attackRange);
        Gizmos.DrawWireSphere(swordPoint.position, attackRange);
    }

    public void StopAttacking()
    {
        playerState = State.freeroam;
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            
            return (success: true, position: hitInfo.point);
        }
        else
        {
            
            return (success: false, position: Vector3.zero);
        }
    }
}
