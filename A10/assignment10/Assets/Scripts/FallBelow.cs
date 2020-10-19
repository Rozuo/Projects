using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBelow : MonoBehaviour {

	private float yRestriction = -5;

	private Vector3 startingPoint;

	// Use this for initialization
	void Start () {
		startingPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.y <= yRestriction)
		{
			transform.position = startingPoint;
		}
	}
}
