using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour {

    private bool canOpen = false;

	void Update () {
        GetComponentInParent<Door>().doorCanRaise = canOpen;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canOpen = true;
        }
    }
}
