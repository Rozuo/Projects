using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehaviour : MonoBehaviour {
    // initialize states
    public enum dragonStates
    {
        idle, walk, charge, fireAttack, Jump, homingAttack
    };

    //public enum dragonAttackStates
    //{

    //}

    private dragonStates currentState = dragonStates.idle;
    private Transform directionFacing;
    private Rigidbody2D rgb;
    public Transform maxRight;
    public Transform maxLeft;
    public GameObject fireBall;
    public GameObject homingFireBall;

    // origin point of projectiles
    public Vector2 origin = new Vector2(2f, 0.1f);
    // speed of fire balls (might add this to the actual fire balls)
    private Vector2 fireBallVelocity = new Vector2(20f, 0);

    // bools to activate a state
    private bool canCharge = true;
    private bool canFire = false;
    private bool canFireHoming = false;

    // dragons movements stats
    private float movespeed = 10f;
    private float chargeForce = 5f;
    private float jumpForce = 18f;

    // specifications of duration, recharge and rates of stated behaviors
    private float chargeStateRecharge = 12f;
    private float fireBallStateRecharge = 5f;
    private float homingFireBallStateRecharge = 10f;
    public float chargeStateRequirement = 12f;
    public float fireBallStateRequirement = 0f;
    private float fireBallRate = 0.4f;
    private float delayedFireBallRate = 0.9f;
    private float nextFireBall = 0f;
    private float nextHomingFireBall = 0f;
    private float homingFireBallRate = 1f;


    private void Awake()
    {
        // start in idle
        StartCoroutine(changeStateFromIdle());
    }

    // Use this for initialization
    void Start () {
        // aquire the current direction facing and rigidbody components
        directionFacing = GetComponent<Transform>();
        rgb = GetComponent<Rigidbody2D>();
        
        // get out of idle state
        StopCoroutine(changeStateFromIdle());
    }
	
	// Update is called once per frame
	void Update () {
        
        // stop previous coroutine from previous update loop (if it's not the first time running)
        StopCoroutine(changeState());
        
        // start a new coroutine
        StartCoroutine(changeState());
        
        // change the facing direction of the dragon
        changeDirection();

        // state manager
        switch (currentState)
        {
            case dragonStates.idle:
                idleState();
                break;
            case dragonStates.walk:
                walkState();
                break;
            case dragonStates.fireAttack:
                fireAttackState();
                break;
            case dragonStates.charge:
                chargeState();
                break;
            case dragonStates.homingAttack:
                homingFireAttackState();
                break;
                //case dragonStates.Jump:
                //    jumpState();
                //    break;

        }
        // dragon will keep on walking when in either fire ball state or homing attack state
        if (dragonStates.fireAttack == currentState || dragonStates.homingAttack == currentState)
            walkState();
        StopCoroutine(changeState());
        StartCoroutine(changeState());
    }

    // change from idle state to walk state (will only be called once)
    IEnumerator changeStateFromIdle()
    {
        yield return new WaitForSeconds(1.0f);
        if (currentState == dragonStates.idle)
            currentState = dragonStates.walk;
    }

    // state changer manager
    IEnumerator changeState()
    {
        yield return new WaitForSeconds(5.0f);
        if (canCharge) 
        {
            currentState = dragonStates.charge;
            yield return new WaitForSeconds(5.0f);
            canFire = true;
            canCharge = false;
        }
        else if (canFire)
        {
            currentState = dragonStates.fireAttack;
            yield return new WaitForSeconds(5.0f);
            canFireHoming = true;
            canFire = false;
        }
        else if (canFireHoming)
        {
            currentState = dragonStates.homingAttack;
            yield return new WaitForSeconds(6.0f);
            canCharge = true;
            canFireHoming = false;
        }
        else
            currentState = dragonStates.walk;

        //yield return new WaitForSeconds(3.0f);
        //currentState = dragonStates.walk;
    }

    // idle state behavior
    void idleState()
    {
        rgb.velocity = new Vector2(0, rgb.velocity.y);
        
    }

    // walk state behavior
    void walkState()
    {
        if (directionFacing.localScale.x > 0)
            rgb.velocity = new Vector2(movespeed, rgb.velocity.y);
        else if (directionFacing.localScale.x < 0)
            rgb.velocity = new Vector2(movespeed * -1, rgb.velocity.y);
    }

    // charge state behavior
    void chargeState()
    {
        if (directionFacing.localScale.x > 0)
        {
            rgb.AddForce(new Vector2(chargeForce, 0), ForceMode2D.Impulse);
        }
        else if (directionFacing.localScale.x < 0)
        {
            rgb.AddForce(new Vector2(chargeForce * -1, 0), ForceMode2D.Impulse);
        }
    }

    // change the direction once at specified points
    void changeDirection()
    {
        if (directionFacing.localPosition.x > maxRight.localPosition.x)
        {
            if (directionFacing.localScale.x < 0)
                return;
            else if (directionFacing.localScale.x > 0)
            {
                Vector3 scale = directionFacing.localScale;
                scale.x *= -1;
                directionFacing.localScale = scale;
            }
        }
        else if (directionFacing.localPosition.x < maxLeft.localPosition.x)
        {
            if (directionFacing.localScale.x > 0)
                return;
            else if (directionFacing.localScale.x < 0)
            {
                Vector3 scale = directionFacing.localScale;
                scale.x *= -1;
                directionFacing.localScale = scale;
            }
        }
        else
            return;
    }

    // fire attack state behavior
    private void fireAttackState()
    {   
        if (Time.time > nextFireBall)
        {
            GameObject gO = Instantiate(fireBall, (Vector2)transform.position + origin * (transform.localScale.x / (transform.localScale.x * -1)), Quaternion.identity);
            Physics2D.IgnoreCollision(gO.GetComponent<Collider2D>(), GetComponent<Collider2D>(), gO.GetComponent<Collider2D>());
            gO.GetComponent<Rigidbody2D>().velocity = new Vector2(fireBallVelocity.x * (transform.localScale.x / (transform.localScale.x * -1)), fireBallVelocity.y);
            if (transform.localScale.x < 0)
                nextFireBall = Time.time + delayedFireBallRate;
            else
                nextFireBall =  Time.time + fireBallRate;
        }
    }

    // homing fire attack state behavior
    private void homingFireAttackState()
    {
        if (Time.time > nextHomingFireBall)
        {
            nextHomingFireBall = Time.time + homingFireBallRate;
            GameObject gO = Instantiate(homingFireBall, (Vector2)transform.position + origin * (transform.localScale.x / (transform.localScale.x * -1)), Quaternion.identity);
            Physics2D.IgnoreCollision(gO.GetComponent<Collider2D>(), GetComponent<Collider2D>(), gO.GetComponent<Collider2D>());
        }
    }

    // making the homing fire ball will require transform to home in on the player while velocity will remain the same but now directed at the player

    //}
    //void jumpState()
    //{
    //    rgb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    //}
}
