using System.Collections;
using System.Collections.Generic;
using Unit.Info;
using UnityEngine;

// reference: https://learn.unity.com/tutorial/create-an-ability-system-with-scriptable-objects#5cf5ecededbc2a36a1bd53b7

namespace EnvironmentEffect
{
    /// <summary>
    /// Abstract scriptable object for an effect for environment triggers.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// singleUse       determines if the effect is a single use or not
    /// particlePrefab  particle to spawn on activation of effect
    /// 
    public abstract class Effect : ScriptableObject
    {
        // public AudioClip sound;
        [Header("Toggle if effect only occurs once")]
        [Tooltip("If enabled, the effect will only occur once for the object")]
        public bool singleUse = false;
        [Header("Particle on effect")]
        public GameObject particlePrefab;

        /// <summary>
        /// Activates the effect
        /// </summary>
        /// <param name="gO">GameObject that carries the effect</param>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 
        public virtual void ActivateEffect(GameObject gO)
        {
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, gO.transform.position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Gets the range of the effect
        /// </summary>
        /// <returns>default is 0, override to yield different result</returns>
        /// 
        /// 2021-06-25  JH  Initial Work
        /// 
        public virtual float GetRange()
        {
            return 0;
        }

        /// <summary>
        /// Gets the delay of the effect
        /// </summary>
        /// <returns>default is 0, override to yield different result</returns>
        /// 
        /// 2021-06-25  JH  Initial Work
        /// 
        public virtual float GetDelayTime()
        {
            return 0;
        }

        /// <summary>
        /// Determines if the effect is single use or not
        /// </summary>
        /// <returns>true if effect is single use, false otherwise</returns>
        /// 
        /// 2021-06-20  JH  Initial Work
        /// 
        public bool IsSingleUse()
        {
            return singleUse;
        }
    }
}
