using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ManaManager : MonoBehaviour {

    public Image Bar;
    public float content = 100;
    public float maxContent = 100f;
    public float fillContent = 1f;
    public float fillRate = 0.05f;

    private Combo spellUsed;

    // Use this for initialization
    void Start () {
        spellUsed = GetComponent<Combo>();
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("can refill mana: " + (content != maxContent && spellUsed.spellAmount == 0));
        if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && spellUsed.spellAmount <= 1)
        {
            content -= 10;
            if (content <= 0)
                content = 0;
            contentCalculation(content, fillContent, maxContent, Bar);
        }
        if (content != maxContent && spellUsed.spellAmount == 0)
        {
            Bar.fillAmount += Time.deltaTime*20;
        }
        contentCalculation(content, fillContent, maxContent, Bar);
    }

    void contentCalculation(float currentContent, float fillContent, float maxContent, Image bar)
    {
        fillContent = currentContent / maxContent;
        bar.fillAmount = fillContent;
    }
}
