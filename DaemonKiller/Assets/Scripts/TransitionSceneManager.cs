using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TransitionScene
{
    /// <summary>
    /// Manages how to load save data upon changing scenes.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Var      Description
    /// loadType        how to load data when transitioning scenes
    /// instance        instance of TransitionSceneManager to ensure only one instance of it and for persistance
    /// 
    /// 
    public class TransitionSceneManager : MonoBehaviour
    {
        /// <summary>
        /// Enum to determine how to load data when transitioning scenes
        /// </summary>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        public enum LoadType { NoData, FirstLevel, AllData, PlayerData }
        public LoadType loadType = LoadType.NoData;

        public static TransitionSceneManager instance;

        /// <summary>
        /// Initialize instance. If it exists, destroy it
        /// </summary>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Subscribes to the SceneManager for behaviour on load
        /// </summary>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnLoadScene;
        }

        /// <summary>
        /// Unsubscribes to the SceneManager for behaviour on load
        /// </summary>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLoadScene;
        }

        /// <summary>
        /// Method to call on loading a scene.
        /// Loads the player data based on the current LoadType.
        /// Only applies to scenes with LV in its name.
        /// </summary>
        /// <param name="scene">the scene that is loaded</param>
        /// <param name="mode">type of load type</param>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        private void OnLoadScene(Scene scene, LoadSceneMode mode)
        {
            if (!scene.name.Contains("LV") || mode != LoadSceneMode.Single)
                return;
            GameObject smObj = GameObject.FindGameObjectWithTag("Scene Manager");
            if (smObj)
            {
                SaveDataManager sm = smObj.GetComponent<SaveDataManager>();
                sm.ClearData();
                switch (loadType)
                {
                    case LoadType.AllData:
                        StartCoroutine(sm.LoadData());
                        break;
                    case LoadType.PlayerData:
                        sm.LoadPlayerData();
                        break;
                    case LoadType.NoData:
                        break;
                    case LoadType.FirstLevel:
                        Inventory inventory = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
                        inventory.giveStartingGear.Invoke();
                        break;
                    default:
                        break;
                }
            }
            else Debug.Log("Loading data not necessary");
        }
    }
}
