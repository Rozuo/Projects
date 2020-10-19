using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crazyParralelogram : MonoBehaviour {

    public Transform maxRight;
    public Transform maxLeft;
    private Vector3 maxRightCoordinates;
    private Vector3 maxLeftCoordinates;
    public Vector3 rotationDirection = Vector3.forward;
    private Rigidbody2D rgb;

    public float moveSpeed = 10f;
    private float spinSpeed = 50f;
    public int startDirection = 1;

	// Use this for initialization
	void Start () {
        rgb = GetComponent<Rigidbody2D>();
        maxRightCoordinates = maxRight.position;
        maxLeftCoordinates = maxLeft.position;
        rgb.velocity = new Vector2(moveSpeed* startDirection, 0);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (transform.position.x >= maxRightCoordinates.x)
        {
            rgb.velocity = new Vector2(moveSpeed*-1, 0);
        }
        else if (transform.position.x <= maxLeftCoordinates.x)
        {
            rgb.velocity = new Vector2(moveSpeed , 0);
        }
        transform.Rotate(rotationDirection * spinSpeed);
	}
}
