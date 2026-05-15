using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerControls : MonoBehaviour
{

    public float jumpForce = 10f;
    public float moveSpeed = 7f;
    public Transform gameCamera;
    public float cameraFollowSpeed = 5f;
    public Clone clonePrefab;
    public float recordTime = 5f;

    public float cameraOffset;

    private Input input;
    private Rigidbody2D rigidBody;
    private bool onGround;

    private Queue<Vector2> recordedPositions = new();
    private float recordingTime = 0f;
    private Vector2 recordingPosition;

    void Awake()
    {
        input = new Input();
        rigidBody = GetComponent<Rigidbody2D>();
        cameraOffset += transform.position.y;
    }

    void Update()
    {
        Vector2 moveInput = input.Player.Move.ReadValue<Vector2>();
        Vector3 movement = new(moveInput.x, 0, 0);
        transform.Translate(moveSpeed * Time.deltaTime * movement);

        if (input.Player.Jump.triggered && onGround)
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            onGround = false;
        }

        if (input.Player.Record.triggered && recordingTime == 0)
        {
            Debug.Log("recording uwu");
            recordingTime = recordTime;
            recordingPosition = new Vector2(transform.position.x, transform.position.y);
        }
    }

    void FixedUpdate()
    {
        if (recordingTime > 0) {
            var current = new Vector2(transform.position.x, transform.position.y);
            recordedPositions.Enqueue(current);

            recordingTime = Mathf.Max(recordingTime - Time.fixedDeltaTime, 0);

            if (recordingTime == 0)
            {
                Debug.Log("done recording owo");
                SpawnClone();
            }
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

    void LateUpdate()
    {
        Vector3 targetPosition = new(
            transform.position.x,
            gameCamera.position.y,
            gameCamera.position.z
        );

        if(transform.position.y+cameraOffset < gameCamera.position.y)
        {
            targetPosition = new(transform.position.x,
                                transform.position.y,
                                gameCamera.position.z);
        }

        gameCamera.position = Vector3.Lerp(
            gameCamera.position,
            targetPosition,
            cameraFollowSpeed * Time.deltaTime
        );
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }
}
