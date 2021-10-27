using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Base class for all objects that require to use specific trigger collider events.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// Public Variables:
    /// retrievedObj        The object that will have the trigger collider.
    /// objTag              Tag of the object incase the retrievedObj is NULL.
    /// 
    public class TriggerObject : MonoBehaviour
    {
        public GameObject retrievedObj;
        public string objTag;

        /// <summary>
        /// Initialize all variables that haven't been initialized.
        /// </summary>
        /// 
        /// 2021-07-31 RB Initial documentation.
        /// 
        private void Awake()
        {
            if(retrievedObj == null)
                retrievedObj = GameObject.FindGameObjectWithTag(objTag);
        }

        /// <summary>
        /// Action performed when an object enters it's trigger.
        /// </summary>
        /// 
        /// 2021-07-31 RB Initial documentation.
        /// 
        protected virtual void TriggerAction()
        {
            return;
        }
    }
}

