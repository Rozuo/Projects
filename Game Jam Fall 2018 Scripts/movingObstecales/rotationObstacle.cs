using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationObstacle : MonoBehaviour {

    public float spinSpeed = 3f;

    public Vector3 rotationDirection = Vector3.forward;
	void Start () {

	}
	
	void FixedUpdate () {
        transform.Rotate(rotationDirection*spinSpeed);
    }
}
