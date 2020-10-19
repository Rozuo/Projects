using UnityEngine;

public class BasicCameraBehavior : MonoBehaviour {

    public Transform target;

    public float transitionSpeed = 0.15f;
    public Vector3 offSet;
    private Vector3 speed = Vector3.zero;

    private void Update()
    {
        Vector3 intendedLocation = target.position + offSet;
        transform.position = Vector3.SmoothDamp(transform.position, intendedLocation, ref speed, transitionSpeed*Time.deltaTime);
        
    }
}
