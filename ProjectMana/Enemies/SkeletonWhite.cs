using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonWhite : MonoBehaviour {

    // initialize states
    public enum skeletonStates
    {
        idle, patrol
    };

    //public enum dragonAttackStates
    //{

    //}

    public Transform maxRight;
    public Transform maxLeft;

    private Vector3 maxRightCoordinates;
    private Vector3 maxLeftCoodinates;

    private skeletonStates currentState = skeletonStates.idle;
    private Transform directionFacing;

    private Rigidbody2D rgb;

    private int skeletonHealth = 1;
    private float skeletonSpeed = 15f;

    private void Awake()
    {
        StartCoroutine(changeStateFromIdle());
        StopCoroutine(changeStateFromIdle());
        maxRightCoordinates = maxRight.position;
        maxLeftCoodinates = maxLeft.position;
    }

    private void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
        directionFacing = GetComponent<Transform>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case skeletonStates.idle:
                idleState();
                break;
            case skeletonStates.patrol:
                patrolState();
                break;
        }
        changeDirection();
    }

    // change from idle state to walk state (will only be called once)
    IEnumerator changeStateFromIdle()
    {
        yield return new WaitForSeconds(1.0f);
        if (currentState == skeletonStates.idle)
            currentState = skeletonStates.patrol;
    }

    void changeDirection()
    {
        if (directionFacing.position.x > maxRightCoordinates.x)
        {
            //if (directionFacing.localScale.x < 0)
            //    return;
            if (directionFacing.localScale.x > 0)
            {
                Vector3 scale = directionFacing.localScale;
                scale.x *= -1;
                directionFacing.localScale = scale;
            }
        }
        else if (directionFacing.position.x < maxLeftCoodinates.x)
        {
            //if (directionFacing.localScale.x > 0)
            //    return;
            if (directionFacing.localScale.x < 0)
            {
                Vector3 scale = directionFacing.localScale;
                scale.x *= -1;
                directionFacing.localScale = scale;
            }
        }
    }
    void idleState()
    {
        rgb.velocity = new Vector2(0, rgb.velocity.y);
    }

    void patrolState()
    {
        rgb.velocity = new Vector2(skeletonSpeed * directionFacing.localScale.x, rgb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("collided with player");
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(800f * GetComponent<Transform>().localScale.x/ GetComponent<Transform>().localScale.x*-1, 400f));
        }
        if (collision.gameObject.tag == "MagicProjectile")
        {
            Debug.Log("Collided with magic");
            Destroy(gameObject);
        }
    }
}
