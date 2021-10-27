using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool.FireParticle
{
    /// <summary>
    /// Object pool for fire particles.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// reserve         gameobject to hold all the particle effects
    /// prefab          prefab to instantiate and pool
    /// initialPoolSize initial size of pool
    /// 
    /// Private Vars    
    /// firePool        pool of fire particles
    /// 
    public class FireParticlePool : MonoBehaviour
    {
        public GameObject reserve;
        public GameObject prefab;
        public int initialPoolSize;
        private static ObjectPool firePool;

        /// <summary>
        /// Initializes the particle pool
        /// </summary>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 
        void Start()
        {
            firePool = new ObjectPool(reserve, prefab, initialPoolSize);
        }

        /// <summary>
        /// Puts the particle effect on the target game object
        /// </summary>
        /// <param name="target">target to put particle on</param>
        /// <returns>pooled object that is set</returns>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 
        public static GameObject SetToObject(GameObject target)
        {
            return firePool.SetToObject(target);
        }

        /// <summary>
        /// Returns the particle effect into the reserve pool
        /// </summary>
        /// <param name="fireParticle">particle to return to the pool</param>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 
        public static void ReturnToPool(GameObject fireParticle)
        {
            firePool.ReturnToPool(fireParticle);
        }
    }
}
