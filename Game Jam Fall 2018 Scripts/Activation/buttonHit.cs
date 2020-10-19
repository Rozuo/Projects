using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonHit : MonoBehaviour {

    public bool buttonPress = false;
    public removeDoor door;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ball")
        {
            door.canRemove = true;
        }
    }
}
