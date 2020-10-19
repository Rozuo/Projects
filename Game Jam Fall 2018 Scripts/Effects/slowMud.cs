using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowMud : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            #warning require a player slowed down state
        }
    }
}
