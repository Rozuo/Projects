using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// Not class for the Gate system.
    /// Only first input is considered.
    /// If the first input is false, returns true, otherwise false
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CreateAssetMenu(fileName = "NewNot", menuName = "Logic/Not")]
    public class Not : Gate
    {
        /// <summary>
        /// Returns the value of the gate
        /// </summary>
        /// <returns>true if first input is false, otherwise false</returns>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public override bool Value()
        {
            // no inputs
            if (inputs == null || inputs.Length == 0)
            {
                value = false;
                return value;
            }
            switch (inputs[0].value)
            {
                // first input true
                case true:
                    value = false;
                    return value;
                // first input false
                case false:
                    value = true;
                    return true;
                default: break;
            }
            // unity thinks this is reachable - value is null somehow -> no input -> false
            value = false;
            return value;
        }
        
        /// <summary>
        /// Prints the string, showing the gates name, its inputs and their values, and its value.
        /// </summary>
        /// <returns>string of the gate</returns>
        /// 
        /// 2021-06-24  JH  Iniital Work
        /// 
        public override string ToString()
        {
            string temp = "Gate: " + name + "\nInputs: \n";
            if (inputs != null && inputs.Length > 0)
            {
                temp += inputs[0].name + ". Value: " + inputs[0].Value() + "\n";
            }
            temp += "\nValue: " + Value();
            return temp;
        }
    }
}