using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Logic
{
    // Modified from the Breakfast Game for CMPT 330
    /// <summary>
    /// Editor for Or class. It shows its name, its inputs, and their values, and its value
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    [CustomEditor(typeof(Or))]
    public class OrEditor : Editor
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
            Or orScript = (Or)target;

            GUILayout.Label(target.ToString());
        }
    }
}

