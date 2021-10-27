using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable
{
    /// <summary>
    /// Manages interactions with a save point
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Private Var     Description
    /// saveMan         save manager to save with
    /// 
    public class SaveInteract : Interactables
    {
        private SaveDataManager saveMan;

        /// <summary>
        /// Initialize save manager
        /// </summary>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        void Start()
        {
            GameObject saveObj = GameObject.FindGameObjectWithTag("Scene Manager");
            if (saveObj)
                saveMan = saveObj.GetComponent<SaveDataManager>();
        }

        /// <summary>
        /// Save game data to a file upon interaction
        /// </summary>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 
        public override void Interact(GameObject gO)
        {
            if (saveMan)
                saveMan.SaveData();
            else Debug.Log("Scene Manager not found");
        }
    }
}
