using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// code based on brackeys tutorial
// https://www.youtube.com/watch?v=4HpC--2iowE&ab_channel=Brackeys

public class ThirdPersonController : MonoBehaviour {

	public GameObject cam;

	private CharacterController controller;

	private float movementSpeed = 10f;
	private float turnSpeed;

	private float horiMove;
	private float vertMove;
	private float turnSmoothTime = 0.1f;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		moveV2();
	}

	private void moveV2()
	{
		horiMove = Input.GetAxis("Horizontal") * movementSpeed;
		vertMove = Input.GetAxis("Vertical") * movementSpeed;
		Vector3 playerMovement = new Vector3(horiMove, 0f, vertMove) * movementSpeed * Time.deltaTime;
		controller.Move(playerMovement);

		if(playerMovement != Vector3.zero)
		{
			gameObject.transform.forward = playerMovement;
		}
		//transform.Translate(playerMovement, Space.Self);
	}

	private void move()
	{
		horiMove = Input.GetAxisRaw("Horizontal") * movementSpeed;
		vertMove = Input.GetAxisRaw("Vertical") * movementSpeed;

		Vector3 direction = new Vector3(horiMove, 0f, vertMove).normalized;

		if (direction.magnitude >= 0.1f)
		{
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, turnSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
			controller.Move(direction * movementSpeed * Time.deltaTime);
		}
	}

	
}
