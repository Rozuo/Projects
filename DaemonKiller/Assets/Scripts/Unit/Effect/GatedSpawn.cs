using Action.Info.Imbues;
using Logic;
using Pool.ElectricParticle2;
using Pool.FireParticle;
using Pool.IceParticle;
using Unit.Info.Environment;
using UnityEngine;

namespace EnvironmentEffect
{
    /// <summary>
    /// Spawns its contents when the gate is true.
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// Steps to Use:
    /// 1. Put GatedSpawn onto an empty game object.
    /// 2. Create game objects that will affect this gate.
    ///    Game Objects should have a script that will hold onto its own gate and 
    ///    have a way to trigger it.
    ///         Example: SwitchEffectManager should have a switch gate assigned to it and 
    ///                  is a component for an environment object. Environment object
    ///                  should have TimedSwitchEffect as its effect.
    ///                  Note - each switch must be its own unique scriptable object
    /// 3. Put the gates from the previous step into this Component's inputs
    /// 4. Put Game Objects to spawn into the spawner array and set them as inactive
    /// 
    /// Public Vars             Description
    /// gate                    gate's value to check
    /// spawners                game objects to spawn
    /// deactivateOnCompletion  deactivate game objects when gate is true
    /// isActivated             spawn activated
    /// 
    public class GatedSpawn : MonoBehaviour
    {
        [Header("Gate for bool logic checking")]
        public Gate gate;

        [Header("Items to spawn upon true on Gate")]
        public GameObject[] spawners;

        [Header("Deactivate objects upon completion")]
        public bool deactivateOnCompletion = false;

        [HideInInspector] public bool isActivated = false;

        /// <summary>
        /// Initialize spawner
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        void Awake()
        {
            SetInactive();
        }

        /// <summary>
        /// Manages the behaviour for the gate
        /// </summary>
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 2021-07-22  JH  Now only spawns objects that are not null 
        ///                 Also makes them untargetable after the gate is activated
        /// 
        void Update()
        {
            if (!isActivated && gate.value)
            {
                isActivated = true;
                // Activate everything in the spawners array
                if (spawners.Length > 0)
                {
                    // Activate everything in the spawners array
                    foreach (GameObject gO in spawners)
                        if (gO != null)
                            gO.SetActive(true);
                }
                foreach (Transform t in transform)
                {
                    if (t.gameObject.layer == 12)
                    {
                        EnvironmentInfo eI = t.GetComponent<EnvironmentInfo>();
                        if (eI != null && eI.onImbue)
                            // leaves particle on for environments
                            eI.StopEndParticle();
                        // Make them untargetable afterwards
                        t.gameObject.layer = 0;
                    }
                }
                Deactivate();
            }
        }

        /// <summary>
        /// Set all objects in the array inactive recursively (if multiple triggers exist).
        /// </summary>
        /// Note: Modified from TriggerManager by Tyson Hoang (TH)
        /// 
        /// 2021-06-24  JH  Initial Work
        /// 
        private void SetInactive()
        {
            foreach (GameObject gO in spawners)
            {
                gO.SetActive(false);
            }
        }

        /// <summary>
        /// Deactivates the gate.
        /// Only occurs if gate is activated and deactivate bool is checked.
        /// </summary>
        /// 
        /// 2021-07-22  JH  Initial Implementation
        /// 
        public void Deactivate()
        {
            if (deactivateOnCompletion && isActivated)
                gameObject.SetActive(false);
        }

        /// <summary>
        /// Set particles to the objects required to switch this gate.
        /// Only applies to environmental objects that have the 
        /// onImbue bool checked.
        /// </summary>
        /// 
        /// 2021-07-21  JH  Initial Implementation
        /// 
        public void SetParticles()
        {
            if (isActivated)
            {
                foreach (Transform t in transform)
                {
                    if (t.gameObject.layer == 12)
                    {
                        EnvironmentInfo eI = t.GetComponent<EnvironmentInfo>();
                        if (eI != null && eI.onImbue)
                        {
                            switch (eI.imbueCondition)
                            {
                                case ImbueType.Electric:
                                    ElectricParticlePool2.SetToObject(t.gameObject); // CHANGE HERE
                                    break;
                                case ImbueType.Fire:
                                    FireParticlePool.SetToObject(t.gameObject);
                                    break;
                                case ImbueType.Ice:
                                    IceParticlePool.SetToObject(t.gameObject);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
