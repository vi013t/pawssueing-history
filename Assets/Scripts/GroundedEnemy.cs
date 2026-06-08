using UnityEngine;
using System.Collections.Generic;

public class GroundedEnemy : MonoBehaviour
{
	[Header("Attacking")]
	public float visionRadius = 5f;
	public float maxChargeDistance = 3f;
	public float chargeSpeed = 20f;
	public float damage = 1f;

	[Header("Idle Patrol")]
	public float patrolRange = 5f;
	public float patrolSpeed = 10f;
	public HorizontalDirection? initialPatrolDirection = HorizontalDirection.Left;

	[Header("Game Objects")]
	public PlayerControls player;

	[Header("Technical")]

	[Tooltip("Threshold by which two distances are considered equal")]
	public float distanceEpsilon = 0.01f;

	private State state = State.Idle;
	private Vector2 startingPosition;
	private HorizontalDirection patrollingDirection;
	private bool onGround = false;
	private Rigidbody2D rigidBody;

	void Start()
	{
		patrollingDirection = initialPatrolDirection ?? HorizontalDirection.Left;
		rigidBody = GetComponent<Rigidbody2D>();
		startingPosition = rigidBody.position;
	}

	void FixedUpdate()
	{
		Debug.Log(CanSeePlayer);
		if (state == State.Charging)
		{
			ContinueCharge(Time.fixedDeltaTime);
		}

		else if ((state == State.Idle || state == State.Returning) && CanSeePlayer)
		{
			ChargeAtPlayer(Time.fixedDeltaTime);
		}

		else if (state == State.Returning && initialPatrolDirection != null && onGround)
		{
			Return(Time.fixedDeltaTime);
		}

		else if (state == State.Idle && initialPatrolDirection != null && onGround)
		{
			Patrol(Time.fixedDeltaTime);
		}

		CheckForFall();
	}

	void CheckForFall()
	{
		if (onGround && rigidBody.position.y - startingPosition.y > distanceEpsilon)
		{
			startingPosition = rigidBody.position;
		}
	}

	void Return(float deltaTime)
	{
		var direction = Mathf.Sign((startingPosition - rigidBody.position).x);
		rigidBody.MovePosition(rigidBody.position + new Vector2(direction * deltaTime, 0f));

		if (Mathf.Abs(rigidBody.position.x - startingPosition.x) <= distanceEpsilon)
		{
			state = State.Idle;
		}
	}

	void Patrol(float deltaTime)
	{
		rigidBody.MovePosition(rigidBody.position + new Vector2(patrolSpeed * (int)patrollingDirection * deltaTime, 0f));

		// switch directions
		if (
			(patrollingDirection == HorizontalDirection.Left && startingPosition.x - rigidBody.position.x >= patrolRange) ||
			(patrollingDirection == HorizontalDirection.Right && rigidBody.position.x - startingPosition.x >= patrolRange)
		)
		{
			patrollingDirection = (HorizontalDirection)((int)patrollingDirection * -1);
			Debug.Log($"Switching to {patrollingDirection}");
		}

	}

	void ChargeAtPlayer(float deltaTime)
	{
		state = State.Charging;
		ContinueCharge(deltaTime);
	}

	void ContinueCharge(float deltaTime)
	{
		rigidBody.MovePosition(rigidBody.position + new Vector2(chargeSpeed * (int)patrollingDirection * deltaTime, 0f));
		if ((rigidBody.position - startingPosition).magnitude > maxChargeDistance)
		{
			state = State.Returning;
		}
	}

	bool CanSeePlayer
	{
		get {
			Vector2 direction = patrollingDirection == HorizontalDirection.Left ? Vector2.left : Vector2.right;
			float distance = Vector2.Distance(transform.position, player.transform.position);
			Debug.DrawRay(transform.position, direction * visionRadius, Color.red);

			if (distance > visionRadius) return false;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionRadius);
			if (hit.collider != null)
			{
				return hit.transform == player.transform;
			}

			return false;
		}
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Clone"))
        {
            onGround = true;
        }
    }

	public enum State
	{
		Charging,
		Idle,
		Returning
	}
}

[System.Serializable]
public enum HorizontalDirection
{
	Left = -1,
	Right = 1
}