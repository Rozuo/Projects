using ObserverPattern;
using ObserverPattern.Clock;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
    /// 
    /// General Steps:
    /// 1. Attach SpawnEffect scriptable object to EnvironmentInfo object.
    /// 2. Put an empty game object as a child to the EnvironmentInfo object and attach 
    ///    SpawnEffectManager to it.
    ///    Note: Environment object should be set to not destroyable, will add support 
    ///          'destroying' the object later.
    /// 3. Set settings for the spawns
    /// 
    /// Spawn Steps: 
    /// 1. Choose the spawn size.
    /// 2. Create GameObject(s) to spawn (can make it this object's child), set it inactive, and fill the elements.
    /// 
    /// Wave Spawn:
    /// 1. Create an empty GameObject, and put the things you want to spawn in there.
    /// 2. Set the empty GameObjects to inactive. Keep the children inside to active.
    /// 3. Add those GameObject to the array of spawners.
    /// 4. (Optional) Add prize GameObjects to completionReward for the prize for beating the wave.
    /// 
    /// Timed Spawn & Waved Timed Spawn:
    /// Use TimedSpawnEffectManager
    /// 
    /// Public Vars         Description
    /// triggerType         type of trigger to occur from effect
    /// waveType            type of wave to spawn
    /// spawners            game objects to spawn on activation
    /// cutscene            cutscene to play on activation 
    /// completionReward    reward for completing the wave
    /// 
    /// isActivated         determines if trigger occurred
    /// isCompleted         determines if trigger finished
    /// wave                current wave, index for wave spawn
    /// 
    public class SpawnEffectManager : MonoBehaviour
    {
        public enum TriggerType { spawn, waveSpawn, timedSpawn, timedWaveSpawn, cutscene }
        public enum WaveType { enemy, target }
        [Header("Timed type use TimedSpawnEffectManager")]
        public TriggerType triggerType;

        [Header("Type of objects to spawn (if applicable)")]
        public WaveType waveType;

        [Header("Groups to set active (if applicable)")]
        public GameObject[] spawners;

        [Header("Cutscene to activate (if applicable)")]
        public Playable[] cutscene;

        [Header("Reward for completing spawn (for waves)")]
        public GameObject[] completionReward;

        
        [HideInInspector] public bool isActivated = false;
        [HideInInspector] public bool isCompleted = false;
        protected int wave = 0;

        /// <summary>
        /// Prepare references
        /// </summary>
        /// 
        /// 2021-05-27  TH  Initial Implementation
        /// 
        private void Awake()
        {
            SetInactive();
        }

        /// <summary>
        /// Overlook the wave status
        /// </summary>
        /// 
        /// 2021-05-27  TH  Initial Implementation
        /// 2021-06-05  TH  Add GameState flag
        /// 2021-06-21  JH  Changed how wave indexing works.
        ///                 Add prize completion reward.
        ///
        private void Update()
        {
            if (!isCompleted)
            {
                if (triggerType == TriggerType.waveSpawn && isActivated)
                {
                    if (spawners[wave].transform.childCount < 1)
                    {
                        wave++;
                        if (wave < spawners.Length)
                        {
                            // Spawn the next wave
                            spawners[wave].SetActive(true);
                        }
                        else
                        {
                            // Waves completed
                            isCompleted = true;

                            foreach(GameObject prize in completionReward)
                            {
                                prize.SetActive(true);
                            }

                            if (waveType == WaveType.enemy)
                                GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().forceCombat = false;
                        }
                    }
                }
            }    
        }

        /// <summary>
        /// Activate this trigger, if the player has entered it
        /// </summary>
        /// 
        /// 2021-05-27  TH  Initial Implementation
        /// 2021-06-05  TH  Add GameState flag
        /// 2021-06-19  JH  Removed setting parent to inactive
        /// 
        public virtual void ActivateTrigger()
        {
            if (isActivated)
                return; // do not activate if already activated

            //Debug.Log("Player has entered a trigger.");

            switch (triggerType)
            {
                case TriggerType.spawn:
                    if (spawners.Length > 0)
                    {
                        // Activate everything in the spawners array
                        foreach (GameObject gO in spawners) gO.SetActive(true);
                    }
                    else Debug.LogError("A trigger tried to spawn an inexistent GameObject.");
                    break;
                case TriggerType.waveSpawn:
                    if (spawners.Length > 0)
                    {
                        spawners[0].SetActive(true);
                        if (waveType == WaveType.enemy)
                            GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().forceCombat = true;
                    }
                    break;
            }

            isActivated = true;
        }

        /// <summary>
        /// Set all objects in the array inactive recursively (if multiple triggers exist).
        /// </summary>
        /// 
        /// 2021-06-03  TH  Initial Implementation
        /// 
        private void SetInactive()
        {
            switch (triggerType)
            {
                case TriggerType.spawn:
                    foreach (GameObject gO in spawners)
                    {
                        if (gO.GetComponent<SpawnEffectManager>() != null)
                            // This object has a Trigger Component as well.
                            gO.GetComponent<SpawnEffectManager>().SetInactive();
                        gO.SetActive(false);
                    }
                    break;
            }
        }

    }
}
