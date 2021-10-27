using Action.Info;
using System.Collections;
using System.Collections.Generic;
using Unit.Info;
using UnityEngine;

namespace Action.Info.Abilities
{
    /// <summary>
    /// Contains all information about an ability action.
    /// They require learning the ability rather than having an item.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Private Vars        Description
    /// abilityAcquired     determines if ability is acquired and can be used 
    /// 
    public class AbilityActionInfo : ActionInfo
    {
        private bool abilityAcquired = false;

        public override void SetUpAction()
        {
            base.SetUpAction();
            actionType = ActionType.Ability;
        }
        /// <summary>
        /// Getter method for ability acquired
        /// </summary>
        /// <returns>true if player got ability, false otherwise</returns>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 
        public bool GetAbilityAcquired()
        {
            return abilityAcquired;
        }
        /// <summary>
        /// Setter method for ability acquired
        /// </summary>
        /// <param name="acquired">bool to set to</param>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 
        public void SetAbilityAcquired(bool acquired)
        {
            abilityAcquired = acquired;
        }
        /// <summary>
        /// Determines if the action is available.
        /// For abilities, they only have to be acquired.
        /// </summary>
        /// <returns></returns>
        /// 
        /// 2021-05-27  JH  Initial Work
        /// 
        public override bool ActionAvailable()
        {
            if (abilityAcquired)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Activates the abilities actions. Varies
        /// </summary>
        /// <param name="target">target to activate ability on</param>
        /// 
        /// 2021-05-28  JH  Initial Work
        /// 
        public override bool CommenceAction(UnitInfo target)
        {
            if (target == null || target.currentHealth <= 0)
            {
                Debug.Log("Target is null or dead for action: " + actionName);
                return false;
            }
            return true;
        }


    }

}
