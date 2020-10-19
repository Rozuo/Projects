using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

	// Enemy states
	public enum EnemyStates
	{
		attacking, patroling, idle
	}

	private EnemyStates currentState = EnemyStates.idle;

	private Animator animator;

	private int health = 10;
	[SerializeField]
	private int maxHealth = 10;
	private int bumpAttack = 8;
	private int slashAttack = 12;

	private bool dead = false;

	private Rigidbody2D rb;
	//public BoxCollider2D sightRange;
	public Transform sightPoint;

	[SerializeField]
	private Vector2 sightSize = new Vector2(0.8f, 0.5f);
	public Transform attackPoint;
	private float attackRange = 0.5f;

	private float attackSpeed = 2.0f;
	private float nextAttackTime = 0.0f;
	public GameObject projectile;
	public Transform rangeAttackSpawn;


	private float movementSpeed = 10f;
	public float maxWalkRange = 5f;


	private float jumpForceY = 7.0f;
	
	private bool grounded = true;
	
	private RaycastHit2D hit;
	private BoxCollider2D box2D;
	private LayerMask floorLayer;
	private float boxOffset = 0.1f; 
	private float boxSizeOffSetX = 0.5f;

	private float nextJumpTime = 0.0f;
	private float jumpCooldown = 5.0f;

	private float leftWalkRange;
	private float rightWalkRange;

	private bool facingRight = false;
	private bool facingLeft = true;

	public bool canShoot = false;
	public bool canPatrol = false;
	public bool canJump = false;
	public bool weakenedAttackSpeed = false;

	private LayerMask playerLayer;

	// Use this for initialization
	void Start () {
		// set the health of the enemy
		health = maxHealth;
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();

		// Set the max distance we go towards the left and right.
		leftWalkRange = transform.position.x - maxWalkRange;
		rightWalkRange = transform.position.x + maxWalkRange;
		
		// Get the player layer mask so that we collide with the player
		playerLayer = LayerMask.GetMask("Player");

		box2D = GetComponent<BoxCollider2D>();

		// get the floor layer to know when we're on the ground
		floorLayer = LayerMask.GetMask("Floor");
	}
	
	// Update is called once per frame
	void Update () {

		// increase attack speed when the enemy is damaged and when we set the weakened attack speed
		if (weakenedAttackSpeed && health <= maxHealth * 0.25)
		{
			attackSpeed = 1.0f;
		}
	}

	private void FixedUpdate()
	{
		// check for the ground
		GroundCheck();

		/* if we are facing the left and pass are max left distance range then we change our direction.
		   similarly we do the same thing when going pass the right range.	
		*/
		if (facingLeft && transform.position.x <= leftWalkRange)
		{
			FlipDirection();
		}
		else if (facingRight && transform.position.x >= rightWalkRange)
		{
			FlipDirection();
		}


		// if our velocity is 0 then we are in idle states
		if (rb.velocity.x == 0)
		{
			currentState = EnemyStates.idle;
			animator.SetFloat("Speed", 0);
		}

		// if the player is in our sight then we stop to attack them. If not and we have the canPatrol functionality then we proceed to patrol.
		if(InSight())
		{
			if(Time.time >= nextAttackTime)
			{
				Attack();
				nextAttackTime = Time.time + attackSpeed;
			}
			
		}
		else if (canPatrol)
		{
			Patrol(transform.localScale.normalized);
		}

		// if we have the jump the functionality then we can jump after the nextJumpTime amount of time has passed
		if (canJump && Time.time >= nextJumpTime)
		{
			Jump();
			nextJumpTime = Time.time + jumpCooldown;
		}
		
	}

	 
	//This method is to make the enemy patrol in a direction
	private void Patrol(Vector2 direction)
	{
		rb.MovePosition(new Vector2(transform.position.x + (-direction.x * Time.deltaTime* movementSpeed), transform.position.y));
		animator.SetFloat("Speed", Mathf.Abs(direction.x));
		currentState = EnemyStates.patroling;
		
	}

	// Enemy will jump there jump force
	private void Jump()
	{
		if (grounded)
		{
			rb.AddForce(Vector2.up * jumpForceY, ForceMode2D.Impulse);
			grounded = false;
			animator.SetBool("IsJumping", true);
		}
	}

	// This method will check to see if the enemy is touching the floor layer and will set the grounded bool to true.
	private void GroundCheck()
	{
		hit = Physics2D.BoxCast(box2D.bounds.center, new Vector2(box2D.bounds.size.x - boxSizeOffSetX, box2D.bounds.size.y), 0f, Vector2.down, boxOffset, floorLayer);

		if (hit.collider != null)
		{
			grounded = true;
			animator.SetBool("IsJumping", false);
		}
	}

	// We change the direction the enemy is facing.
	private void FlipDirection()
	{
		transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		facingRight = !facingRight;
		facingLeft = !facingLeft;
	}

	// The enemy confirms whether the player is their sight line.
	private bool InSight()
	{
		Collider2D playerInSight = Physics2D.OverlapBox(sightPoint.position, sightSize, 0, playerLayer);

		if(playerInSight != null)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	// Send an attack in the direction infront of the enemy
	private void Attack()
	{
		animator.SetTrigger("Attack");
		
		Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
		if(hitPlayer != null && !hitPlayer.gameObject.GetComponent<PlayerHealth>().GetInvincible())
		{
			hitPlayer.GetComponent<PlayerHealth>().TakeDamage(slashAttack);
		}
		currentState = EnemyStates.attacking;
	}

	// Fire a projectile in the direction that the enemy is facing. We do this by instantiate a projectile and setting is position after. This called in an animation.
	public void ShootEnergy()
	{
		GameObject go = Instantiate(projectile);
		go.transform.position = rangeAttackSpawn.position;
		go.GetComponent<Projectile>().SetDirection(transform.localScale.x < 0 ? (int)(transform.localScale.x / transform.localScale.x) : (int)(transform.localScale.x / transform.localScale.x * -1));
	}

	// method will do damage to the enemy and check if it's killed
	public void TakeDamage(int damage)
    {
		animator.SetTrigger("TakeDamage");
		health -= damage;
		Die();
    }

	// This method will kill the enemy making it unable to collide with anything and disabling this script.
	private void Die()
    {
		if(health <= 0)
        {
			dead = true;
			animator.SetBool("Dead", true);
			GetComponent<Rigidbody2D>().simulated = false;
			this.enabled = false;
        }
    }


	/* 
		Get the dead boolean. From the EnemyScript class
	*/
	public bool GetDead()
	{
		return dead;
	}


	// We check to see if we collide with the player if we collide with the player then we do damage to them.
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
		{
			if(!col.gameObject.GetComponent<PlayerHealth>().GetInvincible())
				col.gameObject.GetComponent<PlayerHealth>().TakeDamage(bumpAttack);
		}
	}

	// uncomment if you would like debug the attack radius and the sight box.
	//void OnDrawGizmosSelected()
	//{
	//	if (attackPoint == null)
	//		return;

	//	Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawWireCube(sightPoint.position, sightSize);

	//	Gizmos.color = Color.blue;
	//	Gizmos.DrawRay(Vector3.right * leftWalkRange, Vector3.right * rightWalkRange);
	//}
}
