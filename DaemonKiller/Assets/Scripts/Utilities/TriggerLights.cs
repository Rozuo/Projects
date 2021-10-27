using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Trigger lights class is an extension of TriggerObject. It's responsible for turning on a spotlight on
    /// when the player walks into the trigger or turn it off.
    /// </summary>
    /// 
    /// Private Variables:
    /// spotLight           The spotlight of the player.
    /// playerMask          Player's layer mask.
    /// 
    /// Public Variables:
    /// diableLight         A bool to determine whether it will only turn off the spot light.
    /// 
    [RequireComponent(typeof(Collider))]
    public class TriggerLights : TriggerObject
    {
        private Light spotLight;
        private LayerMask playerMask;
        public bool disableLight = false;

        /// <summary>
        /// Init variables.
        /// </summary>
        /// 
        /// 2021-07-31 RB Initial documentation.
        /// 
        private void Start()
        {
            spotLight = retrievedObj.GetComponent<Light>();
            spotLight.intensity = 1.0f;
            spotLight.enabled = false;
            playerMask = 1 << LayerMask.NameToLayer("Player");
        }

        /// <summary>
        /// The action that turn the spot light on or off.
        /// </summary>
        /// 
        /// 2021-07-31 RB Initial Documentation.
        /// 
        protected override void TriggerAction()
        {
            if (!disableLight)
            {
                if (spotLight.enabled)
                {
                    spotLight.enabled = false;
                }
                else
                {
                    spotLight.enabled = true;
                }
            }
            else
            {
                spotLight.enabled = false;
            }
        }

        /// <summary>
        /// What happens when the player intersects with the trigger.
        /// </summary>
        /// <param name="other">Collider that entered the trigger.</param>
        /// 
        /// 2021-07-31 RB Initial Documentation.
        /// 
        private void OnTriggerEnter(Collider other)
        {
            if(1 << other.gameObject.layer == playerMask)
            {
                TriggerAction();
            }
        }
    }
}

