using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour, Damageable
{
    public int maxHealth;
    [Header("Movement")]
    public float jumpForce = 10f;
    public float dashForce = 10f;
    public float moveSpeed = 7f;
    public float maxStamina = 100f;
    public float dashStaminaCost = 30f;
    public float jumpStaminaCost = 10f;
    public float passiveStaminaRegeneration = 5f;
    public float sprintStaminaDepletion = 5f;
    public float sprintMultiplier = 1.4f;
    public float staminaRegenerationDelay = 3f;

    [Header("Camera")]
    public Transform gameCamera;
    public float cameraFollowSpeed = 5f;
    public float cameraOffset = 0;

    [Header("Cloning")]
    public Clone clonePrefab;
    public float recordTime = 5f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Input input;
    private Rigidbody2D rigidBody;

    private Queue<Vector2> recordedPositions = new();
    private float recordingTime = 0f;
    private Vector2 recordingPosition;

    private bool onGround;
    private bool isSprinting = false;
    private float stamina; 
    private float timeSinceUsedStamina;
    public int Health { get; set; }

    void Awake()
    {
        input = new Input();
        rigidBody = GetComponent<Rigidbody2D>();
        cameraOffset += transform.position.y;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stamina = maxStamina;
        timeSinceUsedStamina = staminaRegenerationDelay;
        Health = maxHealth;
    }

    void Update()
    {
        Move(Time.deltaTime);
        AnimateMovement();
        CheckForRecordingStart();
    }

    void FixedUpdate()
    {
        CheckForRecordingEnd(Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        CameraFollowPlayer();
    }

    void CameraFollowPlayer()
    {
        Vector3 targetPosition = new(
            transform.position.x,
            gameCamera.position.y,
            gameCamera.position.z
        );

        if (transform.position.y + cameraOffset != gameCamera.position.y)
        {
            targetPosition = new(
                transform.position.x,
                transform.position.y,
                gameCamera.position.z
            );
        }

        gameCamera.position = Vector3.Lerp(
            gameCamera.position,
            targetPosition,
            cameraFollowSpeed * Time.deltaTime
        );
    }

    void CheckForRecordingEnd(float deltaTime)
    {
        if (recordingTime > 0) {
            var current = new Vector2(transform.position.x, transform.position.y);
            recordedPositions.Enqueue(current);

            recordingTime = Mathf.Max(recordingTime - deltaTime, 0);

            if (recordingTime == 0)
            {
                Debug.Log("done recording owo");
                SpawnClone();
            }
        }
    }

    void CheckForRecordingStart()
    {
        if (input.Player.Record.triggered && recordingTime == 0)
        {
            Debug.Log("recording uwu");
            recordingTime = recordTime;
            recordingPosition = new(transform.position.x, transform.position.y);
        }
    }

    void CheckForJump()
    {
        if (input.Player.Jump.triggered && onGround && stamina >= jumpStaminaCost)
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            onGround = false;
            stamina -= jumpStaminaCost;
        }
    }

    void AnimateMovement()
    {
        Vector2 moveInput = input.Player.Move.ReadValue<Vector2>();
        bool isWalking = moveInput.x != 0;
        animator.SetBool("isWalking", isWalking);
        if (isWalking) spriteRenderer.flipX = moveInput.x > 0;
    }

    void Move(float deltaTime)
    {
        Vector2 moveInput = input.Player.Move.ReadValue<Vector2>();
        Vector3 movement = new(moveInput.x, 0, 0);

        if (input.Player.Sprint.triggered) isSprinting = !isSprinting;
        var sprinting = isSprinting && stamina > sprintStaminaDepletion;

        var speed = moveSpeed * (isSprinting ? sprintMultiplier : 1);
        transform.Translate(speed * deltaTime * movement);

        var previousStamina = stamina;
        if (isSprinting) stamina -= sprintStaminaDepletion * deltaTime;

        CheckForJump();
        CheckForDash();

        if (stamina == previousStamina) {
            if (timeSinceUsedStamina < staminaRegenerationDelay) timeSinceUsedStamina += deltaTime;
            else stamina += passiveStaminaRegeneration * deltaTime;
        }
    }

    void CheckForDash()
    {
        if (input.Player.Dash.triggered && stamina >= dashStaminaCost)
        {
            var direction = spriteRenderer.flipX ? Vector3.right : Vector3.left;
            rigidBody.AddForce(direction * dashForce, ForceMode2D.Impulse);
            stamina -= dashStaminaCost;
        }
    }

    private void SpawnClone()
    {
        var clone = Instantiate(clonePrefab);
        clone.transform.position = recordingPosition;

        var deltas = new Queue<Vector2>(); 
        Vector2? previous = null;
        foreach(var currentPosition in recordedPositions)
        {
            var previousPosition = previous ?? recordingPosition;
            var deltaMovement = currentPosition - previousPosition;
            deltas.Enqueue(deltaMovement);
            previous = currentPosition;
        }

        clone.movements = new Queue<Vector2>(deltas);
        recordedPositions = new();
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Clone"))
        {
            onGround = true;
        }
    }

    public void Damage(int damage)
    {
        Health-=damage;
        if(Health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };
    }
}
