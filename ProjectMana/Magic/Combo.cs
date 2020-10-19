using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : MonoBehaviour {

    public int aButtonPress = 0;
    public int bButtonPress = 0;
    public int spellAmount = 0;

    public bool fireSpell = false;
    public bool iceSpell = false;
    public bool fireIceSpell = false;
    public bool canFire = false;

    // Use this for initialization
    private void Awake()
    {
        aButtonPress = 0;
        bButtonPress = 0;
        spellAmount = 0;
        canFire = false;
    }


    void Start () {
        //fireProjectile.SetActive(false);
        //iceProjectile.SetActive(false);
        //fireIceProjectile.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1") && !canFire)
        {
            if ((aButtonPress + bButtonPress) < 3)
            {
                aButtonPress += 1;
                Debug.Log("A button was pressed and added 1 to count");
                spellAmount += 1;
            }
            else if (spellAmount >= 3)
            {
                spellAmount = 3;
                if (aButtonPress > 3)
                    aButtonPress = 3;
            }
        }
        else if (Input.GetButtonDown("Fire2") && !canFire)
        {
            if ((aButtonPress + bButtonPress) < 3)
            {
                bButtonPress += 1;
                Debug.Log("B button was pressed and added 1 to count");
                spellAmount += 1;
            }
            else if (spellAmount >= 3)
            {
                spellAmount = 3;
                if (bButtonPress > 3)
                    bButtonPress = 3;
            }
        }
        if (aButtonPress+bButtonPress == 3 && spellAmount == 3)
        {
            canFire = true;
        }
        if (aButtonPress + bButtonPress == 3)
        {
            if (aButtonPress==3)
            {
                //fireProjectile.SetActive(true);
                aButtonPress = 0;
                spellAmount = 0;
                canFire = false;
                fireSpell = true;
                iceSpell = false;
                fireIceSpell = false;
            }
            else if (bButtonPress==3)
            {
                //iceProjectile.SetActive(true);
                bButtonPress = 0;
                spellAmount = 0;
                canFire = false;
                fireSpell = false;
                iceSpell = true;
                fireIceSpell = false;
            }
            else if (aButtonPress+bButtonPress == 3)
            {
                //fireIceProjectile.SetActive(true);
                aButtonPress = 0;
                bButtonPress = 0;
                spellAmount = 0;
                canFire = false;
                fireSpell = false;
                iceSpell = false;
                fireIceSpell = true;
            }
        }
	}
}
