using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable
{
    /// <summary>
    /// Teleport Interact is like the Door Interact, but the interaction is a one-way teleport.
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    /// 
    /// private var     desc
    /// newPosi         the position to teleport the GameObject to
    /// 
    public class TeleportInteract : Interactables
    {
        private Transform newPosi;

        /// <summary>
        /// Find the new position transform
        /// </summary>
        /// 
        /// 2021-06-01  TH  Initial Implementation
        /// 
        void Awake()
        {
            newPosi = transform.GetChild(0).GetComponent<Transform>();
        }

        /// <summary>
        /// Teleport the GameObject to the newPosi destination
        /// </summary>
        /// 
        /// 2021-06-01  TH  Initial Implementation
        /// 
        public override void Interact (GameObject gO)
        {
            if(newPosi != null)
                gO.transform.position = new Vector3(newPosi.transform.position.x, gO.transform.position.y, newPosi.transform.position.z);
        }
    }
}
