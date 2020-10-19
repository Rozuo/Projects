using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code based on brackeys: https://www.youtube.com/watch?v=sPiVz1k-fEs&ab_channel=Brackeys
public class PlayerCombat : MonoBehaviour {
	/* This class is solely responsible for all combat 
	 * actions of the player, such as using there sword or bow.
	 */
	private int attack = 12;

	private PlayerController.playerStates state;
	private PlayerController player;

	public GameObject arrow;

	public Transform attackPoint;
	public Transform rangeAttackPoint;
	[SerializeField]
	private float attackRange = 0.8f;
	private LayerMask enemyLayerMask;


	//[SerializeField] private bool swordDrawn = false;

	private float attackSpeed = 2.0f;
	private float nextAttackTime = 0.0f;

	private float attackSpeedRange = 1.5f;
	private float nextRangeAttackTime = 0.0f;

	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		player = GetComponent<PlayerController>();
		state = player.currentState;
		enemyLayerMask = LayerMask.GetMask("Enemy");
	}
	
	// Update is called once per frame
	void Update () {
		// if the player is dead then we cannot attack anymore.
		switch (player.GetState())
		{
			case PlayerController.playerStates.dead:
				this.enabled = false;
				break;
		}

		// The player cannot attack until there nextAttackTime preventing from attacking constantly
		if(Time.time >= nextAttackTime)
		{
			if (player.GetGrounded() && Input.GetKeyDown(KeyCode.P)) 
			{
				Attack();
				player.currentState = PlayerController.playerStates.attacking;
				nextAttackTime = Time.time + 1f / attackSpeed;
			}
		}

		// The player cannot use a ranged attack until there nextAttackTime preventing from attacking constantly
		if (Time.time >= nextRangeAttackTime)
		{
			if (player.GetGrounded() && Input.GetKeyDown(KeyCode.O))
			{
				Fire();
				nextRangeAttackTime = Time.time + 1f / attackSpeedRange;
				player.currentState = PlayerController.playerStates.attacking;
			}
		}
		
	}

	/*
	 * This method allows the player to attack by creating the circle that will check for collisions and
	 * will do damage to every enemy that is hit.
	 */
	private void Attack()
	{
		anim.SetTrigger("Attack");

		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayerMask);

		foreach(Collider2D enemy in hitEnemies)
		{
			//Debug.Log("Hit: " + enemy.name);
			enemy.GetComponent<EnemyScript>().TakeDamage(attack);
		}
		
	}

	// trigger the fire animation
	private void Fire()
	{
		anim.SetTrigger("RangeAttack");
		
	}

	//void OnDrawGizmosSelected()
	//{
	//	if (attackPoint == null)
	//		return;
	//	Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	//}

	/*
	 * This method will spawn an arrow projectile and send it towards the direction of where the player is facing when firing the bow.
	 * This method is called in an animation.
	 */
	public void SpawnArrow()
	{
		GameObject go = Instantiate(arrow);
		go.transform.position = rangeAttackPoint.position;
		Debug.Log(transform.localScale.x > 0 ? transform.localScale.x/transform.localScale.x : transform.localScale.x / transform.localScale.x * -1 );
		go.GetComponent<Projectile>().SetDirection(transform.localScale.x > 0 ? (int)(transform.localScale.x / transform.localScale.x) : (int)(transform.localScale.x / transform.localScale.x * -1));
	}
}
