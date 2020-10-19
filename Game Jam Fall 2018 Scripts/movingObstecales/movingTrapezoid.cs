using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingTrapezoid : MonoBehaviour {

    public Transform maxRight;
    public Transform maxLeft;

    private Vector3 maxRightCoordinates;
    private Vector3 maxLeftCoordinates;

    private Rigidbody2D rgb;

    public int directionHorz = 1;
    public float horzSpeed = 4f;
    private float vertSpeed = 10f;

	// Use this for initialization
	void Start () {
        rgb = GetComponent<Rigidbody2D>();
        maxRightCoordinates = maxRight.position;
        maxLeftCoordinates = maxLeft.position;
        rgb.velocity = new Vector2(horzSpeed*directionHorz, vertSpeed);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (transform.position.y >= maxRightCoordinates.y)
        {
            rgb.velocity = new Vector2(horzSpeed*directionHorz*-1, vertSpeed*-1);
        }
        else if (transform.position.y <= maxLeftCoordinates.y)
        {
            rgb.velocity = new Vector2(horzSpeed*directionHorz, vertSpeed);
        }
    }
}
