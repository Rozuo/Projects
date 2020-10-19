using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIbars : MonoBehaviour {
    
    public enum BarTypes
    {
        Health, Mana
    };

    public BarTypes type;
    public Image Bar;
    public float content;
    public float maxContent = 100f;
    public float fillContent = 1f;
    public float fillRate = 0.05f;

    private Combo spellUsed;

	// Use this for initialization
	void Start () {
        content = maxContent;
	}
	
	// Update is called once per frame
	void Update () {
        if (BarTypes.Mana == type)
        {
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") && spellUsed.spellAmount<=1)
            {
                content -= 10;
                if (content <= 0)
                    content = 0;
                contentCalculation(content, fillContent, maxContent, Bar);
            }
            if (content != maxContent && spellUsed.spellAmount == 0)
            {
                Bar.fillAmount += Time.deltaTime;
            }
            //else if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    content += 10;
            //    if (content > maxContent)
            //        content = maxContent;
            //}
            contentCalculation(content, fillContent, maxContent, Bar);
        }
	    if (Input.GetKeyDown(KeyCode.E))
        {
            content -= 10;
            if (content <= 0)
                content = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            content += 10;
            if (content > maxContent)
                content = maxContent;
        }
        contentCalculation(content, fillContent, maxContent, Bar);
    }

    void contentCalculation(float currentContent, float fillContent, float maxContent, Image bar)
    {
        fillContent = currentContent / maxContent;
        bar.fillAmount = fillContent;
    }
}
