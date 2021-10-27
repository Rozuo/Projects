using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// reference https://learn.unity.com/tutorial/introduction-to-object-pooling#5ff8d015edbc2a002063971d
namespace Pool
{
    /// <summary>
    /// Object pool for game objects of the same type such as
    /// bullets and imbue effects. Prevents instantiating and
    /// destroying too many objects
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Private Vars    Description
    /// reserve         gameobject to hold all the particle effects
    /// prefab          prefab to instantiate and pool
    /// size            initial size of pool
    /// poolObjects     list of pooled objects
    /// 
    public class ObjectPool
    {
        private GameObject reserve;
        private GameObject prefab;
        private int size;
        private List<GameObject> poolObjects;

        /// <summary>
        /// constructor for the object pool, storing its parameters and
        /// creates the initial pool
        /// </summary>
        /// <param name="reserve">parent to instantiate under</param>
        /// <param name="prefab">object to pool</param>
        /// <param name="size">size of initial pool</param>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 
        public ObjectPool(GameObject reserve, GameObject prefab, int size)
        {
            this.reserve = reserve;
            this.prefab = prefab;
            this.size = size;
            poolObjects = new List<GameObject>();
            for (int i = 0; i < this.size; i++)
            {
                CreateObject();
            }
        }

        /// <summary>
        /// Sets a pooled object to the target provided.
        /// If no pooled object is found, it will create a new object
        /// and set that to the target provided.
        /// </summary>
        /// <param name="target">target to make parent of pool object</param>
        /// <returns></returns>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 2021-08-04  JH  Size of pool object now to its normal size.
        /// 
        public GameObject SetToObject(GameObject target)
        {
            for (int i = 0; i < size; i++)
            {
                if (!poolObjects[i].activeInHierarchy)
                {
                    poolObjects[i].transform.SetParent(target.transform);
                    poolObjects[i].transform.position = target.transform.position;
                    poolObjects[i].transform.localScale = new Vector3(1, 1, 1);
                    poolObjects[i].SetActive(true);
                    return poolObjects[i];
                }
            }
            // case: no objects remain, create new and return it
            GameObject tmp = CreateObject();
            tmp.transform.SetParent(target.transform);
            tmp.transform.position = target.transform.position;
            tmp.transform.localScale = new Vector3(1, 1, 1);
            tmp.SetActive(true);
            return tmp;
        }

        /// <summary>
        /// Returns the pooled object to the reserve
        /// </summary>
        /// <param name="gO">game object to return to the pool</param>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 
        public void ReturnToPool(GameObject gO)
        {
            gO.transform.SetParent(reserve.transform);
            gO.SetActive(false);
        }

        /// <summary>
        /// Instantiates a new object for the pool
        /// </summary>
        /// <returns>the object instantiated</returns>
        /// 
        /// 2021-06-12  JH  Initial Work
        /// 
        private GameObject CreateObject()
        {
            GameObject tmp = Object.Instantiate(prefab);
            tmp.transform.SetParent(this.reserve.transform);
            tmp.SetActive(false);
            poolObjects.Add(tmp);
            return tmp;
        }
    }
}
