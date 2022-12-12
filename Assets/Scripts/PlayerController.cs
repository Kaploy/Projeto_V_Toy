using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        freeroam,
        attacking,
        dodging,
        takingdamage
    }

    public State playerState;
    public static PlayerController instance;
    public CharacterController controller;
    [SerializeField] float speed = 6;
    public bool playerVisible = true;
    public Animator playerAnimator;

    [SerializeField] private LayerMask groundMask;
    private Camera mainCamera;

    //Variáveis de combate
    [SerializeField] float playerHP = 10;
    public Transform swordBase;
    public Transform swordPoint;
    [SerializeField] float attackRange;
    public LayerMask enemyLayer;
    public bool canBeDamaged = true;

    //Outras variáveis

    [SerializeField] float turnSpeed = 1000f;

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

            case State.dodging:
                
                //INFORMAÇÕES DA ANIMAÇÃO, CONTROLE DO "CANBEDAMAGED", CONTROLE DA STEALTH E MOVIMENTAÇÃO

                break;

            case State.takingdamage:

                //INFORMAÇÕES DA ANIMAÇÃO, CONTROLE DO "CANBEDAMAGED", PARAR MOVIMENTAÇÃO

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
            LookAtMouse();
            playerAnimator.SetTrigger("attack");

            
            
            
            
            
        }
    }

    private void LookAtMouse()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDist;

        if(playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

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
        if (canBeDamaged)
        {
            playerHP = -3f;
        }
    }

    IEnumerable DeathRoutine()
    {
        Destroy(gameObject);

        yield return new WaitForSeconds(3);
        //MOVER ESSE CÓDIGO PARA O GAMECONTROLLER E REMOVER O SCENEMANAGER
        SceneManager.LoadScene(0);

    }

    private void DodgeRoll()
    {
        canBeDamaged = false;
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
