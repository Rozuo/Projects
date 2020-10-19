using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNGPlacement : MonoBehaviour {
    public enum typeOfBox
    {
        dummy1, dummy2, dummy3, real
    };

    public typeOfBox box;
    public Transform box1;
    public Transform box2;
    public Transform box3;

    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform pos4;
    private Vector3 cord1;
    private Vector3 cord2;
    private Vector3 cord3;
    private Vector3 cord4;

    private float val;
	// Use this for initialization
	void Start () {
        assign();
        place();
        if (box == typeOfBox.dummy1 || box == typeOfBox.dummy2 || box == typeOfBox.dummy3)
        {
            StartCoroutine(placeDummy());
            StopCoroutine(placeDummy());
        }
	}

    private void assign()
    {
        cord1 = pos1.position;
        cord2 = pos2.position;
        cord3 = pos3.position;
        cord4 = pos4.position;
    }
    IEnumerator placeDummy()
    {
        if (box == typeOfBox.dummy1)
        {
            yield return new WaitForSeconds(0.1f);
        }
        else if (box == typeOfBox.dummy2)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
            yield return new WaitForSeconds(1f);
        for (int i = 1; i<=4; i++)
        {
            if (i == 1 & (box1.position != cord1 | box2.position != cord1 | box3.position != cord1))
                transform.position = cord1;
            else if (i == 2 & (box1.position != cord2 | box2.position != cord2 | box3.position != cord2))
                transform.position = cord2;
            else if (i == 3 & (box1.position != cord3 | box2.position != cord3 | box3.position != cord3))
                transform.position = cord3;
            else if (i == 4 & (box1.position != cord4 | box2.position != cord4 | box3.position != cord4))
                transform.position = cord4;
        }
    }
    private void place()
    {
        if (box == typeOfBox.real)
        {
            val = Mathf.Floor(Random.Range(1f, 4f));
            if (val == 1 & (box1.position != cord1 | box2.position != cord1 | box3.position != cord1))
                transform.position = cord1;
            else if (val == 2 & (box1.position != cord2 | box2.position != cord2 | box3.position != cord2))
                transform.position = cord2;
            else if (val == 3 & (box1.position != cord3 | box2.position != cord3 | box3.position != cord3))
                transform.position = cord3;
            else if (val == 4 & (box1.position != cord4 | box2.position != cord4 | box3.position != cord4))
                transform.position = cord4;
        }
    }
}
