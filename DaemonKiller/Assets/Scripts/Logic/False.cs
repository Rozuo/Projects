using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// False class for the gate system.
    /// Always yields a false value.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CreateAssetMenu(fileName = "NewFalse", menuName = "Logic/False")]
    public class False : Gate
    {
        /// <summary>
        /// Returns the value of the gate
        /// </summary>
        /// <returns>False regardless</returns>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public override bool Value()
        {
            value = false;
            return value;
        }

        /// <summary>
        /// Prints the string, showing the gates name and its value.
        /// </summary>
        /// <returns>string of the gate</returns>
        /// 
        /// 2021-06-24  JH  Iniital Work
        /// 
        public override string ToString()
        {
            string temp = "Gate: " + name + "\n";
            temp += "\nValue: " + Value();
            return temp;
        }
    }
}
