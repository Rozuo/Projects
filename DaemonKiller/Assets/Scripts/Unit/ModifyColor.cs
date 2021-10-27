using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit.Info;
using UnityEngine.UI;

namespace Unit.ColorChange
{
    /// <summary>
    /// Class to cover color changes of materials on units
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    public class ModifyColor : MonoBehaviour
    {
        /// <summary>
        /// Changes color of a unit to the desired color
        /// </summary>
        /// <param name="unitInfo">UnitInfo of a gameobject</param>
        /// <param name="color">color to change material to</param>
        /// 
        /// 2021-05-06  JH  Initial Work
        /// 
        public static void ChangeUnitColor(UnitInfo unitInfo, Color color)
        {
            unitInfo.rend.material.color = color;
        }
        /// <summary>
        /// Changes alpha of an image to the desired transparency
        /// </summary>
        /// <param name="img">image to modify</param>
        /// <param name="alpha">amount of transparency, 1 = opaque, 0 = transparent</param>
        /// 
        /// 2021-06-08  JH  Initial Work
        /// 
        public static void ChangeImageAlpha(Image img, float alpha)
        {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
        }
    }
}
