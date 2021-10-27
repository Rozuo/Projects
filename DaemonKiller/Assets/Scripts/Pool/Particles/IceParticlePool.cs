using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool.IceParticle
{
    /// <summary>
    /// Object pool for ice particles.
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
    /// icePool         pool of ice particles
    /// 
    public class IceParticlePool : MonoBehaviour
    {
        public GameObject reserve;
        public GameObject prefab;
        public int initialPoolSize;
        private static ObjectPool icePool;

        /// <summary>
        /// Initializes the particle pool
        /// </summary>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 
        void Start()
        {
            icePool = new ObjectPool(reserve, prefab, initialPoolSize);
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
            return icePool.SetToObject(target);
        }

        /// <summary>
        /// Returns the particle effect into the reserve pool
        /// </summary>
        /// <param name="iceParticle">particle to return to the pool</param>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 
        public static void ReturnToPool(GameObject iceParticle)
        {
            icePool.ReturnToPool(iceParticle);
        }
    }
}
