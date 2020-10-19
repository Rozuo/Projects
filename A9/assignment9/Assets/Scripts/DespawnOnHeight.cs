using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DespawnOnHeight : MonoBehaviour {

	private Transform trans;

	public float despawnHeight = -5;

	public GameObject text;
	// Use this for initialization
	void Start () {
		trans = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if(trans.position.y <= despawnHeight)
		{
			onDespawn();
		}
	}


	void onDespawn()
	{
		text.GetComponent<LevelText>().setLevel(0);
		SceneManager.LoadScene("Game Over");
	}
}
