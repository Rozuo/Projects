using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference CMPT 330 (2020 Fall) - 8.0 - Coroutines and Timing Notes
namespace ObserverPattern.Clock
{
    /// <summary>
    /// Clock class, used for updating subscribers
    /// based on ticks.
    /// Static methods are used for global access
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// timeScale       the number of ticks per second
    /// tickTime        amount of time for a tick
    /// 
    /// Private Vars    Description
    /// clock           observable clock 
    /// running         boolean to determine if clock is running
    /// 
    public class Clock : MonoBehaviour
    {
        public const float timeScale = 10.0f;
        public const float tickTime = 1.0f / timeScale;

        private static Observable clock;
        private static bool running;

        /// <summary>
        /// Initialize observable clock and start ticking
        /// </summary>
        /// 
        /// 2021-06-02  JH  Iniital Work
        /// 
        void Start()
        {
            clock = new Observable();
            running = true;

            StartCoroutine(Tick());
        }

        /// <summary>
        /// Subscribes an observer to the clock
        /// </summary>
        /// <param name="observer">observer to subscribe to the clock</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public static void Subscribe(Observer observer)
        {
            clock.Subscribe(observer);
        }

        /// <summary>
        /// Unsubscribes an observer to the clock
        /// </summary>
        /// <param name="observer">observer to unsubscribe to the clock</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public static void Unsubscribe(Observer observer)
        {
            clock.Unsubscribe(observer);
        }

        /// <summary>
        /// Stops the clock by setting the 
        /// running boolean false.
        /// Stops the coroutine
        /// </summary>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public static void StopClock()
        {
            running = false;
        }

        /// <summary>
        /// Returns the time key given the time
        /// </summary>
        /// <param name="time">time to convert to a time key</param>
        /// <returns>tick number</returns>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public static int TimeKey(float time)
        {
            return Mathf.CeilToInt(time * timeScale);
        }

        /// <summary>
        /// Returns the time key in tick form
        /// </summary>
        /// <returns>time converted to tick form</returns>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public static int TimeKey()
        {
            return TimeKey(Time.time);
        }

        /// <summary>
        /// Checks subscription of an observer
        /// </summary>
        /// <param name="obj">observer to check</param>
        /// <returns>true if observer is subscribed, false otherwise</returns>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public static bool CheckSubscription(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return clock.CheckSubscription(obj);
        }

        /// <summary>
        /// IEnumerator for the length of a tick
        /// Creates a new time key and sends this data to observers
        /// every tick length
        /// </summary>
        /// <returns>IEnumerator which waits for the tick time</returns>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        private IEnumerator Tick()
        {
            ClockTick tick = new ClockTick(0);
            while (running)
            {
                yield return new WaitForSeconds(tickTime);
                tick = new ClockTick(TimeKey());

                clock.UpdateData(tick);
            }
        }
    }
}