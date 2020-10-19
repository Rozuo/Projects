using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// camera script based on https://www.youtube.com/watch?v=7nxpDwnU0uU&ab_channel=StephenBarr
public class CameraScript : MonoBehaviour {

	public Transform target;
	public Transform player;

	private float camSpeed = 1f;

	private float maxRotationX = 30f;

	private float horizontal = 0f;
	private float vertical = 0f;

	private float currentRotation;
	// Use this for initialization
	void Start () {
		//currentRotation = transform.rotation.eulerAngles.x;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		
		
		//transform.RotateAround(target.transform.position, Vector3.up, horizontal * camSpeed);

		//if(transform.rotation.eulerAngles.x <= maxRotationX & transform.rotation.eulerAngles.x >= -maxRotationX)
		//{
            //transform.RotateAround(target.transform.position, transform.right, vertical * -camSpeed);
        //}

	}

	void LateUpdate()
	{
		cameraControl();
	}

	private void cameraControl()
	{
		horizontal += Input.GetAxis("Mouse X") * camSpeed;
		vertical -= Input.GetAxis("Mouse Y") * camSpeed;

		vertical = Mathf.Clamp(vertical, -35, 60);

		transform.LookAt(target);

		target.rotation = Quaternion.Euler(vertical, horizontal, 0);
		player.rotation = Quaternion.Euler(0, horizontal, 0);
	}
}
