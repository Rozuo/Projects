using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour {

    public Transform maxRight; // Insert an empty object (within unity) as the right max point
    public Transform maxLeft; // Insert an empty object (within unity) as the left max point

    private Rigidbody2D rgb;
    private Transform currentPosition;
    private Vector3 maxRightCoordinates;
    private Vector3 maxLeftCoordinates;

    public float moveSpeed = 3f;
    public int startDirection = 1;


	void Awake () {
        rgb = GetComponent<Rigidbody2D>();  // Get the platforms 2d rigidbody
        currentPosition = GetComponent<Transform>(); // Get the platforms transform component
        maxRightCoordinates = maxRight.position; // Get the vector3 for positions from both max points.
        maxLeftCoordinates = maxLeft.position;
        rgb.velocity = new Vector2(moveSpeed*startDirection, 0); // Start the platform to move

    }

	void FixedUpdate () {
        // If the current position goes beyond either max coordinates it will change direction and move 
        // the other way.
        if (maxRightCoordinates.x <= currentPosition.position.x)
        {
            rgb.velocity = new Vector2(moveSpeed * -1, 0);
        }
        else if (maxLeftCoordinates.x >= currentPosition.position.x)
        {
            rgb.velocity = new Vector2(moveSpeed, 0);
        }
    }

}
