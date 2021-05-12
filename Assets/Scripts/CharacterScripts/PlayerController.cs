using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
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

    private int numDeaths = 0;

    public GameObject vCamera;

    void DoAttack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            PlayerController e = enemy.GetComponent<PlayerController>();
            if (!e.isLocalPlayer)
            {
                e.DoDamage();
                Debug.Log("Attacked " + enemy.name + ". They have " + e.health + " health left.");
                SendDamage(e.netId);
            }
            if (e.health == 0)
            {
                e.Respawn();
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
        //character = GetComponent<Character>();
        //transform.position = startingPosition.initialValue;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isLocalPlayer && numDeaths == 0)
        {
            playerName.text = "Charmander";
        }
        else if (isLocalPlayer)
        {
            playerName.text = "Charmander" + numDeaths;
        }

        healthbar.value = health;

        if (isLocalPlayer)
        {
            healthbar.value = health;

            if (!isMoving)
            {
                // Input
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                //find the next target position when a player attempts to move
                if (movement != Vector2.zero)
                {
                    //Debug.Log("Movement X: " + movement.x);
                    animator.SetFloat("MoveX", movement.x);
                    animator.SetFloat("MoveY", movement.y);

                    //makes sure an area is walkable before allowing a player move
                    //StartCoroutine(character.Move(movement, OnMoveOver));
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
        }
    }

    private void Respawn()
    {
        Debug.Log("Respawning....");
        //playerName.text = "Charmander" + numDeaths;
        numDeaths++;
        health = 4;
        healthbar.value = health;
    }

    private void DoDamage()
    {
        takeDamage = false;
        health--;
        healthbar.value = health;
        if (health <= 0)
        {
            Debug.Log("No health left.");
            //StartCoroutine(Respawn(this));
        }
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

        if (Physics2D.OverlapCircle(targetPosition, 0.3f, enemyLayers) != null)
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
