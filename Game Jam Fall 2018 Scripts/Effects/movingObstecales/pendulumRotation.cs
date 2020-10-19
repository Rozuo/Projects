using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pendulumRotation : MonoBehaviour {

    public Transform hinge;

    private Transform pendulum;

    public Vector3 direction =new Vector3 (0,0,1);


    public int maxRotation = 75;
	// Use this for initialization
	void Start () {
        pendulum = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Debug.Log(transform.rotation.z*180);
        if (pendulum.rotation.z*180 >= maxRotation)
            direction = Vector3.back;
        else if (pendulum.rotation.z*180 <= -maxRotation)
            direction = Vector3.forward;
        transform.RotateAround(hinge.position, direction, 50 * Time.deltaTime);
    }
}
