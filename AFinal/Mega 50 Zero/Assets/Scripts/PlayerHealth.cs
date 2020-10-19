using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour {

	private static int health;
	private int maxHealth = 50;

	private float invincibleTime = 1.5f;
	private float invincibleStartTime = -1f;

	private bool invincible = false;

	private Animator animator;
	private PlayerController player;
	private SpriteRenderer render;


	public Image healthBar;
	private Text healthText;

	// Use this for initialization
	void Start () {
		// set the health to the max HP if we have not else we will leave the current value of health as is.
		if(health <= 0)
		{
			health = maxHealth;
		}
		
		render = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		player = GetComponent<PlayerController>();
		
		// set the percentage of health.
		healthText = healthBar.GetComponentInChildren<Text>();
		healthText.text = "HP: " + ((float)health / (float)maxHealth * 100) + "%";
		healthBar.fillAmount = (float)health / (float)maxHealth; ;
	}
	
	// Update is called once per frame
	void Update () {
		/* if we are invincible we are immune to damage for a certain amount of time.
		 * Invincibility results in us starting our render coroutine allowing the player to 
		 * look like they flickering allowing the player to know when the player is invincible.
		 */
		if (invincible){
			if(invincibleStartTime == -1)
			{
				invincibleStartTime = Time.time;
			}
			//Debug.Log("Invincible");
			player.SetState(PlayerController.playerStates.invincible);
			if(invincibleTime <= Time.time - invincibleStartTime){
				player.SetState(PlayerController.playerStates.idle);
				invincible = false;
				invincibleStartTime = -1;
			}
			StopCoroutine("RenderInvincible");
			StartCoroutine("RenderInvincible");
			
		}
	}


	// Make the player flicker to show that they are invincible.
	IEnumerator RenderInvincible()
	{
		if (render.color.a == 0.25f | !invincible)
		{
			render.color = new Color(render.color.r, render.color.g, render.color.b, 1f);
			yield return new WaitForSeconds(2f);
		}
		else if (render.color.a == 1f)
		{
			render.color = new Color(render.color.r, render.color.g, render.color.b, 0.25f);
			yield return new WaitForSeconds(2f);
		}

		
	}

	/*
	 * This method is called only to do damage to the player meaning that we subtract x amount of from health. If the player goes below 0 health then we call the dead method.
	 */
	public void TakeDamage(int damage)
	{
		Debug.Log("Player took damage");
		health -= damage;
		animator.SetTrigger("TakeDamage");
		player.SetState(PlayerController.playerStates.invincible);
		invincible = true;

		float currentHpPercent = (float) health / (float) maxHealth;

		healthBar.fillAmount = currentHpPercent;
		healthText.text = "HP: " + ((currentHpPercent * 100) >= 0 ? currentHpPercent *100 : 0f) + "%";
		if(health <= 0)
		{
			Die();
		}
	}


	// This method should only be called if the players health is below 0. This will result in the player not being able to move at all.
	private void Die()
	{
		Debug.Log("Player Died");
		player.SetState(PlayerController.playerStates.dead);
		player.enabled = false;
		GetComponent<Rigidbody2D>().mass = 100;
		GetComponent<Rigidbody2D>().simulated = false;

		animator.SetBool("Dead", true);
	}

	// Get the invinciblility flag
	public bool GetInvincible()
	{
		return invincible;
	}

	// Set the invincibility flag
	public void SetInvincible(bool invinc)
	{
		invincible = invinc;
	}
}
