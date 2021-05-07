using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    //public VectorValue startingPosition;

    private Character character;

    private Vector2 movement;

    private void Awake()
    {
        character = GetComponent<Character>();
        //transform.position = startingPosition.initialValue;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!character.IsMoving)
        {
            // Input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            //prevents diagonal movement
            if (movement.x != 0) movement.y = 0;

            //find the next target position when a player attempts to move
            if (movement != Vector2.zero)
            {
                //makes sure an area is walkable before allowing a player move
                StartCoroutine(character.Move(movement, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Pressed Z in the player controller");
            Interact();
        }
    }

    private void Interact()
    {
        var faceDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + faceDir;

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
