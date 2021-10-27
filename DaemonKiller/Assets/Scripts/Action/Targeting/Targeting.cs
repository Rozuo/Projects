using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit.ColorChange;
using Unit.Info;
using Action.Gauge;
using Camera;
using UnityEngine.EventSystems;
using Indicator.Circle;
using CharacterMovement.Player;
using Unit.Info.Environment;
using Action.Overload;
using Action.HUDListView;
using Unit.Player;

namespace Action.Target
{
    /// <summary>
    /// Controls targeting for the player, highlighting enemies and selecting a target
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    ///         Tyson Hoang (TH)
    /// 
    /// Public Vars         Description
    /// player              player to spawn target circle around
    /// maxRange            maximum range of all actions by player
    /// targeting           bool to determine if targeting is on
    /// rangeIndicator      rangeIndicator to spawn
    /// actionGauge         actionGauge to determine if action is possible
    /// dimBG               game object of image to dim the screen
    /// actionOverload      overload system manager
    /// aHUD                action HUD to display when selecting targets
    /// pHUD                player HUD to toggle off resource previews
    /// occlusionLayers     layers that block line of sight
    /// 
    /// Private Vars    
    /// mainCamera          main camera to manipulate upon selecting targets
    /// playerController    player's controller to access exhaust state
    /// playerMotor         player's motor that controls animator
    /// currentTarget       selected target
    /// previousTarget      previously selected target
    /// targetsInRange      list of targetable objects in range (of all possible actions)
    /// envIndicator        environment object's indicator when targeting them
    /// redHighlight        red color for highlighting enemies
    /// blueHighlight       blue color for highlighting the player
    /// yellowHighlight     yellow color for highlighting environment targets
    /// 
    public class Targeting : MonoBehaviour
    {
        // Update later to have player data. remove player + gun
        [Header("Player to spawn indicator on")]
        public GameObject player;
        [HideInInspector]
        public float maxRange = 2f; // default value, changes with actions available
        [HideInInspector]
        public bool targeting = false; 
        public RangeIndicator rangeIndicator; 
        public ActionGauge actionGauge;
        public GameObject dimBG;
        public ActionOverload actionOverload;
        public ActionHUD aHUD;
        public PlayerHUD pHUD;
        [Header("Layers that block line of sight")]
        public LayerMask occlusionLayers;

        private CameraTarget mainCamera;
        private PlayerController playerController;
        private PlayerMotor playerMotor;
        private GameObject currentTarget = null;
        private GameObject previousTarget = null;
        private List<GameObject> targetsInRange = new List<GameObject>();
        private GameObject envIndicator;

        // Theses requires enemy models to have a material to change color with
        private Color redHighlight = Color.red;
        private Color blueHighlight = new Color(0.03921569f, 0.482353f, 0.8039216f);
        private Color yellowHighlight = Color.yellow;

        /// <summary>
        /// Initializes references
        /// </summary>
        /// 
        /// 2021-05-14  JH  Initial Work
        /// 2021-05-21  JH  Find by tag instead of hierarchy
        /// 2021-06-16  JH  Finds game manager instead of through inspector
        /// 2021-07-06  JH  Now gets reference to player controller
        /// 
        void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraTarget>();
            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
            playerMotor = playerController.GetPlayerMotor();
        }


