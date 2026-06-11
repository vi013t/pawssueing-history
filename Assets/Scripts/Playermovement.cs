using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;


    public float left = -4f;
    public float rite = 4f;
   private float moveInput = 0f;
private float stateTimer = 0f; 


    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

void Update()
{
    stateTimer -= Time.deltaTime;

    if (stateTimer <= 0)
    {
        if (moveInput == 0)
        {
            moveInput = Random.value > 0.5f ? 1f : -1f;

            stateTimer = Random.Range(0.3f, 1f);
        }
        else
        {
            moveInput = 0;

            stateTimer = Random.Range(0.3f, 1.5f);
        }
    }

    Move(Time.deltaTime);
    AnimateMovement();
}
    void AnimateMovement()
    {
        bool isWalking = moveInput != 0;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
            spriteRenderer.flipX = moveInput > 0;
    }

    void Move(float deltaTime)
    {
        Vector3 movement = new(moveInput, 0, 0);
        transform.Translate(moveSpeed * deltaTime * movement);
    }
}