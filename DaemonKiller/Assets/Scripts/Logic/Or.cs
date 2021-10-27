using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// Or class for the gate system.
    /// Yields true if any of its inputs are true, otherwise false
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CreateAssetMenu(fileName = "NewOr", menuName = "Logic/Or")]
    public class Or : Gate
    {
        /// <summary>
        /// Returns the value of the gate
        /// </summary>
        /// <returns>true if any input is true, otherwise false</returns>
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
            // any inputs true
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i])
                    if (inputs[i].value)
                    {
                        value = true;
                        return value;
                    }
            }
            // all inputs false
            value = false;
            return value;
        }
    }
}
