using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Resource(s) used for this script:
// https://www.youtube.com/watch?v=MFQhpwc6cKE&ab_channel=Brackeys
namespace Camera
{
    /// <summary>
    /// Cameras with this script can focus on specific Transforms.
    /// </summary>
    ///
    /// Author: Rozario (Ross) Beaudin (RB)
    ///         Tyson Hoang (TH)
    ///         
    /// public var      desc
    /// target          The current Transform the camera is pointing to
    ///                 Used when temporarily moving the camera somewhere else
    /// defaultTarget   The Transform to have the camera focus on
    ///                 NOTE: Assign this value in Inspector, target can be left blank
    /// focusSpeed      How fast the camera moves to position over the target
    /// 
    /// private var     desc
    /// offset          Position offset away from the target
    /// rotationOffset  Rotation offset away from 0
    /// rotationQuat    Quaternion of the rotationOffset
    ///
    public class CameraTarget : MonoBehaviour
    {
        public Transform target;
        public Transform defaultTarget;
        public float focusSpeed = 0.5f;

        private Vector3 offset = new Vector3(0, 8, -3);
        private Vector3 rotationOffset = Vector3.right * 60f;
        private Quaternion rotationQuat;

        /// <summary>
        /// Sets up the Camera to focus on the defaultTarget immediately
        /// </summary>
        /// 
        /// 2021-05-21  TH  Initial Implementation
        /// 
        void Start()
        {          
            InstantMove();
            if (target != null) // If someone assigned target instead of defaultTarget, assign values appropriately
                defaultTarget = target;
        }

        /// <summary>
        /// Move the camera towards the target, and apply offsets
        /// </summary>
        /// 
        ///             RB  Initial Implementation
        /// 2021-05-21  TH  Changed from LateUpdate() to Update() to move camera while pausing.
        ///                 Camera now uses MoveTowards to move to target over time.
        void Update()
        {
            if (target == null) // Set target to camera's default target
                target = defaultTarget;

            // TODO: may remove this null check since there's one above
            // Move camera towards target
            if (target != null) 
            {
                rotationQuat.eulerAngles = rotationOffset;
                transform.position = Vector3.MoveTowards(transform.position, target.position + offset, focusSpeed);
                transform.rotation = rotationQuat;
            }
        }

        /// <summary>
        /// Assigns this camera to a new target
        /// </summary>
        /// <param name="gO">The GameObject to focus on</param>
        /// 
        /// 2021-05-21  TH  Initial Implementation
        /// 
        public void SetTarget(GameObject gO)
        {
            target = gO.transform;
        }

        /// <summary>
        /// Jump the camera to the target (if it exists)
        /// </summary>
        /// 
        /// 2021-05-21  TH  Initial Implementation
        /// 
        public void InstantMove()
        {
            if (target != null) 
                transform.position = target.position + offset;
        }
    }
}
