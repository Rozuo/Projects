using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectiles : MonoBehaviour {

    public enum magicType
    {
        Fire, Ice, FireIce
    };

    public magicType type;

    public int fireDamage = 10;
    public int iceDamage = 8;
    public int fireIceDamage = 20;

    public LayerMask structures;
    public LayerMask enemies;

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
        else if (rgb.IsTouchingLayers(enemies))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
            return;
        gameObject.SetActive(false);
        Destroy(gameObject, 10f);
    }



}
