using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference CMPT 330 (2020 Fall) - 8.0 - Coroutines and Timing Notes
namespace ObserverPattern
{
    /// <summary>
    /// The observer interface to define an observer
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    public interface Observer 
    {
        /// <summary>
        /// How to update an observer when an update is called
        /// </summary>
        /// <param name="obj">data for observer</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        void UpdateObserver(object obj);
    }
}
