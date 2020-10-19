using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class homingFireBall : MonoBehaviour {

    private Transform target;

    private Rigidbody2D rgb;

    private Vector2 direction;

    public LayerMask player;
    public LayerMask structures;
    public LayerMask magic;

    [SerializeField]
    private float speed = 20f;
    private float rotation;
    private float rotationSpeed = 245f;
    private int homingFireBallDamage = 15;

	void Awake () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rgb = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {

        direction = (Vector2)target.position - rgb.position;  //Aquire set direction

        direction.Normalize();  // Prevent odd rotations

        rotation = Vector3.Cross(direction, transform.up).z;

        rgb.angularVelocity = -rotation * rotationSpeed;

        rgb.velocity = transform.up * speed;

	}

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (rgb.IsTouchingLayers(player))
        {
            Destroy(gameObject);
        }else if (rgb.IsTouchingLayers(structures))
        {
            Destroy(gameObject);
        }else if (rgb.IsTouchingLayers(magic))
        {
            Destroy(gameObject);
        }
    }
}
