using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	private LayerMask targetMask;
	public string nameOfTargetMask;

	private LayerMask omitMask;
	public string nameOfOmitMask;

	private int damage = 10;

	private float speed = 20f;

	private float lifeTime = 0.5f;
	private float startTime = 0;

	private int direction = 1; 

	private Rigidbody2D rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = Vector2.right * speed * direction;
		startTime = Time.time;
		targetMask = LayerMask.NameToLayer(nameOfTargetMask);
		omitMask = LayerMask.NameToLayer(nameOfOmitMask);
	}

	void Update()
	{
		// Destroy the projectile if it has been flying for more then it's life time.
		if(Time.time - startTime >= lifeTime)
		{
			Destroy(gameObject);
		}
	}

	// set the projectiles direction.
	public void SetDirection(int direction)
	{
		this.direction = direction;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		//Debug.Log("Collinding with " + col.gameObject.layer);
		//Debug.Log("projectile layer " + gameObject.layer);

		/* If the game objects layer is equal to the omit mask then we ignore it's collision. Else we will perform an action that is specified.
		 * If our target mask is the player then we do damage to the player. If our target mask is the enemy then we do damage to the enemy.
		 */
		if (!col.gameObject.layer.Equals(omitMask) && !LayerMask.Equals(col.gameObject.layer, gameObject.layer))
		{
			switch (nameOfTargetMask)
			{
				case ("Player"):
					if (col.gameObject.layer.Equals(targetMask))
					{
						col.GetComponent<PlayerHealth>().TakeDamage(damage);
					}
					break;
				case ("Enemy"):
					if (col.gameObject.layer.Equals(targetMask))
					{
						col.GetComponent<EnemyScript>().TakeDamage(damage);
			
					}
					break;
			}
			Destroy(gameObject);
		}
	}
}
