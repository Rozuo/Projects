using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference CMPT 330 (2020 Fall) - 8.0 - Coroutines and Timing Notes
namespace ObserverPattern.Clock
{
    /// <summary>
    /// Class to store and determine what number of tick
    /// the clock is currently at
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// timeKey         tick number to store
    /// 
    public class ClockTick
    {
        public int timeKey;

        /// <summary>
        /// Constructor class for a clock tick
        /// </summary>
        /// <param name="tK">tick number to store</param>
        /// 
        /// 2021-06-02  JH  Iniital Work
        /// 
        public ClockTick(int tK)
        {
            timeKey = tK;
        }
    }
}
