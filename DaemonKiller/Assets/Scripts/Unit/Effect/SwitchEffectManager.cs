using Logic;
using ObserverPattern;
using System.Collections;
using UnityEngine;


namespace EnvironmentEffect
{
    /// <summary>
    /// Manages the switch on an effect.
    /// Time it stays toggled lasts as long as the toggleOffTime.
    /// Can also be used as a regular toggle.
    /// </summary>
    /// 
    /// Note: Attach to the object that controls the switch
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// switchGate      gate to toggle switch
    /// toggleOffTime   time to toggle switch off
    /// isActivated     toggle has been activated
    /// 
    public class SwitchEffectManager : MonoBehaviour
    {
        [Header("Requires different gate for each object")]
        public Switch switchGate;
        [Header("Time until switch turns off")]
        public float toggleOffTime;
        [HideInInspector]
        public bool isActivated = false;

        /// <summary>
        /// Initialize switch value
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        void Awake()
        {
            switchGate.SetValue(false);
        }

        /// <summary>
        /// Toggles the switch 
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public void ToggleSwitch()
        {
            if (!isActivated)
            {
                switchGate.SetValue(true);
                switchGate.UpdateOutputs();
                isActivated = true;
            }
            else
            {
                // switch toggled, turn off
                switchGate.SetValue(false);
                switchGate.UpdateOutputs();
                isActivated = false;
            }

        }

        /// <summary>
        /// Toggles the switch on that is on a timer
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public void TimedToggleOn()
        {
            if (!isActivated)
            {
                switchGate.SetValue(true);
                switchGate.UpdateOutputs();
                isActivated = true;
                StartCoroutine(ToggleOff());
            }
        }

        /// <summary>
        /// Toggles the switch off on a timer
        /// </summary>
        /// <returns>IEnumerator for coroutine</returns>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        private IEnumerator ToggleOff()
        {
            yield return new WaitForSeconds(toggleOffTime);
            switchGate.SetValue(false);
            switchGate.UpdateOutputs();
            isActivated = false;
        }
    }
}