        /// <summary>
        /// Controls the targeting system. The player can target freely out of combat but must wait 
        /// when for the action gauge to be full when in combat. 
        /// Click to target enemies, and escape to deselect enemies.
        /// </summary>
        /// 
        /// 2021-05-14  JH  Initial Work
        /// 2021-05-20  JH  Add actionHUD interactions
        /// 2021-05-21  TH  Added Target flashing
        /// 2021-05-21  JH  Added deselection of target
        /// 2021-05-25  JH  Added new parameter to ActivateActionHUD for distance
        /// 2021-06-16  JH  Targeting now available outside of combat which
        ///                 does not require action.
        ///                 Now targets environment targets, highlights them yellow.
        /// 2021-06-29  JH  Add overload behaviour such as toggling overload mode,
        ///                 removing an action from its queue, and activating all overload actions
        /// 2021-06-30  JH  Change conditions for key presses for an overload.
        ///                 Should feel more intuitive
        /// 2021-07-02  JH  Changed inputs for overload queueing and overload is no longer a toggle.
        ///                 It occurs when queuing up more than one action instead.
        /// 2021-07-06  JH  Targeting and its behaviours now only happens when the player is not exhausted
        /// 
        void Update()
        {
            if (!playerController.GetExhaustion())
            {
                // targeting
                switch (playerController.PlayerState)
                {
                    case PlayerController.State.Exploring:
                        if (Input.GetKeyDown("space") && Time.timeScale == 1f && !playerMotor.GetReloadAnimatorState())
                        {
                            // toggle target system on
                            IndicatorOn();
                            dimBG.SetActive(true);
                            Time.timeScale = 0f;
                        }
                        else if (Input.GetKeyDown("space") && Time.timeScale == 0f)
                        {
                            // toggle target system off
                            EndTargeting();
                            actionGauge.DeactivateActionHUD();
                            pHUD.DisableImbuePreviewChange();
                            actionOverload.DeactivateOverload();
                        }
                        else if (Input.GetMouseButtonDown(0) && Time.timeScale == 0f)
                        {
                            // Select target + display possible actions
                            currentTarget = TargetSelect();
                            if (currentTarget)
                                actionGauge.ActivateActionHUD(currentTarget.GetComponent<UnitInfo>(), DistanceFromTarget(currentTarget));
                        }
                        break;
                    case PlayerController.State.Combat:
                        if (Input.GetKeyDown("space") && actionGauge.IsActionFull() && Time.timeScale == 1f && !playerMotor.GetReloadAnimatorState())
                        {
                            // toggle target system on
                            IndicatorOn();
                            dimBG.SetActive(true);
                            Time.timeScale = 0f;
                        }
                        else if (Input.GetKeyDown("space") && actionGauge.IsActionFull() && Time.timeScale == 0f)
                        {
                            // toggle target system off
                            EndTargeting();
                            actionGauge.DeactivateActionHUD();
                            pHUD.DisableImbuePreviewChange();
                            actionOverload.DeactivateOverload();
                        }
                        else if (Input.GetMouseButtonDown(0) && actionGauge.IsActionFull() && Time.timeScale == 0f)
                        {
                            // Select target + display possible actions
                            currentTarget = TargetSelect();
                            if (currentTarget)
                                actionGauge.ActivateActionHUD(currentTarget.GetComponent<UnitInfo>(), DistanceFromTarget(currentTarget));
                        }
                        break;
                }
                // Deselect target
                if (Input.GetKeyDown("escape") && targeting)
                {
                    DeselectTarget();
                }
                // Remove an action from queue
                else if (Input.GetKeyDown("q") && targeting && !actionOverload.IsListEmpty())
                {
                    actionOverload.RemoveOverloadAction(); // remove a single action from overload
                }
                // Commence action(s)
                else if (Input.GetKeyDown("e") && targeting && !actionOverload.IsListEmpty())
                {
                    actionOverload.ActivateActions();
                    actionOverload.DeactivateOverload();
                    EndTargeting();
                    aHUD.gameObject.SetActive(false);
                }

                // Highlight the target
                if (previousTarget != null)
                {
                    UnitInfo uI = previousTarget.GetComponent<UnitInfo>();
                    Color newColor;

                    switch (uI.GetUnitType())
                    {
                        case UnitType.Player:
                            newColor = Color.Lerp(uI.startColor, blueHighlight, Mathf.Abs(Mathf.Cos(Time.unscaledTime * 3)));
                            break;
                        case UnitType.Enemy:
                            newColor = Color.Lerp(uI.startColor, redHighlight, Mathf.Abs(Mathf.Cos(Time.unscaledTime * 3)));
                            break;
                        case UnitType.Environment:
                            newColor = Color.Lerp(uI.startColor, yellowHighlight, Mathf.Abs(Mathf.Cos(Time.unscaledTime * 3)));
                            break;
                        default:
                            newColor = Color.Lerp(uI.startColor, redHighlight, Mathf.Abs(Mathf.Cos(Time.unscaledTime * 3)));
                            break;
                    }

                    ModifyColor.ChangeUnitColor(uI, newColor);
                }
            }
        }

        /// <summary>
        /// Checks player input to select target, player or enemy.
        /// Cannot click through UI elements.
        /// </summary>
        /// <returns>game object targeted. null if target selection failed</returns>
        ///
        /// 2021-05-14  JH  Inital Work
        /// 2021-05-16  JH  Add return value
        /// 2021-05-21  TH  Added Level Layer Mask in collision check
        ///
        private GameObject TargetSelect()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                // prevents selecting targets over UI elements
                return null;

