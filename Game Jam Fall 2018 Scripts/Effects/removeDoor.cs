using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class removeDoor : MonoBehaviour {

    //public GameObject button;
    public bool canRemove = false;
	// Use this for initialization
	void Start () {
        //button = GetComponent<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (canRemove)
            gameObject.SetActive(false);
	}
}
