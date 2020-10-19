using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class deadStateManager : MonoBehaviour {

    public GameObject deadText;
    private HealthManager dead;

	// Use this for initialization
	void Start () {
        dead = GetComponent<HealthManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (dead.dead)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            deadText.SetActive(true);
        }
	}
}