            RaycastHit hit;
            int levelLayerMask = 1 << 11; //used to ignore the level layer collision

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit, 100f, ~levelLayerMask) && targetsInRange.Contains(hit.transform.gameObject) && Time.timeScale == 0f)
            {
                // enemy selected
                ChangeTarget(hit.transform.gameObject);
                return hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, 100f, ~levelLayerMask) && hit.transform.gameObject == player && Time.timeScale == 0f)
            {
                // player selected
                ChangeTarget(hit.transform.gameObject);
                return hit.transform.gameObject;
            }
            return null; // did not select target
        }
        /// <summary>
        /// Scans for nearby enemies using maximum range of possible actions
        /// and adds enemies in range that are in sight in the list of 
        /// targets in range.
        /// </summary>
        /// 
        /// 2021-05-06  JH  Initial Work
        /// 2021-05-25  JH  Add getting max range from all possible actions
        /// 2021-05-31  JH  Swapped from checking tag to layermask for enemies. 
        /// 2021-06-16  JH  Layermask now includes environmental objects.
        /// 2021-07-05  JH  Now checks if object is between the player and target
        /// 
        private void ScanNearby()
        {
            int enemyLayerMask = 1 << 10; // used to hit only enemies
            int environmentalLayerMask = 1 << 12; // used to hit environmental objects
            int finalLayerMask = enemyLayerMask | environmentalLayerMask;

            // clear previous scan
            targetsInRange.Clear();
            // range to scan with possible actions
            maxRange = aHUD.GetMaxRange();

            // scan objects within a sphere,
            Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, maxRange, finalLayerMask);
            foreach (Collider c in hitColliders)
            {
                if (IsInSight(c.gameObject))
                {
                    targetsInRange.Add(c.gameObject);
                }
            }
        }

        /// <summary>
        /// Determines if a target is within sight for the player
        /// Considers if there is occlusion (center to center)
        /// </summary>
        /// 
        /// Reference: https://www.youtube.com/watch?v=znZXmmyBF-o&ab_channel=TheKiwiCoder
        /// 
        /// <param name="target">target to determine if in sight</param>
        /// <returns>true if target is in sight, false otherwise</returns>
        /// 
        /// 2021-07-05  JH  Initial Work
        /// 2021-07-14  JH  Sight is now at the player's waist level rather than feet.
        /// 2021-07-20  JH  Sight for target is slightly above their feet now.
        /// 2021-08-03  JH  Sight now also checks from feet to feet.
        /// 
        private bool IsInSight(GameObject target)
        {
            //Debug.DrawLine(player.transform.position + new Vector3(0, 1.6f, 0), target.transform.GetComponent<Collider>().bounds.center, Color.red, 10f, false);
            if (Physics.Linecast(player.transform.position + new Vector3(0, 1.6f, 0), 
                                    target.transform.GetComponent<Collider>().bounds.center, occlusionLayers) && 
                                    Physics.Linecast(player.transform.position + new Vector3(0, 0.2f, 0),
                                    target.transform.position + new Vector3(0, 0.2f, 0), occlusionLayers))
                return false;
            return true;
        }

        /// <summary>
        /// Draws a circular indicator for player's range
        /// </summary>
        /// 
        /// 2021-05-06  JH  Initial Start 
        /// 2021-06-30  JH  targeting bool moved here from ChangeTarget
        /// 
        private void IndicatorOn()
        {
            ScanNearby();
            rangeIndicator.SpawnRangeIndicator(player, maxRange);
            player.GetComponent<Animator>().SetBool("Targeting", true);
            targeting = true;
        }
        
        /// <summary>
        /// Removes circular indicator for player's range.
        /// Also reverts color change from the previous target.
        /// </summary>
        /// 
        /// 2021-05-06  JH  Initial Start
        /// 2021-05-16  JH  Add color revert of previous target
        /// 2021-05-21  TH  Add camera work
        /// 2021-05-21  JH  Add clearing values upon disabling
        private void IndicatorOff()
        {
            // revert color of previous target
            if (previousTarget)
            {
                UnitInfo uI = previousTarget.GetComponent<UnitInfo>();
                ModifyColor.ChangeUnitColor(uI, uI.startColor);
            }

            // clear variables
            rangeIndicator.DespawnAllIndicators();
            targetsInRange.Clear();
            previousTarget = null;
            targeting = false;

            // move camera back to player
            mainCamera.SetTarget(player);
            mainCamera.GetComponent<CameraTarget>().InstantMove();
            player.GetComponent<Animator>().SetBool("Targeting", false);
        }

        /// <summary>
        /// Change target selected
        /// Highlights target with appropriate colors and
        /// reverts previous target to its default color.
        /// Moves camera to selected target
        /// </summary>
        /// <param name="gO">game object of new target to change to</param>
        /// 
        /// 2021-05-06  JH  Initial Start   
        /// 2021-05-16  JH  Added Rotation 
        /// 2021-05-21  TH  Added camera work
        /// 2021-06-16  JH  Add color for environment targets
        /// 2021-06-25  JH  Add indicator around environment objects that
        ///                 affect their surroundings
        /// 2021-06-30  JH  targeting bool moved to IndicatorOn
        /// 
        private void ChangeTarget(GameObject gO)
        {
            if (envIndicator != null)
                rangeIndicator.DespawnIndicator(envIndicator);

            UnitType targetType = gO.GetComponent<UnitInfo>().GetUnitType();
            if (previousTarget)
            {
                // Revert color of previous target
                UnitInfo uI = previousTarget.GetComponent<UnitInfo>();
                ModifyColor.ChangeUnitColor(uI, uI.startColor);
            }
            switch (targetType)
            {
                case UnitType.Player:
                    ModifyColor.ChangeUnitColor(player.GetComponent<UnitInfo>(), blueHighlight);
                    break;
                case UnitType.Enemy:
                    ModifyColor.ChangeUnitColor(gO.GetComponent<UnitInfo>(), redHighlight);
                    StartCoroutine(RotateToTarget(gO.transform)); // rotate to target
                    break;
                case UnitType.Environment:
                    EnvironmentInfo eI = gO.GetComponent<EnvironmentInfo>();
                    if (eI.effect != null)
                    {
                        // if environment object has an effect on hit with a range, show it
                        float envRange = eI.effect.GetRange();
                        if (envRange > 0)
                            envIndicator = rangeIndicator.SpawnColorRangeIndicator(gO, envRange, yellowHighlight);
                    }
                    ModifyColor.ChangeUnitColor(eI, yellowHighlight);
                    StartCoroutine(RotateToTarget(gO.transform)); // rotate to target
                    break;
                default:
                    ModifyColor.ChangeUnitColor(gO.GetComponent<UnitInfo>(), redHighlight);
                    StartCoroutine(RotateToTarget(gO.transform)); // rotate to target
                    break;
            }
            previousTarget = gO;
            // move camera to new target
            mainCamera.SetTarget(previousTarget);
        }


        /// <summary>
        /// Rotates the player towards the target smoothly.
        /// </summary>
        /// <param name="target">target's transform to look to</param>
        /// <returns>IEnumerator for coroutine, single frame</returns>
        /// 
        /// 2021-05-16  JH  Initial Work
        /// 
        IEnumerator RotateToTarget(Transform target)
        {
            Vector3 lookDirection = (target.position - player.transform.position).normalized; // get distance 
            lookDirection = Vector3.ProjectOnPlane(lookDirection, Vector3.up); // remove Y component
            Quaternion lookAtRotation = Quaternion.LookRotation(lookDirection);  // get rotation
            while (Quaternion.Angle(player.transform.rotation, lookAtRotation) > 5.0f)
            {
                // rotate smoothly from initial rotation to target 
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookAtRotation, 0.5f);
                yield return null;
            }
        }

        /// <summary>
        /// Returns the distance from the target to the player
        /// </summary>
        /// <returns>float distance from target to the player</returns>
        /// 
        /// 2021-05-25  JH  Initial Work 
        ///
        private float DistanceFromTarget(GameObject target)
        {
            if (target == player)
                // player case, no distance from self
                return 0;
            return Vector3.Distance(target.GetComponent<Collider>().ClosestPoint(player.transform.position), 
                player.transform.position); 
        }

        /// <summary>
        /// Ends targeting system outside.
        /// For use outside of this function.
        /// </summary>
        /// 
        /// 2021-05-20  JH  Initial Work
        /// 
        public void EndTargeting()
        {
            IndicatorOff();
            dimBG.SetActive(false);
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Deselects the target, reverting color, clearing values, 
        /// and resets the HUD and camera
        /// </summary>
        /// 
        /// 2021-05-20  JH  Inital Work
        /// 2021-06-30  JH  Now checks if there was a previous target
        /// 
        private void DeselectTarget()
        {
            if (previousTarget != null)
            {
                UnitInfo uI = previousTarget.GetComponent<UnitInfo>();
                ModifyColor.ChangeUnitColor(uI, uI.startColor);
                previousTarget = null;
                actionGauge.DeactivateActionHUD();
                aHUD.ActionDescBGOff();
                mainCamera.SetTarget(GameObject.FindGameObjectWithTag("Player"));
                mainCamera.InstantMove();
            }
        }
    }
}
