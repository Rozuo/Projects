using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelText : MonoBehaviour {

	private static int level = 0;


	// Use this for initialization
	void Start () {
		GetComponent<Text>().text = "level: " + level;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void nextLevel()
	{
		level += 1;
		GetComponent<Text>().text = "level: " + level;
	}

	public void setLevel(int levelNum)
	{
		level = levelNum;
	}
}
