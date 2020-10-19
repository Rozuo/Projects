using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {

	// All states the player can be in
	public enum playerStates
	{
		idle, moving, jumping, falling, attacking, dead, invincible
	}

	private float movementSpeed = 8f;

	private float jumpForce = 8f;

	private Animator animator;
	public UnityEvent onLandEvent;


	private bool grounded = true;

	private bool touchingWall = false;

	public playerStates currentState = playerStates.idle;

	private Rigidbody2D rb;

	private BoxCollider2D box2D;
	private RaycastHit2D hit;
	private float boxOffset = 0.1f;
	private float boxSizeOffSetX = 0.5f;

	private LayerMask floorLayer;

	private float gravityModifier = 2.0f;

	private void Awake()
	{
		// set up unity event
		if (onLandEvent == null)
			onLandEvent = new UnityEvent();
	}

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		box2D = GetComponent<BoxCollider2D>();
		floorLayer = LayerMask.GetMask("Floor");
		animator = GetComponent<Animator>();
	}


	// Update is called once per frame
	void FixedUpdate () {
		
		// call the method move to move the player if there is input
		Move();

		// if our velocity is less then 0, then we are falling
		if(rb.velocity.y < 0)
		{
			currentState = playerStates.falling;
			animator.SetBool("IsFalling", true);
			grounded = false;
		}
		// if we are grounded invoke our land method
		if (grounded)
		{
			onLandEvent.Invoke();
		}

		//Debug.Log(hit.collider.name);
		//if(hit.collider != null && hit.collider.CompareTag("Floor"))
		//{
		//	grounded = true;
		//}
		//WallCheck();
	}


	/*
	 * This method allows the player to move by using the translate method from the transform class. 
	 * It will also change the direction the player is facing by calling the facedirection method to allow
	 * the sprite and all attacks to go in the direction the player last moved to. Furthermore, we set 
	 * the player in idle if there is no input at all. As well as, preform our ground check and jump methods here.
	 */
	private void Move()
	{
		currentState = playerStates.moving;
		float horizontal = Input.GetAxis("Horizontal");

		FaceDirection(horizontal);

		Vector3 direction = new Vector3(horizontal, 0, 0).normalized * movementSpeed * Time.deltaTime;

		if (direction.magnitude >= 0.1f)
		{
			animator.SetFloat("Speed", Mathf.Abs(horizontal));
			transform.Translate(direction);
		}
		else
		{
			currentState = playerStates.idle;
			animator.SetFloat("Speed", 0);
		}

		GroundCheck();
		Jump();
	}


	/* This method will get the input axis of jump, to allow for an upward force to be created below the player. 
	 * This method will not run if we are not grounded, meaning that we are not touching the floor.
	 */
	private void Jump()
	{
		float jumpStrength = Input.GetAxis("Jump");

		
		if(grounded && jumpStrength >= 0.1f)
		{
			rb.AddForce(Vector2.up * jumpStrength * jumpForce, ForceMode2D.Impulse);
			currentState = playerStates.jumping;
			grounded = false;
			animator.SetBool("IsJumping", true);
		}
	}

	/* This method will simply check if the player is touching the floor layer. 
	 * If they are touching the ground then we are grounded and can preform actions like jumping.
	 */
	private void GroundCheck()
	{
		hit = Physics2D.BoxCast(box2D.bounds.center, new Vector2(box2D.bounds.size.x - boxSizeOffSetX, box2D.bounds.size.y), 0f, Vector2.down, boxOffset, floorLayer);
		

		if(hit.collider != null)
		{
			grounded = true;
			
		}
	}

	// This method will check if we are touching a wall allowing us to wall jump.
	private void WallCheck()
	{
		hit = Physics2D.Raycast(box2D.bounds.center, Vector2.right * transform.localScale.normalized.x, box2D.bounds.extents.x + boxOffset, floorLayer);
		Debug.DrawRay(box2D.bounds.center, Vector3.right * (box2D.bounds.extents.x + boxOffset) * transform.localScale.normalized.x, Color.cyan);
		if(hit.collider != null)
		{
			touchingWall = true;
		}
		else
		{
			touchingWall = false;
		}
	}

	// Change the direction that the player is facing based off the float direction.
	private void FaceDirection(float direction)
	{
		if(direction < 0 && transform.localScale.x > 0)
		{
			float newDirection = transform.localScale.x * -1;
			transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		}
		else if (direction > 0 && transform.localScale.x < 0)
		{
			float newDirection = transform.localScale.x * -1;
			transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		}
	}

	// change all animation involving falling or jumping to false resulting in the player to go back to anyother animation state.
	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
		animator.SetBool("IsFalling", false);
	}

	/*
		This method will get the player's current state.
	 */
	public playerStates GetState()
	{
		return currentState;
	}

	/*
		Set the player state to something else.
	 */
	public void SetState(playerStates theState)
	{
		currentState = theState;
	}

	/*
		get the grounded state from the player.
	 */
	public bool GetGrounded()
	{
		return grounded;
	}
}
