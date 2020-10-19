using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateMachine : MonoBehaviour {
	// The states of the game.
	public enum GameStates
	{
		Start, Play, BossFight, GameOver, Win
	}

	private GameStates currentState = GameStates.Start;

	private PlayerController player;

	private EnemyScript boss;

	private Text winText;

	public AudioClip winAudio;
	private AudioSource music;
	private bool winWasPlayed = false;

	void Update()
	{
		// We check to see what state the game is in and perform a certain task.
		switch (currentState)
		{
			case GameStates.Start:
				// This will state will mean we are at the start screen and if we hit enter we will transistion to the play state.
				if (Input.GetKeyDown(KeyCode.Return))
				{
					currentState = GameStates.Play;
				}
				else if (Input.GetKeyDown(KeyCode.Escape))
				{
					Application.Quit();
				}
				break;
			case GameStates.Play:
				// We get the player here, so that we can check if they are dead or not.
				if(player == null)
				{
					player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
				}
				break;
			case GameStates.GameOver:
				// This state will only happen if the player has been confirmed to be dead.
				if (Input.GetKeyDown(KeyCode.Return))
				{
					currentState = GameStates.Start;
				}
				else if (Input.GetKeyDown(KeyCode.Escape))
				{
					Application.Quit();
				}
				break;
			case GameStates.BossFight:
				/* In our boss fight we get several objects. 
				 * We get the boss to see whether it is dead or not. 
				 * We get the player to see if they are dead or not.
				 * We get the text in case the player wins.
				 * We get the main camera to play the victory music.
				 */
				if(boss == null)
				{
					player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
					boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyScript>();
					winText = GameObject.FindGameObjectWithTag("Win").GetComponent<Text>();
					music = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
				}
				break;
			case GameStates.Win:
				// If we are in this state then the boss is dead which means we can play the victory music once.
				winText.enabled = true;
				music.loop = false;
				music.clip = winAudio;
				if (!music.isPlaying && !winWasPlayed)
				{
					music.Play();
					winWasPlayed = true;
				}
				if(!music.isPlaying && winWasPlayed)
				{
					SceneManager.LoadScene("Title Screen");
				}
				
				break;
		}

		if (player != null){
			switch (player.GetState())
			{
				case PlayerController.playerStates.dead:
					// if the player is dead then it is game over.
					currentState = GameStates.GameOver;
					player = null;
					SceneManager.LoadScene("Game Over");
					break;
			}
		}

		if (boss != null && boss.GetDead())
		{
			// if the boss is dead then the player has won.
			currentState = GameStates.Win;
		}
	}

	// Get the current state of the game.
	public GameStates GetCurrentState()
	{
		return currentState;
	}

	// Set the current state of the game.
	public void SetCurrentState(GameStates state)
	{
		currentState = state;
	}
}
