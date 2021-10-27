using Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EnvironmentEffect
{
    /// <summary>
    /// Activates a timed switch on activation
    /// </summary>
    /// 
    /// Note: Attach component SwitchEffectManager to the 
    ///       environment object
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CreateAssetMenu(fileName = "NewTGEffect", menuName = "Effect/Timed Switch")]
    public class TimedSwitchEffect : Effect
    {
        /// <summary>
        /// Activates the effect of the timed switch
        /// </summary>
        /// <param name="gO">game object to activate the effect</param>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public override void ActivateEffect(GameObject gO)
        {
            SwitchEffectManager timedSwitch = gO.GetComponent<SwitchEffectManager>();
            if (timedSwitch != null)
                timedSwitch.TimedToggleOn();
        }
    }
}
