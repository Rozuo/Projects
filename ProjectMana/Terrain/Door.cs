using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    private Rigidbody2D rgb;

    public bool doorCanRaise = false;

    private Vector2 doorVector = new Vector2(0, 10);

    private void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Debug.Log("door can raise " + doorCanRaise);
        if (doorCanRaise == true)
        {
            rgb.velocity = doorVector;
            StartCoroutine(timeToDisable());
        }
    }

    IEnumerator timeToDisable()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }

}
