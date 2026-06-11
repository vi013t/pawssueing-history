using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class GroundedEnemy : MonoBehaviour
{
	[Header("Attacking")]
	public float visionRadius = 5f;
	public float maxChargeDistance = 3f;
	public float chargeSpeed = 20f;
	public int damage = 1;

	[Header("Idle Patrol")]
	public float patrolRange = 5f;
	public float patrolSpeed = 10f;
	public bool flying = false;
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
	private (Damageable, GameObject)? chargeAt = null;

	void Start()
	{
		patrollingDirection = initialPatrolDirection ?? HorizontalDirection.Left;
		rigidBody = GetComponent<Rigidbody2D>();
		startingPosition = rigidBody.position;
	}

	void FixedUpdate()
	{
		chargeAt = FindTarget();

		if (state == State.Charging)
		{
			ContinueCharge(Time.fixedDeltaTime);
		}

		else if ((state == State.Idle || state == State.Returning) && chargeAt != null)
		{
			ChargeAtPlayer(Time.fixedDeltaTime);
		}

		else if (state == State.Returning && initialPatrolDirection != null && (onGround || flying))
		{
			Return(Time.fixedDeltaTime);
		}

		else if (state == State.Idle && initialPatrolDirection != null && (onGround || flying))
		{
			Patrol(Time.fixedDeltaTime);
		}
		if(!flying) CheckForFall();
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
		patrollingDirection = (HorizontalDirection) direction;
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
		}

	}

	void ChargeAtPlayer(float deltaTime)
	{
		state = State.Charging;
		ContinueCharge(deltaTime);
	}

	void ContinueCharge(float deltaTime)
	{
		if (chargeAt == null) return;
		var direction = Mathf.Sign(chargeAt.Value.Item2.transform.position.x - rigidBody.position.x);
		rigidBody.MovePosition(rigidBody.position + new Vector2(chargeSpeed * direction * deltaTime, 0f));
		if ((rigidBody.position - startingPosition).magnitude > maxChargeDistance)
		{
			state = State.Returning;
		}
	}

	(Damageable, GameObject)? FindTarget() {
		Vector2 direction = patrollingDirection == HorizontalDirection.Left ? Vector2.left : Vector2.right;
		float distance = Vector2.Distance(transform.position, player.transform.position);
		if (distance > visionRadius)
		{ 
			return null;
		}

		var start = new Vector3(transform.position.x + (int) patrollingDirection * (this.GetComponent<BoxCollider2D>().size.x / 2f + 0.5f), transform.position.y, transform.position.z);
		//Debug.DrawRay(start, direction * visionRadius, Color.red);

		RaycastHit2D hit = Physics2D.Raycast(start, direction, visionRadius);
		if (hit.collider == null) return null;

		if (hit.transform == player.transform)
		{
			return (player, player.gameObject);
		}

		foreach (var clone in Clone.clones) {
			if (hit.transform == clone.transform)
			{
				return (clone, clone.gameObject);
			}
		}

		return null;
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }

		Damageable player = ((Damageable) collision.gameObject.GetComponent<PlayerControls>()) ?? ((Damageable) collision.gameObject.GetComponent<Clone>());
		if (player != null)
		{
			player.Damage(damage);
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