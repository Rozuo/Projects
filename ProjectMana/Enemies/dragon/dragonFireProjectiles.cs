using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonFireProjectiles : MonoBehaviour {

    public enum fireType
    {
        fireBall, homingFireBall
    };

    public fireType type;

    public int fireBallDamage = 10;

    public LayerMask structures;
    public LayerMask player;

    private Rigidbody2D rgb;


    private void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (rgb.IsTouchingLayers(structures))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (rgb.IsTouchingLayers(player))
        {
            gameObject.SetActive(false);
            
            Destroy(gameObject);
        }
        else
            return;
        gameObject.SetActive(false);
        Destroy(gameObject, 10f);
    }
    // if you use collider2d you can look for the players name and use it to filter collision like that
}
