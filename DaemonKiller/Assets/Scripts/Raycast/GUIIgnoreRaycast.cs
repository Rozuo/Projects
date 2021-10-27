using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference: https://answers.unity.com/questions/816861/46-ui-image-is-capturing-clicks-how-to-prevent.html
/// <summary>
/// Causes the pointer to ignore the UI that this Component is attached to.
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
public class GUIIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
{
    /// <summary>
    /// Determines if location is a valid location for a raycast to hit
    /// </summary>
    /// <param name="sp">screen point</param>
    /// <param name="eventCamera">event camera for raycast</param>
    /// <returns>false regardless, thus making the raycast passable</returns>
    /// 
    /// 2021-06-24  JH  Initial Work
    /// 
    bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 sp, UnityEngine.Camera eventCamera)
    {
        return false;
    }
}
