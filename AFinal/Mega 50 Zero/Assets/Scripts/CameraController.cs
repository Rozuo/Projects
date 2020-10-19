using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;


	public float zOffset;
	private bool followTarget = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// This will set the cameras position to that of it's target with a zOffset.
		transform.position = new Vector3(target.position.x, target.position.y, target.position.z + zOffset);
	}


}
