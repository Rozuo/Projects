using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

	public string sceneName;

	private 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// transition scene if we hit enter.
		if (Input.GetKeyDown(KeyCode.Return))
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}
