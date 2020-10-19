using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    public Image Bar;
    private GameObject player;

    public bool dead = false;

    public float content = 100f;
    public float maxContent = 100f;
    public float fillContent = 1f;
    public float fillRate = 0.05f;
    public int damage;
    public int enemieCollisionDamage = 5;

    // Use this for initialization
    private void Awake()
    {
        player = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update () {
        if (content<=0)
        {
            dead = true;
        }
        contentCalculation(content, fillContent, maxContent, Bar);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damaging")
        {
            if (damage == null)
            {
                Debug.Log("No damage value was set. Please check for it.");
            }
            else if (damage != null)
                content -= damage;
        }
        else if (collision.gameObject.tag == "Enemies")
        {
                content -= enemieCollisionDamage;
        }
    }

    void contentCalculation(float currentContent, float fillContent, float maxContent, Image bar)
    {
        fillContent = currentContent / maxContent;
        bar.fillAmount = fillContent;
        if (currentContent <= 0)
            bar.fillAmount = 0f;
            return;

    }

}
