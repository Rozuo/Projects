using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// Editor for Switch class. It shows its name and its value
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CustomEditor(typeof(True))]
    public class TrueEditor : Editor
    {
        /// <summary>
        /// Updates the Inspector GUI
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            True trueScript = (True)target;

            GUILayout.Label(target.ToString());
        }
    }
}
