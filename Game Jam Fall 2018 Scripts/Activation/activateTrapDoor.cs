using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateTrapDoor : MonoBehaviour {

    public enum doorType
    {
        rightDoor, leftDoor
    };

    public doorType doorSide;
    
    private Rigidbody2D rgb;

    public Transform maxRight;
    public Transform maxLeft;

    private Vector3 maxRightCoordinates;
    private Vector3 maxLeftCoordinates;

    public bool activateDoor = false;

    private float doorSpeed = 15f;

	// Use this for initialization
	void Start () {
        rgb = GetComponent<Rigidbody2D>();
        maxRightCoordinates = maxRight.position;
        maxLeftCoordinates = maxLeft.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (doorSide == doorType.rightDoor)
        {
            if (activateDoor)
            {
                if (transform.position.x < maxRightCoordinates.x)
                    rgb.velocity = new Vector2(doorSpeed, 0);
                else
                    rgb.velocity = new Vector2(0, 0);
            }
            else
            {
                if (transform.position.x >= maxLeftCoordinates.x)
                {
                    rgb.velocity = new Vector2(doorSpeed * -1, 0);
                }
                else
                    rgb.velocity = new Vector2(0, 0);
            }
        }
        else
        {
            if (activateDoor)
            {
                if (transform.position.x > maxLeftCoordinates.x)
                {
                    rgb.velocity = new Vector2(doorSpeed*-1, 0);
                }
                else
                { 
                    rgb.velocity = new Vector2(0, 0);
                }

            }
            else
            {
                if (transform.position.x < maxRightCoordinates.x)
                    rgb.velocity = new Vector2(doorSpeed, 0);
                else
                    rgb.velocity = new Vector2(0, 0);
            }
        }

	}
}
