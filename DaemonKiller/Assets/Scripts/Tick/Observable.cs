using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference CMPT 330 (2020 Fall) - 8.0 - Coroutines and Timing Notes
namespace ObserverPattern
{
    /// <summary>
    /// Observable class which observers can subscribe to for updates
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Protected Vars      Description
    /// observers           observers subscribed to the observable
    /// 
    /// Private Vars        Description
    /// inactiveObservers   observers to be unsubscribed
    /// 
    public class Observable
    {
        protected HashSet<Observer> observers;
        private HashSet<Observer> inactiveObservers;

        /// <summary>
        /// Constructor class for the observable
        /// Initializes the hashsets
        /// </summary>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public Observable()
        {
            // initialize hash sets
            observers = new HashSet<Observer>();
            inactiveObservers = new HashSet<Observer>();
        }

        /// <summary>
        /// Subscribes an observer to the observable
        /// </summary>
        /// <param name="observer">observer to subscribe to the observable</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public void Subscribe(Observer observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Unsubscribes an observer to the observable
        /// </summary>
        /// <param name="observer">observer to unsubscribe to the observable</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public void Unsubscribe(Observer observer)
        {
            inactiveObservers.Add(observer);
        }

        /// <summary>
        /// Updates the observer with data for them to use
        /// Also removes any subscribers in the observer hash set
        /// by checking the inactive observer hash set
        /// </summary>
        /// <param name="data">data for observers to be updated with</param>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public void UpdateData(object data)
        {
            if (observers != null)
            {
                if (inactiveObservers.Count > 0)
                {
                    // removes inactive observers from active list
                    // in case observer unsubscribes themselves
                    observers.ExceptWith(inactiveObservers);
                    inactiveObservers.Clear();
                }
                if (observers.Count > 0)
                {
                    foreach(Observer observer in observers)
                    {
                        observer.UpdateObserver(data);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the subscription of an observer
        /// </summary>
        /// <param name="obj">observer to check subscription for</param>
        /// <returns></returns>
        /// 
        /// 2021-06-02  JH  Initial Work
        /// 
        public bool CheckSubscription(object obj)
        {
            if (obj == null)
            {
                // case for no object is passed
                return false;
            }
            if (observers.Contains((Observer) obj))
            {
                return true;
            }
            return false;
        }
    }

}
