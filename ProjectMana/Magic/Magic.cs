using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour {

    Transform originPoint;
    public LayerMask Enemies;
    public GameObject fireMagic;
    public GameObject iceMagic;
    public GameObject fireIceMagic;

    private int aButton;
    private int bButton;

    // Spell check for specific spells
    public bool castFire;
    public bool castIce;
    public bool castFireIce;

    public bool canFire = false;

    //private Rigidbody2D rgb;

    public Vector2 fireVelocity = new Vector2(25f, 0);
    public Vector2 iceVelocity = new Vector2(23f, 0);
    public Vector2 fireIceVelocity = new Vector2(30f, 0);

    public Vector2 origin = new Vector2(2.0f, 0.1f);

    private void Awake()
    {
        originPoint = transform.Find("originPoint");
        if (originPoint == (null))
            Debug.LogError("Error! No origin point for magic projectiles to spawn.");
        aButton = GetComponent<Combo>().aButtonPress;
        bButton = GetComponent<Combo>().bButtonPress;
    }

    private void Start()
    {
        spellCheck();
    }


    private void Update()
    {
        spellCheck();
        if (castFire && (Input.GetButtonDown("Fire3")))
        {
            //canFire = true;
            castMagic(fireMagic, fireVelocity);
            resetCombo(aButton, bButton);
            castFire = false;
            spellReset();
            //GameObject tempMagicManager;
            //tempMagicManager = Instantiate(magic, magic_Spawner.transform.position) as GameObject;
        }
        else if (castIce && (Input.GetButtonDown("Fire3")))
        {
            //canFire = true;
            castMagic(iceMagic, iceVelocity);
            resetCombo(aButton, bButton);
            castIce = false;
            spellReset();
        }
        else if (castFireIce && (Input.GetButtonDown("Fire3")))
        {
            //canFire = true;
            castMagic(fireIceMagic, fireIceVelocity);
            resetCombo(aButton, bButton);
            castFireIce = false;
            spellReset();
        }

    }

    private void castMagic(GameObject projectile, Vector2 velocity)
    {
        //Vector2 originPointPosition = new Vector2(originPoint.position.x, originPoint.position.y);
        //RaycastHit2D collide = Physics2D.Raycast(originPointPosition, originPointPosition, 80, Enemies);
        //Debug.DrawLine(originPointPosition, new Vector2(80, originPointPosition.y));
        //if (collide.collider != null)
        //    Debug.DrawLine(originPointPosition, collide.point, Color.red);
        Debug.Log("can fire?" + canFire);
        GameObject gO = (GameObject)Instantiate(projectile, (Vector2)transform.position + origin * transform.localScale.x, Quaternion.identity);
        gO.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);

    }

    private void spellCheck()
    {
        castFire = GetComponent<Combo>().fireSpell;
        castIce = GetComponent<Combo>().iceSpell;
        castFireIce = GetComponent<Combo>().fireIceSpell;
        //if (castFire || castIce || castFireIce)
        //    canFire = true;
        //else
        //    canFire = false;
    }

    private void spellReset()
    {
        GetComponent<Combo>().fireSpell = false;
        GetComponent<Combo>().iceSpell = false;
        GetComponent<Combo>().fireIceSpell = false;
    }

    private void resetCombo(int abutton, int bbutton)
    {
        GetComponent<Combo>().aButtonPress = abutton;
        GetComponent<Combo>().bButtonPress = bbutton;
    }

}
