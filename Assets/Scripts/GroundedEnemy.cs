using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

public class GroundedEnemy : MonoBehaviour
{
	[Header("Attacking")]
	public float visionRadius = 5f;
	public float maxChargeDistance = 3f;
	public float chargeSpeed = 1f;
	public float damage = 1f;

	[Header("Idle Patrol")]
	public float patrolRange = 5f;
	public float patrolSpeed = 0.1f;
	public HorizontalDirection? initialPatrolDirection = HorizontalDirection.Left;

	[Header("Game Objects")]
	public PlayerControls player;

	[Header("Technical")]

	[Tooltip("Threshold by which two distances are considered equal")]
	public float distanceEpsilon = 0.01f;

	private State state = State.Idle;
	private Vector3 startingPosition;
	private HorizontalDirection patrollingDirection;
	private bool onGround = false;

	void Start()
	{
		startingPosition = transform.position;
		patrollingDirection = initialPatrolDirection ?? HorizontalDirection.Left;
	}

	void Update()
	{
		if (state == State.Charging)
		{
			ContinueCharge(Time.deltaTime);
			return;
		}

		if ((state == State.Idle || state == State.Returning) && CanSeePlayer())
		{
			ChargeAtPlayer(Time.deltaTime);
			return;
		}

		if (state == State.Idle)
		{
			if (initialPatrolDirection == null)
			{
				return;
			}

			Patrol(Time.deltaTime);
			return;
		}
	}

	void CheckForFall()
	{
		if (onGround && (transform.position.y - startingPosition.y).magnitude < distanceEpsilon)
		{
			startingPosition = transform.position;
		}
	}

	void Return(float deltaTime)
	{
		var direction = Mathf.Sign((transform.position - startingPosition).x);
		transform.position += new Vector3(direction * deltaTime, 0f, 0f);

		if (transform.position.x - startingPosition.x <= distanceEpsilon)
		{
			state = State.Idle;
		}
	}

	void Patrol(float deltaTime)
	{
		transform.position += new Vector3(patrolSpeed * (int)patrollingDirection * deltaTime, 0f, 0f);

		// switch directions
		if (transform.position.x > startingPosition.x + patrolRange)
		{
			patrollingDirection = (HorizontalDirection)((int)patrollingDirection * -1);
		}
	}

	void ChargeAtPlayer(float deltaTime)
	{
		state = State.Charging;
		ContinueCharge(deltaTime);
	}

	void ContinueCharge(float deltaTime)
	{
		transform.position += new Vector3(chargeSpeed * (int)patrollingDirection * deltaTime, 0f, 0f);
		if ((transform.position - startingPosition).magnitude > maxChargeDistance)
		{
			state = State.Returning;
		}
	}

	bool CanSeePlayer
	{
		get {
			Vector2 direction = patrollingDirection == HorizontalDirection.Left ? new(-1, 0) : new(1, 0);
			float distance = (transform.position - player.transform.position).magnitude;

			if (distance > visionRadius) return false;

			direction.Normalize();
			if (Physics.Raycast(transform.position, direction, out RaycastHit hit, visionRadius))
			{
				return hit.transform == player;
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

public enum HorizontalDirection
{
	Left = -1,
	Right = 1
}