using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// Switch class for the Gate system.
    /// Other scripts can set the value of the switch.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CreateAssetMenu(fileName = "NewSwitch", menuName = "Logic/Switch")]
    public class Switch : Gate
    {
        /// <summary>
        /// Initialize the value and subscribes to its inputs
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        void OnEnable()
        {
            if (inputs != null)
                foreach (Gate g in inputs)
                {
                    g.outputs.Subscribe(this);
                }
            value = false;
        }


        /// <summary>
        /// Returns the value of the gate
        /// </summary>
        /// <returns>bool of whatever value is currently. If not assigned, return false</returns>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public override bool Value()
        {
            if (value)
            {
                return value;
            }    
            // value not assigned yet
            value = false;
            return value;
        }

        /// <summary>
        /// Sets the value of the gate
        /// </summary>
        /// <param name="value">bool to set value</param>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public void SetValue(bool value)
        {
            this.value = value;
        }

        /// <summary>
        /// Prints out the gate's name and its value
        /// </summary>
        /// <returns>string to print</returns>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public override string ToString()
        {
            string temp = "Gate: " + name + "\n";
            temp += "\nValue: " + Value();
            return temp;
        }
    } 
}
