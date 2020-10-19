using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviorActions : MonoBehaviour {

    private Transform directionFacing;
    private Rigidbody2D rgb;
    public Transform maxRight;
    public Transform maxLeft;
    public GameObject fireBall;
    public GameObject homingFireBall;

    public Vector2 origin = new Vector2(2f, 0.1f);
    [SerializeField]
    private Vector2 fireBallVelocity = new Vector2(20f, 0);

    private float movespeed = 15f;
    private float chargeForce = 10f;
    private float jumpForce = 18f;
    private int state;
    private float chargeStateRecharge = 0;
    private float fireBallStateRecharge = 5f;
    private float fireBallRate = 0.4f;
    private float nextFireBall = 0f;
    private float nextHomingFireBall = 0f;
    private float homingFireBallRate = 0.5f;
    private float homingFireBallStateRecharge = 10f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void idleState()
    {
        rgb.velocity = new Vector2(0, rgb.velocity.y);

    }


    void walkState()
    {
        if (directionFacing.localScale.x > 0)
            rgb.velocity = new Vector2(movespeed, rgb.velocity.y);
        else if (directionFacing.localScale.x < 0)
            rgb.velocity = new Vector2(movespeed * -1, rgb.velocity.y);
    }

    void chargeState()
    {

        if (directionFacing.localScale.x > 0)
            rgb.AddForce(new Vector2(chargeForce, 0), ForceMode2D.Impulse);
        else if (directionFacing.localScale.x < 0)
            rgb.AddForce(new Vector2(chargeForce * -1, 0), ForceMode2D.Impulse);
    }

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

    //IEnumerator fireDelay()
    //{
    //    yield return new WaitForSeconds(1);
    //    fireAttackState();
    //    yield return new WaitForSeconds(5);
    //}

    private void fireAttackState()
    {
        if (Time.time > nextFireBall)
        {
            nextFireBall = Time.time + fireBallRate;
            GameObject gO = Instantiate(fireBall, (Vector2)transform.position + origin * (transform.localScale.x / (transform.localScale.x * -1)), Quaternion.identity);
            gO.GetComponent<Rigidbody2D>().velocity = new Vector2(fireBallVelocity.x * (transform.localScale.x / (transform.localScale.x * -1)), fireBallVelocity.y);
        }
    }

    private void homingFireAttackState()
    {
        if (Time.time > nextHomingFireBall)
        {
            nextHomingFireBall = Time.time + homingFireBallRate;
            GameObject gO = Instantiate(homingFireBall, (Vector2)transform.position + origin * (transform.localScale.x / (transform.localScale.x * -1)), Quaternion.identity);
        }
    }

}
