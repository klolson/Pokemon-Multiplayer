using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public Vector3 leftRacketSpawn = new Vector3(-10.5f, 0.5f, 0);
    public Vector3 rightRacketSpawn = new Vector3(10.5f, 0.5f, 0);
    public float moveSpeed;
    //public VectorValue startingPosition;

    //private Character character;
    private Animator animator;

    private Vector2 movement;
    private bool isMoving;

    [SyncVar]
    public int health = 4;

    public Slider healthbar;
    public bool takeDamage;
    public Text playerName;

    public Transform attackPoint;
    public float attackRange = 1.0f;
    public LayerMask enemyLayers;
    public LayerMask collision;

    private int numDeaths = 0;

    public GameObject vCamera;

    public GameObject respawnUI;
    public Button respawnButton;
    public GameObject healthSliderUI;

    void DoAttack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            PlayerController e = enemy.GetComponent<PlayerController>();
            if (!e.isLocalPlayer)
            {
                SendDamage(e.netId);
                //e.DoDamage();
                Debug.Log("Attacked " + enemy.name + ". They have " + e.health + " health left.");
            }

            /*if (enemy.GetComponent<PlayerController>().isLocalPlayer) enemy.GetComponent<PlayerController>().takeDamage = true;*/
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public override void OnStartClient()
    {
        if (!isLocalPlayer)
        {
            vCamera.SetActive(false);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        healthbar = GetComponent<Slider>();
        healthbar = GetComponentInChildren<Canvas>().GetComponentInChildren<Slider>();
        healthbar.maxValue = 4;
        healthbar.minValue = 0;
        healthbar.value = 4;
        respawnUI.SetActive(false);
        //character = GetComponent<Character>();
        //transform.position = startingPosition.initialValue;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isLocalPlayer)
        {
            playerName.text = "Charmander";
        }

        if (isLocalPlayer && health == 0)
        {
            animator.SetTrigger("Dead");
            Respawn();
        }
        if (!isLocalPlayer)
        {
            healthSliderUI.SetActive(false);
            respawnUI.SetActive(false);
        }

        if (isLocalPlayer)
        {
            healthbar.value = health;

            if (!isMoving && health != 0 )
            {
                // Input
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                //find the next target position when a player attempts to move
                if (movement != Vector2.zero)
                {
                    animator.SetFloat("MoveX", movement.x);
                    animator.SetFloat("MoveY", movement.y);

                    var targetPos = transform.position;
                    targetPos.x += movement.x;
                    targetPos.y += movement.y;

                    if (IsPathClear(targetPos))
                        StartCoroutine(Move(targetPos));
                }
            }

            animator.SetBool("Moving", isMoving);
            //character.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoAttack();
            }
            if (Input.GetKeyDown(KeyCode.L) && health == 0)
            {
                OnRespawn();
            }
        }
    }

    private void Respawn()
    {
        Debug.Log("Respawning....");
        respawnUI.SetActive(true);
        //playerName.text = "Charmander" + numDeaths;

        //healthbar.value = health;
    }
    public void OnRespawn()
    {
        respawnUI.SetActive(false);
        health = 4;
        System.Random rnd = new System.Random();
        if (rnd.Next(2) == 1) transform.position = leftRacketSpawn;
        else transform.position = rightRacketSpawn;

        animator.SetTrigger("Respawn");
        //respawn
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }

    private bool IsPathClear(Vector3 targetPosition)
    {
        var difference = targetPosition - transform.position;
        var direction = difference.normalized;

        if (Physics2D.OverlapCircle(targetPosition, 0.3f, enemyLayers | collision) != null)
        {
            return false;
        }

        /*
        if (Physics2D.BoxCast(transform.position + direction, new Vector2(0.2f, 0.2f), 0f, direction, difference.magnitude - 1, GameplayLayers.i.SolidLayer | GameplayLayers.i.InteractLayer | GameplayLayers.i.PlayerLayer) == true)
            return false;
        else*/
        return true;
    }

    [Command]
    void SendDamage(uint id)
    {
        var enemy = NetworkIdentity.spawned[id].gameObject;
        enemy.GetComponent<PlayerController>().health--;

    }
}
