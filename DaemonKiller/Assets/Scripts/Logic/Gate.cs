using ObserverPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// Abstract class for the gate system with booleans
    /// </summary>
    /// 
    /// Note: Awake for ScriptableObjects is on creation, so
    ///       put inputs after making the object.
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// value           value of the gate
    /// inputs          gates to observe
    /// outputs         the gates to update upon changing values
    /// 
    public abstract class Gate : ScriptableObject, Observer
    {
        public bool value { get; set; }
        [Header("Gates to Observe")]
        public Gate[] inputs;
        [HideInInspector] public Observable outputs = new Observable();
        
        /// <summary>
        /// Initialize the gate upon creation
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
        }

        /// <summary>
        /// Gets the value and returns it
        /// </summary>
        /// <returns>true or false based on Gate</returns>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public abstract bool Value();

        /// <summary>
        /// Updates own value, its outputs and their observers
        /// </summary>
        /// <param name="obj">value to pass.</param>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        void Observer.UpdateObserver(object obj)
        {
            value = Value();
            outputs.UpdateData(value);
        }

        /// <summary>
        /// Updates all the outputs and their observers.
        /// To be used by other Components
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public void UpdateOutputs()
        {
            outputs.UpdateData(value);
        }

        /// <summary>
        /// Prints out the gate's name, inputs and their name and values, and value
        /// </summary>
        /// <returns>string to print</returns>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public override string ToString()
        {
            string temp = "Gate: " + name + "\nInputs: \n";
            if (inputs != null)
            {
                int index = 1;
                foreach (Gate g in inputs)
                {
                    if (g != null)
                        temp += index++ + ": " + g.name + ". Value: " + g.Value() + "\n";
                }

            }
            temp += "\nValue: " + Value();
            return temp;
        }
    }
}
