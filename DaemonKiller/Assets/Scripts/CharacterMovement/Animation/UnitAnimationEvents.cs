using System.Collections;
using System.Collections.Generic;
using Unit.Info;
using UnityEngine;

namespace CharacterMovement.Animations
{
    /// <summary>
    /// Controls unit animation events. 
    /// Activates certain triggers during animation to force 
    /// an animation interruption.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Private Vars    Description
    /// animator        animator of the unit for animations
    /// unitInfo        unitInfo of the unit to check for health
    /// 
    public class UnitAnimationEvents : MonoBehaviour
    {
        private Animator animator;
        private UnitInfo unitInfo;

        /// <summary>
        /// Initialize Animator and UnitInfo
        /// </summary>
        /// 
        /// 2021-07-05  JH  Initial Work
        /// 
        void Start()
        {
            animator = GetComponent<Animator>();
            unitInfo = GetComponent<UnitInfo>();
        }

        /// <summary>
        /// Forces death animation by activating its trigger
        /// </summary>
        /// 
        /// 2021-07-05  JH  Initial Work
        /// 
        public void ForceDeathAnimation()
        {
            if (unitInfo.currentHealth <= 0)
                animator.SetTrigger("Dead");
        }

        /// <summary>
        /// Signals the end of taking damage animation by
        /// activating its trigger
        /// </summary>
        /// 
        /// 2021-07-05  JH  Initial Work
        /// 
        public void EndOfDamagedAnimation()
        {
            animator.SetTrigger("Damage End");
        }
    }
}
