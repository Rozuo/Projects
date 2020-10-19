using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		// kill the player if they collide with the death box.
		if (col.tag.Equals("Player"))
		{
			col.GetComponent<PlayerHealth>().TakeDamage(9999999);
		}
	}
}
