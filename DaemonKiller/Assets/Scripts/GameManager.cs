using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CharacterMovement.Player;
using CharacterMovement.Enemies;
using Audio;
using Unit.Info.Player;

/// <summary>
/// Tracks the Game Status, including a Game Over or Pause
/// </summary>
/// TODO: Update documentation
/// 
/// Author: Tyson Hoang (TH), Rozario (Ross) Beaudin (RB)
/// 
/// public var      desc
/// forceCombat     remain in combat mode until it is disabled
/// 
/// private var     desc
/// gameStatus      The games state
/// player          Record of the players controller.
/// pInfo           the player's info such as health and energy
/// combatEnemies   All enemies that are in combat.
/// 
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameState gameStatus = GameState.Safe;
    private PlayerController player;
    private PlayerInfo pInfo;
    private Dictionary<int, GameObject> combatEnemies = new Dictionary<int, GameObject>();
    
    [SerializeField]
    private GameObject dialPuzzle;

    private int keysIndexes;


    [HideInInspector]
    public bool forceCombat = false;


    /// <summary>
    /// A special Get/Set function for gameStatus
    /// NOTE: gameStatus is altered by GameStatus, they're NOT the same thing.
    /// </summary>
    /// 
    /// 2021-05-11  TH  Initial Implementation
    /// 
    public GameState GameStatus
    {
        get { return gameStatus; }
        set { gameStatus = value; }
    }

    /// <summary>
    /// Forces the gameObject attatched to this Component to persist for the rest of the runtime
    /// </summary>
    /// 
    /// 2021-05-11  TH  Initial Implementation
    /// 2021-07-16  RB  Add sound manager init.
    /// 2021-07-20  JH  Add player info reference.
    /// 
    private void Awake()
    {
        GameObject pl = GameObject.FindGameObjectWithTag("Player");
        //allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        SoundManager.Init();
        if (pl)
        {
            player = pl.GetComponent<PlayerController>();
            pInfo = pl.GetComponent<PlayerInfo>();
        }
    }

    /// <summary>
    /// Check if the player is dead
    /// </summary>
    /// 
    /// 2021-05-11  TH  Placeholder code
    /// 2021-05-20  RB  Remade how the game manager handles game states.
    /// 2021-06-01  TH  Check if player exists
    /// 2021-06-05  TH  Removed/added some GameStatus-related code
    /// 2021-07-20  JH  Add player health check to prevent forcing combat when dead.
    ///
    void Update()
    {
        if (!player || pInfo.CurrentHealth <= 0) return; //do not proceed if player doesn't exist

        if(combatEnemies.Count != 0 || forceCombat)
        {
            // an enemy is approaching the player, enter combat
            GameStatus = GameState.Danger;
            if(player.PlayerState != PlayerController.State.Combat)
            {
                player.SetCombatState();
            }
        }
        else if(player.PlayerState == PlayerController.State.Combat && !forceCombat)
        {
            // no enemies and no longer in danger, leave combat
            GameStatus = GameState.Safe;
            keysIndexes = 0;
            player.SetExploringState();
        }
    }

    /// <summary>
    /// Removes an enemy from combat.
    /// </summary>
    /// <param name="index">The index that we want to remove the enemy from.</param>
    /// 
    /// 2021-05-21 RB Initial documentation
    /// 
    public void RemoveEnemyCombat(int index, EnemyController ec)
    {
        combatEnemies.Remove(index);
        ec.EnemyNumber = -1;
    }

    /// <summary>
    /// Adds an enemy to the game managers records.
    /// </summary>
    /// <param name="enemy">The game object of the enemy.</param>
    /// 
    /// 2021-05-21 RB Initial documentation.
    /// 
    public void AddEnemyCombat(GameObject enemy)
    {
        keysIndexes += 1;
        //int index = keysIndexes;
        combatEnemies.Add(keysIndexes, enemy);
        enemy.GetComponent<EnemyController>().EnemyNumber = keysIndexes;
    }

    /// <summary>
    /// Returns the dial puzzle background.
    /// </summary>
    /// <returns></returns>
    /// 
    /// 2021-07-02 RB Initial Documentation.
    /// 
    public GameObject GetDialPuzzleBackground()
    {
        return dialPuzzle;
    }

    /// <summary>
    /// Changes gameState to a game over
    /// </summary>
    /// 
    /// 2021-05-11  TH  Initial Implementation
    /// 2021-06-01  TH  Now moves to a new scene
    /// 
    public void EndGame()
    {
        Debug.Log("Game Over");
        GameStatus = GameState.GameOver;
        SceneManager.LoadScene("room_gameover");       
    }

    /// <summary>
    /// Begin a timer after death for dramatic purposes
    /// </summary>
    /// 
    /// 2021-05-11  TH  Initial Implementation
    /// 
    public IEnumerator GameOverTimer()
    {
        yield return new WaitForSeconds(3.0f);
        EndGame();
    }

    /// <summary>
    /// Gets the reference of the player.
    /// </summary>
    /// <returns>The player controller.</returns>
    /// 
    /// 2021-07-2021 RB Initial Documentation.
    /// 
    public PlayerController GetPlayer()
    {
        return player;
    }
}

public enum GameState { Safe, Danger, Charge, Ready, Inventory, Pause, GameOver, Cutscene }
