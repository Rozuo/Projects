using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// True class for the gate system.
    /// Always yields a true value.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CreateAssetMenu(fileName = "NewTrue", menuName = "Logic/True")]
    public class True : Gate
    {
        /// <summary>
        /// Returns the value of the gate
        /// </summary>
        /// <returns>True regardless</returns>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public override bool Value()
        {
            value = true;
            return true;
        }

        /// <summary>
        /// Prints the string, showing the gate's name and its value.
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