using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// based on https://www.youtube.com/watch?v=aPXvoWVabPY

/// <summary>
/// Unit identitifier to be used by other classes.
/// Flags to allow many different target types for
/// certain actions.
/// </summary>
/// 
/// Author: Jacky Huynh (JH)
/// 
[Flags] public enum UnitType 
{ 
    Player = 1,
    Enemy = 2,
    Environment = 4,
}

namespace Unit.Data
{
    /// <summary>
    /// Scriptable Object for enemy data.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Public Vars     Description
    /// unitType        type of unit
    /// name            name of unit
    /// maxHP           maximum health of unit
    /// unitPrefab      prefab of unit
    /// isDestroyable   for environment unit types, determines if it is destroyable
    /// 
    [CreateAssetMenu(fileName = "UnitData", menuName = "Unit Data")]
    public class UnitData : ScriptableObject
    {
        public UnitType unitType;
        public new string name;
        public float maxHP;
        public GameObject unitPrefab;
        [Header("Environmental Settings")]
        public bool isDestroyable;
    }
}
