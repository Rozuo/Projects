using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingDoor : MonoBehaviour {

    public Transform upperMax; // Insert an empty object (within unity) as the right max point
    public Transform lowerMax; // Insert an empty object (within unity) as the left max point

    private Rigidbody2D rgb;
    private Transform currentPosition;
    private Vector3 upperMaxCoordinates;
    private Vector3 lowerMaxCoordinates;

    public float moveSpeed = 20f;
    public int startDirection = 1;


    void Awake()
    {
        rgb = GetComponent<Rigidbody2D>();  // Get the platforms 2d rigidbody
        currentPosition = GetComponent<Transform>(); // Get the platforms transform component
        upperMaxCoordinates = upperMax.position; // Get the vector3 for positions from both max points.
        lowerMaxCoordinates = lowerMax.position;
        rgb.velocity = new Vector2(0, moveSpeed * startDirection); // Start the platform to move

    }

    void FixedUpdate()
    {
        // If the current position goes beyond either max coordinates it will change direction and move 
        // the other way.
        if (upperMaxCoordinates.y <= currentPosition.position.y)
        {
            rgb.velocity = new Vector2(0, moveSpeed * -1);
        }
        else if (lowerMaxCoordinates.y >= currentPosition.position.y)
        {
            rgb.velocity = new Vector2(0, moveSpeed);
        }
    }
}
