using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable
{
    /// <summary>
    /// Manages the Triggers scattered throughout the Scenes.
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    ///         Rozario (Ross) Beaudin
    /// 
    /// HOW TO USE THE WAVE SPAWNERS
    /// NOTE: Wave Spawners are not supported, and may cause issues with the save/load system!!
    /// 1. Create an empty GameObject, and put the things you want to spawn in there.
    /// 2. Set your empty GameObject to inactive. Keep the children inside to active.
    /// 3. Add the empty GameObject to the array of spawners.
    /// 
    /// public var      desc
    /// type            the type of trigger this is (see TriggerType enum for details)
    /// persistance     persistance of a spawn trigger (see TriggerPersist enum for details)
    /// spawners        holds all the things to spawn (in order)
    /// isActivated     has this trigger been touched by the player?
    /// 
    /// private var     desc
    /// isCompleted     has this trigger completed all their actions? (waveSpawn only)
    /// wave            the current index in the array of spawners
    /// gm              reference to the GameManager component
    /// 
    [RequireComponent(typeof(Collider))]
    public class TriggerManager : MonoBehaviour
    {
        /// <summary>
        /// Determines what the trigger does with its spawners when activated
        /// </summary>
        /// 
        /// Spawn:      Sets all objects to active
        /// WaveSpawn:  Sets objects active in order of the array
        /// Despawn:    Sets all objects to inactive
        /// 
        public enum TriggerType { spawn, waveSpawn, despawn }
        public TriggerType type;

        /// <summary>
        /// Enum to determine if the trigger will respawn objects upon
        /// loading a save file. 
        /// </summary>
        /// 
        /// Desist:     Do not respawn objects
        /// Persist:    Respawn objects
        /// 
        /// Author: Jacky Huynh (JH)
        /// 
        public enum TriggerPersist { desist, persist}
        public TriggerPersist persistance;

        [Header("Groups to set active (if applicable)")]
        public GameObject[] spawners;

        [HideInInspector] public bool isActivated = false;
        private bool isCompleted = false;
        private int wave = 0;
        protected GameManager gm;

        /// <summary>
        /// Prepare references and set up trigger
        /// </summary>
        /// 
        /// 2021-05-27  TH  Initial Implementation
        /// 
        private void Awake()
        {
            gm = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
            GetComponent<Collider>().isTrigger = true;
            SetInactive();
        }

        /// <summary>
        /// Overlook the wave status
        /// </summary>
        /// 
        /// 2021-05-27  TH  Initial Implementation
        /// 2021-06-05  TH  Add GameState flag
        /// 
        void Update()
        {
            if (!isCompleted)               
                if (type == TriggerType.waveSpawn && isActivated)
                {               
                    if (spawners[wave].transform.childCount < 1) // all enemies in current wave are defeated
                    {
                        if (wave < spawners.Length - 1) // spawn the next wave
                        {
                            spawners[++wave].SetActive(true);
                        }
                        else // all waves completed, end combat
                        {
                            spawners[wave].SetActive(true);
                            isCompleted = true;
                            gm.forceCombat = false;
                        }
                    }
                }
        }

        /// <summary>
        /// Activate this trigger, if the player has entered it
        /// </summary>
        /// <param name="other">The other colliding object</param>
        /// 
        /// 2021-05-27  TH  Initial Implementation
        /// 2021-06-05  TH  Add GameState flag
        /// 
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player") || isActivated)          
                return; // do not activate if the collider is not the player          

            switch (type)
            {
                case TriggerType.spawn:
                    if (spawners.Length > 0)
                    {
                        // Activate everything in the spawners array
                        foreach (GameObject gO in spawners) gO.SetActive(true);
                        gameObject.SetActive(false);
                    }
                    else Debug.LogError("A trigger tried to spawn an inexistent GameObject.");
                    break;
                case TriggerType.despawn:
                    if (spawners.Length > 0)
                    {
                        // Deactivate everything in the spawners array
                        foreach (GameObject gO in spawners) gO.SetActive(false);
                        gameObject.SetActive(false);
                    }
                    else Debug.LogError("A trigger tried to despawn an inexistent GameObject.");
                    break;
                case TriggerType.waveSpawn:
                    if (spawners.Length > 0)
                    {
                        spawners[0].SetActive(true);
                        gm.forceCombat = true;
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
        public void SetInactive()
        {
            switch (type)
            {
                case TriggerType.spawn:
                    foreach (GameObject gO in spawners)
                    {
                        if (gO.GetComponent<TriggerManager>() != null) // object has a Trigger Component as well, run recursion
                            gO.GetComponent<TriggerManager>().SetInactive(); 
                        gO.SetActive(false);
                    }
                    break;
            }
        }

        /// <summary>
        /// Retriggers the spawn to allow for 'persistance'
        /// </summary>
        /// 
        /// 2021-07-21  JH  Initial Implementation.
        /// 2021-07-22  JH  No longer respawns enemies
        /// 
        public void Retrigger()
        {
            if (isActivated && (persistance == TriggerPersist.persist))
            {
                switch (type)
                {
                    case TriggerType.spawn:
                        if (spawners.Length > 0)
                        {
                            // Activate everything in the spawners array
                            // EXCEPT for enemies
                            foreach (GameObject gO in spawners) 
                            { 
                                if (gO != null && !gO.CompareTag("Enemy"))
                                    gO.SetActive(true); 
                            }
                            gameObject.SetActive(false);
                        }
                        else Debug.LogError("A trigger tried to spawn an inexistent GameObject.");
                        break;
                    case TriggerType.despawn:
                        if (spawners.Length > 0)
                        {
                            // Deactivate everything in the spawners array
                            foreach (GameObject gO in spawners) gO.SetActive(false);
                            gameObject.SetActive(false);
                        }
                        else Debug.LogError("A trigger tried to despawn an inexistent GameObject.");
                        break;
                    case TriggerType.waveSpawn:
                        Debug.LogError("WaveSpawn cannot be retriggered");
                        break;
                }
            }
        }
    }
}
