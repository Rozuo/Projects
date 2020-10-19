using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapButtonHit : MonoBehaviour {
    private enum doorAct
    {
        doorActive
    };

    private doorAct theDoor;
    public activateTrapDoor doorTrapRight;
    public activateTrapDoor doorTrapLeft;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        StartCoroutine(trapActivated());
        StopCoroutine(trapActivated());
        
	}

    IEnumerator trapActivated()
    {
        yield return new WaitForSeconds(8.0f);
        doorTrapRight.activateDoor = false;
        doorTrapLeft.activateDoor = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            doorTrapRight.activateDoor = true;
            doorTrapLeft.activateDoor = true;
        }
    }
}
