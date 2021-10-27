using ObserverPattern;
using ObserverPattern.Clock;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;

namespace EnvironmentEffect
{
    /// <summary>    
    /// Manages the spawn effect by holding the variables
    /// needed to spawn upon triggering the effect.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// - Credit to Tyson Hoang (TH) who made TriggerManager which
    /// this code is heavily based on
    /// 
    /// Ensure spawner and prefabs size are the same size
    /// Prefab corresponds to the spawner number
    /// 
    /// Note: spawners = spawn location for this class
    /// 
    /// General Steps:
    /// 1. Attach SpawnEffect scriptable object to EnvironmentInfo object.
    /// 2. Put an empty game object as a child to the EnvironmentInfo object and attach 
    ///    SpawnEffectManager to it.
    ///    Note: Environment object should be set to not destroyable
    /// 3. Set settings for the spawns
    /// 
    /// Timed Spawn Steps:
    /// 1. Choose the spawn(location) size & match the prefab size
    /// 2. Create empty GameObjects for the spawn location for each object, make them this GameObject's child, 
    ///    and assign them in 'spawners' (leave as active).
    /// 3. Assign each prefab to spawn respectively.
    /// 4. (Optional) Add completionReward GameObjects to the array (can be child or not).
    /// 
    /// Timed Wave Steps:
    /// 1/2. Same as above.
    /// 3. Assign each prefab to spawn respectively.
    ///    Prefabs are an empty GameObject with child GameObjects as its spawns.
    /// 4. (Optional) Add completionReward GameObjects to the array (can be child or not).
    /// 
    /// Public Vars     Description
    /// prefabs         prefab objects to spawn
    /// spawnTimer      time until despawn
    /// timeActive      active time of the spawn
    /// activeObjects   Objects that are active and need to be interacted by the player
    ///     
    public class TimedSpawnEffectManager : SpawnEffectManager, Observer
    {
        [Header("Prefabs to spawn (match size w/ spawner)")]
        public GameObject[] prefabs;

        [Header("Active Time for Spawn in seconds")]
        public float despawnTimer;

        private float timeActive = 0;
        private List<GameObject> activeObjects = new List<GameObject>();

        /// <summary>
        /// Overlook the timed spawn
        /// </summary>
        /// 
        /// 2021-06-19  JH  Initial Work
        /// 2021-06-21  JH  Add timed wave spawns
        /// 
        void Update()
        {
            if (!isCompleted)
            {
                if (isActivated)
                {
                    int nullCount = 0;
                    switch (triggerType)
                    {
                        case TriggerType.timedSpawn:
                            foreach (GameObject gO in activeObjects)
                            {
                                if (gO == null)
                                    nullCount++;
                            }
                            if (nullCount == activeObjects.Count)
                            {
                                //Debug.Log("Spawn Complete");
                                isCompleted = true;
                                UnsubscribeToClock();

                                foreach (GameObject prize in completionReward)
                                {
                                    prize.SetActive(true);
                                }
                            }
                            break;
                        case TriggerType.timedWaveSpawn:
                            foreach (GameObject gO in activeObjects)
                            {
                                if (gO == null)
                                    nullCount++;
                            }
                            if (nullCount == activeObjects.Count)
                            {
                                // wave complete
                                if (wave < spawners.Length)
                                {
                                    // spawn next wave
                                    activeObjects.Clear();

                                    GameObject aO = Instantiate(prefabs[wave], spawners[wave].transform);
                                    for (int i = 0; i < aO.transform.childCount; i++)
                                        activeObjects.Add(aO.transform.GetChild(i).gameObject);

                                    wave++;
                                }
                                else
                                {
                                    // All waves complete
                                    //Debug.LogWarning("Waves Complete");
                                    isCompleted = true;
                                    UnsubscribeToClock();

                                    if (waveType == WaveType.enemy)
                                        GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().forceCombat = false;

                                    foreach (GameObject prize in completionReward)
                                    {
                                        prize.SetActive(true);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Activate this trigger, if the player has entered it
        /// </summary>
        /// 
        /// 2021-06-19  JH  Initial Work
        /// 2021-06-21  JH  Add timed wave spawns
        /// 
        public override void ActivateTrigger()
        {
            if (isActivated || isCompleted)
                return; // do not activate if activated or completed

            timeActive = 0;
            SubscribeToClock(); // timer starts

            //Debug.Log("Player has entered a trigger.");

            switch (triggerType)
            {
                case TriggerType.timedSpawn:
                    int index = 0;
                    if (spawners.Length > 0)
                    {
                        activeObjects.Clear();

                        // spawn the object at their location and add to active list
                        foreach (GameObject location in spawners)
                        {
                            GameObject aO = Instantiate(prefabs[index++], location.transform);
                            activeObjects.Add(aO);
                        }
                    }
                    else Debug.LogError("A trigger tried to spawn an inexistent GameObject.");
                    break;
                case TriggerType.timedWaveSpawn:
                    if (spawners.Length > 0)
                    {
                        activeObjects.Clear();

                        // spawn wave 1
                        GameObject aO = Instantiate(prefabs[wave], spawners[wave].transform);
                        for (int i = 0; i < aO.transform.childCount; i++)
                            activeObjects.Add(aO.transform.GetChild(i).gameObject);
                        if (waveType == WaveType.enemy)
                            GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().forceCombat = true;
                        

                        wave++;
                    }
                    break;
            }

            isActivated = true;
        }


        /// <summary>
        /// Subscribes this unit to the clock
        /// Only if the unit has buffs and if it is currently not subscribed
        /// </summary>
        /// 
        /// 2021-06-19  JH  Initial Work
        /// 
        public void SubscribeToClock()
        {
            if (Clock.CheckSubscription(this) == false)
            {
                Clock.Subscribe(this);
            }
        }
        /// <summary>
        /// Unsubscribes this unit to the clock
        /// Only if this unit is currently subscribed
        /// </summary>
        /// 
        /// 2021-06-19  JH  Initial Work
        /// 
        public void UnsubscribeToClock()
        {
            if (Clock.CheckSubscription(this) == true)
            {
                Clock.Unsubscribe(this);
            }
        }

        /// <summary>
        /// Actions to do upon an update from the clock
        /// Ticks the puzzle time up.
        /// Resets the puzzle if not completed on time
        /// </summary>
        /// <param name="obj">info to use from the clock</param>
        /// 
        /// 2021-06-19  JH  Initial Work
        /// 
        void Observer.UpdateObserver(object obj)
        {
            timeActive += Clock.tickTime;

            if (timeActive >= despawnTimer)
            {
                Debug.Log("Time is UP");
                UnsubscribeToClock();
                ResetSpawner();
            }

        }

        /// <summary>
        /// Resets the puzzle
        /// </summary>
        /// 
        /// 2021-06-19  JH  Initial Work
        /// 
        public void ResetSpawner()
        {
            wave = 0;
            isActivated = false;
            foreach(GameObject gO in activeObjects)
            {
                Destroy(gO);
            }
            activeObjects.Clear();
        }
    }
}