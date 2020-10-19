using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerAnimationController : MonoBehaviour {

	private Animator animator;

	public UnityEvent onLandEvent;
	

	private PlayerController.playerStates playerState;
	private PlayerController player;

	private void Awake()
	{
		if (onLandEvent == null)
			onLandEvent = new UnityEvent();
	}

	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerController>();
		animator = GetComponent<Animator>();
		playerState = player.GetState();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// get the players state.
		playerState = player.GetState();

		// check to see if the player is grounded.
		if (player.GetGrounded())
		{
			onLandEvent.Invoke();
		}

		// Depending on the player state we set an animation of set player state into motion.
		switch (playerState)
		{
			case (PlayerController.playerStates.idle):
				animator.SetFloat("Speed", 0);
				break;
			case (PlayerController.playerStates.moving):
				animator.SetFloat("Speed", 1.0f);
				break;
			case (PlayerController.playerStates.jumping):
				animator.SetBool("IsJumping", true);
				break;
			case (PlayerController.playerStates.falling):
				animator.SetBool("IsFalling", true);
				break;
			case (PlayerController.playerStates.attacking):
				break;
		}
			

	}

	// set the animations bools to false once we touch the ground.
	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
		animator.SetBool("IsFalling", false);
	}
}
