using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnvironmentEffect
{
    /// <summary>
    /// Spawn effect that spawns objects. 
    /// It may also spawn objects in waves.
    /// </summary>
    /// 
    /// See TriggerEffectManager on how to use
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CreateAssetMenu(fileName = "NewSpawnEffect", menuName = "Effect/Spawn")]
    public class SpawnEffect : Effect
    {
        /// <summary>
        /// Activates the effect of spawning
        /// </summary>
        /// <param name="gO">Game object to activate spawn</param>
        /// 
        /// 2021-06-19  JH  Initial Work
        /// 2021-06-21  JH  Add null checking
        /// 
        public override void ActivateEffect(GameObject gO)
        {
            base.ActivateEffect(gO);
            SpawnEffectManager tEM = gO.GetComponentInChildren<SpawnEffectManager>();
            if (tEM != null && !tEM.isActivated)
                tEM.ActivateTrigger();
        }
    }
}
