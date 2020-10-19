using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

	private float rotationRate = 20f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		transform.RotateAround(transform.position, Vector3.up, rotationRate * Time.deltaTime);

	}
}
