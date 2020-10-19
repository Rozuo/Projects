using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakableStructure : MonoBehaviour {

    private int structHp = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (structHp <= 0)
        {
            Destroy(gameObject);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spoon")
        {
            structHp -= 1;
        }
    }
}
