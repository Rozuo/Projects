using EnvironmentEffect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EnvironmentEffect
{
    /// <summary>
    /// Damage over time (DoT) effect that stays on the object
    /// Damages any unit that is inside the target
    /// </summary>
    /// 
    /// See DoTEffectManager on how to use
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// tickRate        time for a tick in seconds
    /// damage          damage to apply per tick
    /// activeTime      amount of time to be active after activated
    /// 
    [CreateAssetMenu(fileName = "newDoTEffect", menuName = "Effect/DoT")]
    public class DoTEffect : Effect
    {
        [Header("Time for a tick")]
        public float tickRate = 0.5f;
        [Header("Damage per tick")]
        public float damage = 2f;
        [Header("Active Time for DoT Effect")]
        public float activeTime = 0.0f;

        /// <summary>
        /// Starts the damage over time effect
        /// </summary>
        /// <param name="gO">game object with the effect</param>
        /// 
        /// 2021-06-19  JH  Initial Work
        /// 2021-07-21  JH  ActivateTick now takes activeTime
        /// 
        public override void ActivateEffect(GameObject gO)
        {
            base.ActivateEffect(gO);
            DoTEffectManager DoTEM = gO.GetComponent<DoTEffectManager>();
            if (DoTEM != null)
            {
                DoTEM.enabled = true;
                DoTEM.tickRate = tickRate;
                DoTEM.damagePerTick = damage;
                DoTEM.ActivateTick(activeTime);
            }
            else
                Debug.Log("Must attach DoTEffectManager to this object.");
        }
    }
}
