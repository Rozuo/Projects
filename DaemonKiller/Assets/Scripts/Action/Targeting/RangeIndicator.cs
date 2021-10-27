using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Indicator.Circle
{
    /// <summary>
    /// Spawns circular range indicator underneath a target
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars         Description
    /// rangeIndicator      prefab of indicator to spawn
    /// spawnedIndicator    game object of indicator spawned
    /// target              target to spawn indicator on
    /// 
    public class RangeIndicator : MonoBehaviour
    {
        public GameObject rangeIndicator;
        public List<GameObject> indicators = new List<GameObject>();

        /// <summary>
        /// Spawns a circular range indicator
        /// </summary>
        /// <param name="gO">game object to spawn the indicator around</param>
        /// <param name="radius">radius of the indicator</param>
        /// <returns>the spawned indicator</returns>
        /// 2021-05-06  JH  Initial Start
        /// 2021-05-19  JH  Range from int to float
        /// 2021-06-25  JH  Indicator no longer child of GameObject.
        ///                 Also moved slightly higher
        ///           
        public GameObject SpawnRangeIndicator(GameObject gO, float radius)
        {
            GameObject spawnedIndicator = Instantiate(rangeIndicator, gO.transform);
            spawnedIndicator.transform.parent = null;
            Vector3 newPos = new Vector3(spawnedIndicator.transform.position.x, spawnedIndicator.transform.position.y + 0.1f, spawnedIndicator.transform.position.z);
            spawnedIndicator.transform.position = newPos;
            spawnedIndicator.transform.localScale = new Vector3(radius * 2, radius * 2, 1); // *2 to compensate radius (normally in diameter)

            indicators.Add(spawnedIndicator);

            return spawnedIndicator;
        }

        /// <summary>
        /// Spawns a circular range indicator of the desired color
        /// </summary>
        /// <param name="gO">game object to spawn the indicator around</param>
        /// <param name="radius">radius of the indicator</param>
        /// <param name="color">color of the indicator</param>
        /// <returns>the spawned indicator</returns>
        /// 
        /// 2021-06-25  JH  Initial Work
        /// 
        public GameObject SpawnColorRangeIndicator(GameObject gO, float radius, Color color)
        {
            GameObject spawnedIndicator = Instantiate(rangeIndicator, gO.transform);
            spawnedIndicator.transform.parent = null;
            Vector3 newPos = new Vector3(spawnedIndicator.transform.position.x, spawnedIndicator.transform.position.y + 0.1f, spawnedIndicator.transform.position.z);
            spawnedIndicator.transform.position = newPos;
            spawnedIndicator.transform.localScale = new Vector3(radius * 2, radius * 2, 1); // *2 to compensate radius (normally in diameter)
            spawnedIndicator.GetComponent<SpriteRenderer>().color = color;

            indicators.Add(spawnedIndicator);

            return spawnedIndicator;
        }

        /// <summary>
        /// Adjusts the circular range indicator radius. 
        /// Assume range indicator has already been made.
        /// Use when changing guns with different range.
        /// </summary>
        /// 
        /// 2021-05-06  JH  Initial Start
        /// 
        public void AdjustRangeIndicator(int index, int range)
        {
            indicators[index].transform.localScale = new Vector3(range, range, 0);
        }

        /// <summary>
        /// Destroys all range indicators
        /// </summary>
        /// 
        /// 2021-05-06  JH  Initial Start
        /// 2021-06-25  JH  Changed to despawn all indicators
        /// 
        public void DespawnAllIndicators()
        {
            foreach(GameObject indicator in indicators)
            {
                Destroy(indicator);
            }
        }

        /// <summary>
        /// Destroys a single indicator 
        /// </summary>
        /// <param name="indicator">indicator to despawn</param>
        /// 
        /// 2021-06-25  JH  Initial Work
        /// 
        public void DespawnIndicator(GameObject indicator)
        {
            if (indicators.Contains(indicator))
            {
                indicators.Remove(indicator);
                Destroy(indicator);
            }
        }
    }

}


