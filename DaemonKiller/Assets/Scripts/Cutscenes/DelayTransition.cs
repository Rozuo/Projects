using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Cutscenes
{
    /// <summary>
    /// Delay transitions sole purpose is delay the transition until the 
    /// cutscene has been completed and transition to a new level.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin
    /// 
    /// Public Variables:
    /// cutscene        Cutscene that we are waiting to finish
    /// sceneName       Name of the scene that we will be transistioning to.
    /// 
    public class DelayTransition : MonoBehaviour
    {
        public Cutscene cutscene;
        public string sceneName;

        /// <summary>
        /// Updates every frame to determine whether we can transition to a scene.
        /// </summary>
        /// 
        /// 2021-07-27 RB Initial Documentation
        /// 
        void Update()
        {
            if (cutscene.isCompleted)
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}

