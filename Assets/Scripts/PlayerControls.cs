using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    public float jumpForce = 10f;
    public float moveSpeed = 7f;
    public Transform gameCamera;
    public float cameraFollowSpeed = 5f;

    public float cameraOffset;

    private Input input;
    private Rigidbody2D rigidBody;
    private bool onGround;

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
