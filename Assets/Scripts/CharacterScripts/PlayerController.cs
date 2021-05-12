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
    public int health = 4;
    public Slider healthbar;
    public bool takeDamage;
    public Text playerName;

    public Transform attackPoint;
    public float attackRange = 1.0f;
    public LayerMask enemyLayers;

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
                if (e.health == 0)
                {
                    StartCoroutine(e.Respawn());
                }
            }
            /*if (enemy.GetComponent<PlayerController>().isLocalPlayer) enemy.GetComponent<PlayerController>().takeDamage = true;*/
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    private void Awake()
    {
        animator = GetComponent<Animator>();
        healthbar = GetComponent<Slider>();
        healthbar = GetComponentInChildren<Canvas>().GetComponentInChildren<Slider>();
        healthbar.maxValue = 4;
        healthbar.minValue = 4;
        healthbar.value = 4;
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
        else
        {
            playerName.text = "";
        }

        if (isLocalPlayer)
        {
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

    private IEnumerator Respawn()
    {
        playerName.text = "You died.";
        yield return new WaitForSeconds(2);
        playerName.text = "Charmander";
        health = 4;
    }

    private void DoDamage()
    {
        takeDamage = false;
        health--;
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

    private void Interact()
    {
        //var faceDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        //var interactPos = transform.position + faceDir;

        /*
        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameplayLayers.i.InteractLayer);
        if (collider != null && GameController.state == GameState.Roam)
        {
            Debug.Log("Initiating Dialog");
            collider.GetComponent<Interactable>()?.Interact(transform);
        }*/
    }

    private void OnMoveOver()
    {
        //CheckForEncounters();
        //CheckIfInTrainerView();
    }
}
