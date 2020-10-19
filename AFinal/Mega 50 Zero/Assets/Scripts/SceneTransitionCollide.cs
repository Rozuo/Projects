using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionCollide : MonoBehaviour {

	// the scene name
	public string sceneName;

	private StateMachine stateMachine;

	void Start()
	{
		stateMachine = GameObject.FindGameObjectWithTag("StateMachine").GetComponent<StateMachine>();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		// if the tag is the players then we transition to the scene
		switch (col.tag)
		{
			case "Player":
				// we load the scene into a set scene.
				stateMachine.SetCurrentState(StateMachine.GameStates.BossFight);
				SceneManager.LoadScene(sceneName);
				break;
		}
	}
}
