using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public int defaultPos;

    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkDiagonalUpLeftSprites;
    [SerializeField] List<Sprite> WalkDiagonalUpRightSprites;
    [SerializeField] List<Sprite> walkDiagonalDownLeftSprites;
    [SerializeField] List<Sprite> walkDiagonalDownRightSprites;

    // Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    //states
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator walkDiagonalUpLeftAnim;
    SpriteAnimator WalkDiagonalUpRightAnim;
    SpriteAnimator walkDiagonalDownLeftAnim;
    SpriteAnimator walkDiagonalDownRightAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;

    //references
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        walkDiagonalUpLeftAnim = new SpriteAnimator(walkDiagonalUpLeftSprites, spriteRenderer);
        WalkDiagonalUpRightAnim = new SpriteAnimator(WalkDiagonalUpRightSprites, spriteRenderer);
        walkDiagonalDownLeftAnim = new SpriteAnimator(walkDiagonalDownLeftSprites, spriteRenderer);
        walkDiagonalDownRightAnim = new SpriteAnimator(walkDiagonalDownRightSprites, spriteRenderer);

        switch (defaultPos)
        {
            case 1:
                currentAnim = walkUpAnim;
                break;
            case 2:
                currentAnim = walkRightAnim;
                break;
            case 3:
                currentAnim = walkDownAnim;
                break;
            case 4:
                currentAnim = walkLeftAnim;
                break;
            default:
                currentAnim = walkDownAnim;
                break;
        }

    }

    private void Update()
    {
        var prevAnim = currentAnim;

        if (MoveX == 1 && MoveY == 0)
        {
            currentAnim = walkRightAnim;
        }
        else if (MoveX == -1 && MoveY == 0)
            currentAnim = walkLeftAnim;
        else if (MoveY == 1 && MoveX == 0)
            currentAnim = walkUpAnim;
        else if (MoveY == -1 && MoveX == 0)
            currentAnim = walkDownAnim;
        else if (MoveX == -1 && MoveY == -1)
            currentAnim = walkDiagonalDownLeftAnim;
        else if (MoveX == -1 && MoveY == 1)
            currentAnim = walkDiagonalUpLeftAnim;
        else if (MoveX == 1 && MoveY == -1)
            currentAnim = walkDiagonalDownRightAnim;
        else if (MoveX == 1 && MoveY == 1)
            currentAnim = WalkDiagonalUpRightAnim;

        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
            currentAnim.Start();

        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        wasPreviouslyMoving = IsMoving;
    }
}
