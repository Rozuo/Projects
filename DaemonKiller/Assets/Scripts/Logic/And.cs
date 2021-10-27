using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// And class for the Gate system.
    /// Gate is true if all inputs are true, otherwise false.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CreateAssetMenu(fileName = "NewAnd", menuName = "Logic/And")]
    public class And : Gate
    {
        /// <summary>
        /// Gets the value and returns it.
        /// Also updates its outputs.
        /// </summary>
        /// <returns>true if all inputs are true, otherwise false</returns>
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
            for (int i = 0; i < inputs.Length; i++)
            {
                // any inputs false
                if (inputs[i])
                {
                    if (!inputs[i].value)
                    {
                        value = false;
                        return value;
                    }
                }
                // any inputs null
                else
                {
                    value = false;
                    return value;
                }
            }
            // all inputs true
            value = true;
            return value;
        }
    }
}
