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
    private Slider healthbar;
    private bool takeDamage;

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
            Debug.Log("Test");
            if (!isMoving)
            {
                // Input
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                //find the next target position when a player attempts to move
                if (movement != Vector2.zero)
                {
                    Debug.Log("Movement X: " + movement.x);
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

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("Pressed Z in the player controller");
                //Interact();
            }
        }

        if (isLocalPlayer)
        {
            if (takeDamage)
            {
                takeDamage = false;
                healthbar.value--;
                if (healthbar.value == 0) Debug.Log("No health left.");
            }
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
