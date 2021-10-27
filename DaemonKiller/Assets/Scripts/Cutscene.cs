using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Manages the game's Cutscenes
/// </summary>
/// 
/// Author: Tyson Hoang (TH)
/// 
/// private var     desc
/// isCompleted     if the cutscene has ended
/// playDirector    Component for PlayableDirector
/// gameUi          Component for GameUI
/// playerAnim      Component for Animator from Player GameObject
/// targetingObj    GameObject for Targeting from GameDirector GameObject
/// mainCamera      GameObject for the Main Camera used during gameplay
public class Cutscene : MonoBehaviour
{
    public bool isCompleted;
    private PlayableDirector playDirector;
    private GameUI gameUi;
    private Animator playerAnim;
    private GameObject targetingObj;
    private GameObject mainCamera;

    /// <summary>
    /// Checks if the cutscene has already been activated
    /// </summary>
    /// 
    /// 2021-07-02  TH  Initial Implementation
    /// 
    private void OnEnable()
    {
        if (isCompleted)
            gameObject.SetActive(false);
    }


    /// <summary>
    /// Set up references and components
    /// </summary>
    /// 
    /// 2021-07-02  TH  Initial Implementation
    /// 
    void Start()
    {
        playDirector = GetComponent<PlayableDirector>();
        gameUi = GameObject.FindGameObjectWithTag("Game HUD").GetComponent<GameUI>();
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        targetingObj = GameObject.FindGameObjectWithTag("Targeting");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");       

        targetingObj.SetActive(false);  // deactivate targeting GameObject as it interferes with inputs
        mainCamera.SetActive(false);    // deactivate the main camera to prevent conflicts with other cameras
        playerAnim.updateMode = AnimatorUpdateMode.Normal; // "freeze" player animations
        Time.timeScale = 0;             //pause time
    }

    /// <summary>
    /// Check for player input to advance dialogue
    /// </summary>
    /// 
    /// 2021-07-02  TH  Initial Implementation
    /// 
    void Update()
    {
        if(playDirector.state == PlayState.Paused && Input.GetButtonDown("Submit"))
        {
            gameUi.ClearText();
            playDirector.Resume();
        }
    }

    /// <summary>
    /// Method to use when at the end of a timeline. Used as a UnitySignal
    /// </summary>
    /// 
    /// 2021-07-02  TH  Initial Implementation
    /// 
    public void EndTimeline()
    {
        targetingObj.SetActive(true);
        mainCamera.SetActive(true);
        playerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        Time.timeScale = 1.0f;          //resume time
        isCompleted = true;             // set cutscene as completed
        gameObject.SetActive(false);    // deactivate cutscene GameObject
    }
}
