using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit.Data;
using Unit.Info;
using Unit.Info.Player;
using Unit.Info.Environment;
using CharacterMovement.Enemies;
using UnityEngine.SceneManagement;
using Interactable;
using Interactable.UIPuzzles;
using Camera;
using EnvironmentEffect;

/// <summary>
/// Manages the saving and loading of perisistent data.
/// </summary>
/// 
/// Author: Tyson Hoang (TH), Jacky Huynh (JH)
/// 
/// Public Vars     Description
/// instance        instance of the SaveDataManager, following the singleton pattern
/// enemyPrefabs    list of enemy prefabs to reinstantiate
/// pickupPrefabs   list of pickup item prefabs to reinstantiate
/// envPrefabs      list of environmental object prefabs to reinstantiate
/// gunSO           list of gun scriptable objects to set the player's gun
/// loadingData     bool to know if data is loading
/// loadingScene    bool to know if scene is loading
/// 
/// Private Vars    Description
/// gameData        holds all the game data to write to a JSON 
/// 
public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager instance;
    public List<GameObject> pickupPrefabs;
    public List<GameObject> envPrefabs;
    public List<InventoryItem> gunSO;
    [HideInInspector]
    public bool loadingData;
    [HideInInspector]
    public bool loadingScene;

    private GameData gameData = new GameData();
    private string saveDirectory;
    private string saveFilePath;
    private string savePlayerFilePath;

    /// <summary>
    /// Setup singleton pattern and save file paths.
    /// </summary>
    /// 
    /// 2021-07-12  JH  Initial Implementation
    /// 2021-07-28  JH  Save paths added
    /// 
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            loadingData = false;
            loadingScene = false;
            DontDestroyOnLoad(gameObject);
            saveDirectory = Path.Combine(Application.persistentDataPath, "SaveData");
            saveFilePath = Path.Combine(saveDirectory, "savedata.sav");
            savePlayerFilePath = Path.Combine(saveDirectory, "playerdata.sav");
        }
    }

    /// <summary>
    /// Calls the SaveDataManager's SaveData method
    /// </summary>
    /// Context Menu methods can be activated through the Inspector (right-click the component).
    /// 
    /// 2021-06-11  TH  Initial Implementation
    /// 
    [ContextMenu("[Debug] Save Game Data")]
    private void InvokeSave()
    {
        SaveData();
    }

    /// <summary>
    /// Calls the SaveDataManager's LoadData method
    /// </summary>
    /// Context Menu methods can be activated through the Inspector (right-click the component).
    /// 
    /// 2021-06-11  TH  Initial Implementation
    /// 
    [ContextMenu("[Debug] Load Last Save")]
    private void InvokeLoad()
    {
        StartCoroutine(LoadData());
    }
    
    /// <summary>
    /// Loads the scene from the save file
    /// </summary>
    /// 
    /// 2021-07-13  JH  Initial Implmentation
    /// 2021-07-14  JH  Add check for file.
    /// 
    public void LoadScene()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No Save File Found!");
            return;
        }
        if (!loadingScene)
        {
            string jsonStr = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(jsonStr);
            AsyncOperation load = SceneManager.LoadSceneAsync(data.sName, LoadSceneMode.Single);
            StartCoroutine(WaitForLoad(load));
        }
    }

    /// <summary>
    /// Waits for the scene to load.
    /// Also sets the loading scene bool to prevent multiple loads at once.
    /// </summary>
    /// <param name="loading">asyncload of the scene</param>
    /// <returns>null while scene is loading</returns>
    private IEnumerator WaitForLoad(AsyncOperation loading)
    {
        this.loadingScene = true;
        while (!loading.isDone)
        {
            yield return null;
        }
        this.loadingScene = false;
    }

    /// <summary>
    /// Creates the directory for the save file if it is not made
    /// </summary>
    /// 
    /// 2021-07-26  JH  Initial Documentation
    /// 
    private void CreateSaveDirectory()
    {
        // Check path
        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);
    }

    /// <summary>
    /// Holds important game data to save/load
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    /// 
    /// public var      desc
    /// rName           The Game Room the game saved in
    /// sName           The Scene the game saved in
    /// pLocation       Player's location
    /// pRotation       Player's orientation
    /// pCurrentHealth  Player's health
    /// pMaxHealth      Player's health capacity
    /// pCurrentEnergy  Player's energy
    /// pMaxEnergy      Player's energy capacity
    /// pGun            ID of Player's equipped weapon 
    /// pClips          Player's current ammo in all weapons
    /// iData           Saved inventory
    /// rData           Saved data for all rooms
    /// sSaveCount      How many times the game was saved this playthrough
    [System.Serializable]
    private class GameData
    {
        public string rName;
        public string sName;
        public Vector3 pLocation;
        public Quaternion pRotation;
        public float pCurrentHealth;
        public float pMaxHealth;
        public float pCurrentEnergy;
        public float pMaxEnergy;
        public Item pGun;
        public List<AmmoClip> pClips = new List<AmmoClip>();
        public List<InventorySlot> iData = new List<InventorySlot>();
        public List<RoomData> rData = new List<RoomData>();

        //meta
        public static int sSaveCount;
        // human-readable room name
        // current playtime
        // save date


        /// <summary>
        /// Increment save counter
        /// </summary>
        /// 
        /// 2021-06-11  TH  Initial Implementation
        /// 
        public void IncrementSave()
        {
            sSaveCount++;
        }

        /// <summary>
        /// Finds and updates the GameObjects held in the room to be saved
        /// </summary>
        /// <param name="roomName">The room to save the data</param>
        /// 
        /// 2021-06-11  TH  Initial Implementation
        /// 2021-07-12  JH  Now overwrites a room if the room already 
        ///                 exists rather than throwing an exception
        /// 2021-07-13  JH  Now saves door states in a room
        /// 
        public void SaveRoomData(string roomName)
        {
            RoomData rm = new RoomData();
            rm.rName = roomName;
            rm.SaveGameObjects();
            rm.SaveRoomDoors();
            if (rData.Exists(x => x.rName == roomName))
            {
                rData[rData.FindIndex(x => x.rName == roomName)] = rm;
            }
            else
            {
                rData.Add(rm);
            }
        }

        /// <summary>
        /// Clears the Game Data for a new scene.
        /// Clears every entry.
        /// </summary>
        /// 
        /// 2021-07-19  JH  Initial Implementation.
        /// 
        public void ClearData()
        {
            rName = null;
            sName = null;
            pLocation = new Vector3(0, 0, 0);
            pRotation = Quaternion.identity;
            pCurrentHealth = 0;
            pMaxHealth = 0;
            pCurrentEnergy = 0;
            pMaxEnergy = 0;
            pGun = 0;
            pClips.Clear();
            iData.Clear();
            rData.Clear();
        }
    }

    /// <summary>
    /// Clears the Game Data for a new scene.
    /// Also sets the active scene's name
    /// </summary>
    /// 
    /// 2021-07-26  JH  Initial Implementation.
    /// 
    public void ClearData()
    {
        gameData.ClearData();
        gameData.sName = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Fetches data, converts to JSON, then saves to file
    /// </summary>
    /// 
    /// 2021-06-11  TH  Initial Implementation
    /// 2021-07-12  JH  Changed Inventory and RoomManager referencing tag.
    ///                 Also now saves the current scene string.
    /// 2021-07-20  JH  gameData is cleared upon saving in a different scene.
    /// 
    public void SaveData()
    {
        if (gameData.sName != SceneManager.GetActiveScene().name)
        {
            gameData.ClearData();
        }
        // Get Component References      
        GameObject hudMan = GameObject.FindGameObjectWithTag("Game HUD");
        GameObject pl = GameObject.FindGameObjectWithTag("Player");
        Inventory invData = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
        RoomManager roomMan = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<RoomManager>();

        if (pl)
        {
            // Add player data to the GameData class
            PlayerInfo playerData = pl.GetComponent<PlayerInfo>();

            gameData.rName = roomMan.CurrentRoom.name;

            gameData.pLocation = pl.transform.position;
            gameData.pRotation = pl.transform.rotation;

            gameData.pCurrentHealth = playerData.currentHealth;
            gameData.pMaxHealth = playerData.maxHealth;
            gameData.pCurrentEnergy = playerData.currentEnergy;
            gameData.pMaxEnergy = playerData.maxEnergy;
            if (playerData.GetGun() != null)
            {
                gameData.pGun = playerData.GetGun().itemID;
                gameData.pClips = playerData.AmmoClips;
            }
        }
        else
        {
            Debug.LogWarning("Player wasn't found during saving. " +
                "If this isn't intentional, the player may not exist yet.");
        }

        // Add Inventory and Room data to the GameData class
        gameData.iData = invData.inventory;
        gameData.SaveRoomData(roomMan.CurrentRoom.name);

        // Save current scene name
        gameData.sName = SceneManager.GetActiveScene().name;

        // Do meta updates
        gameData.IncrementSave();

        // Check path
        CreateSaveDirectory();

        // Convert to JSON string, then save to a text file.
        string jsonStr = JsonUtility.ToJson(gameData, true);        
        File.WriteAllText(saveFilePath, jsonStr);

             
        // Notify a successful game save.
        Debug.Log("Game Saved");
        hudMan.GetComponent<GameUI>().ChangeTextTimed("Game Saved.");
    }

    /// <summary>
    /// Fetches data, converts to JSON, then saves to file
    /// Saves only player data for loading.
    /// Used for transfering player data between scenes.
    /// </summary>
    /// 
    /// 2021-07-12  JH  Initial Implementation
    ///
    public void SavePlayerData()
    {
        // Get Component References      
        GameObject pl = GameObject.FindGameObjectWithTag("Player");
        Inventory invData = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();

        if (pl)
        {
            // Add player data to the GameData class
            PlayerInfo playerData = pl.GetComponent<PlayerInfo>();

            gameData.pCurrentHealth = playerData.currentHealth;
            gameData.pMaxHealth = playerData.maxHealth;
            gameData.pCurrentEnergy = playerData.currentEnergy;
            gameData.pMaxEnergy = playerData.maxEnergy;
            gameData.pGun = playerData.GetGun().itemID;
            gameData.pClips = playerData.AmmoClips;
        }
        else
        {
            Debug.LogWarning("Player wasn't found during saving. " +
                "If this isn't intentional, the player may not exist yet.");
        }

        // Add Inventory and Room data to the GameData class
        gameData.iData = invData.inventory;

        // Do meta updates
        gameData.IncrementSave();

        // Check path
        CreateSaveDirectory();

        // Convert to JSON string, then save to a text file.
        string jsonStr = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePlayerFilePath, jsonStr);
        //Debug.Log(jsonStr);

        // Notify a successful game save.
        Debug.Log("Player Data Saved");
    }

    /// <summary>
    /// Saves the current room state and stores
    /// it in the game data.
    /// </summary>
    /// 
    /// 2021-07-12  JH  Initial Implementation
    /// 
    public void SaveCurrentRoomData()
    {
        RoomManager roomMan = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<RoomManager>();
        gameData.SaveRoomData(roomMan.CurrentRoom.name);
    }

    /// <summary>
    /// Loads and updates in-game objects with JSON data
    /// </summary>
    /// 
    /// 2021-06-11  TH  Initial Implementation
    /// 2021-07-12  JH  Now loads the scene instead of assuming you are in the same scene.
    ///                 Now only destroy and replace objects if the player has visited the room before in the SaveData,
    ///                 otherwise keep room in its original state.
    /// 2021-07-13  JH  No longer loads the scene. GameObjects that are tagged 'Indestructible' or 'Events' will not be destroyed anymore
    ///                 EventInteracts are now loaded properly.
    ///                 Door states are now loaded properly.
    /// 2021-07-14  JH  Add check for file. 
    ///                 GameData replaced by the loaded data.
    ///                 Add saving of switches, puzzles, enemies
    /// 2021-07-20  JH  Inventory now loads via prefabs.
    ///                 AmmoClips now loads via scriptable object 
    /// 2021-07-21  JH  Changed to an IEnumerator to wait a frame to destroy objects.
    ///                 Now saves DoT Environmental objects.
    ///                 Trigger spawns will have items and environmental objects reassigned
    ///                 in their spawners.
    ///                 Camera will now move to player immediately
    /// 2021-07-22  JH  Now loads gated spawns.
    /// 
    public IEnumerator LoadData()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No Save File Found!");
            yield break;
        }
        if (!loadingData)
        {
            loadingData = true;
            string jsonStr = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(jsonStr);
            gameData = data;

            // Get Component References
            GameObject hudMan = GameObject.FindGameObjectWithTag("Game HUD");
            GameObject pl = GameObject.FindGameObjectWithTag("Player");
            Inventory invData = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();
            RoomManager roomMan = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<RoomManager>();

            // Clear and update Inventory
            invData.inventory.Clear();
            foreach (InventorySlot itm in data.iData)
            {
                InventoryItem itmDetails = pickupPrefabs.Find(x => x.GetComponent<ItemInteract>().item.itemID == itm.itemID).GetComponent<ItemInteract>().item;
                invData.AddItem(itm.GetAmount(), itmDetails);
            }

            // Clear and update Game Objects in rooms
            roomMan.SetRoomsActive();
            GameObject lvlObj = GameObject.Find("Level");

            if (lvlObj != null)
            {
                // for each room
                foreach (Transform child in lvlObj.transform)
                {
                    // only reset objects for rooms in save data (visited rooms)
                    if (data.rData.Exists(x => x.rName == child.gameObject.name))
                    {
                        Transform roomObjects = child.Find("GameObjects");
                        if (roomObjects)
                        {
                            int count = roomObjects.childCount;
                            for (int i = 0; i < count; i++)
                            {
                                GameObject gO = roomObjects.GetChild(i).gameObject;
                                if (!(gO.CompareTag("Indestructible") || gO.CompareTag("Trigger") || gO.CompareTag("Event")
                                    || gO.CompareTag("Enemy") || gO.CompareTag("Switch") || gO.CompareTag("Puzzle") || gO.CompareTag("Gated Spawn")))
                                {
                                    Destroy(gO);
                                    gO = null;
                                }
                            }
                        }
                        else
                            Debug.Log("Room doesnt have Game Objects!");
                    }
                }
            }

            // wait a frame for destroy
            yield return null;

            // Reinstantiate GameObjects
            foreach (RoomData rData in data.rData)
            {
                Transform parent = GameObject.Find("Level").transform.Find(rData.rName).transform.Find("GameObjects");
                List<GameObject> pickUpList = new List<GameObject>();
                List<GameObject> envList = new List<GameObject>();

                for (int i = 0; i < rData.rPickups.Count; i++) // set the room's pickups...
                {
                    SavePickups pickupObj = rData.rPickups[i];
                    int pickupInd = pickupPrefabs.FindIndex(x => x.GetComponent<ItemInteract>().item.itemName == pickupObj.itmName);

                    GameObject newObj = Instantiate(pickupPrefabs[pickupInd], pickupObj.oPos, pickupObj.oRot);
                    newObj.transform.SetParent(parent);
                    newObj.GetComponent<ItemInteract>().amount = pickupObj.amount;
                    newObj.SetActive(pickupObj.isActive);
                    GameObject itm = newObj;
                    pickUpList.Add(itm);
                }

                for (int i = 0; i < rData.rEvents.Count; i++) // set the room's event triggers...
                {
                    SaveEvents eventObj = rData.rEvents[i];
                    GameObject newObj = parent.Find(eventObj.name).gameObject;
                    EventInteract eI = newObj.GetComponentInChildren<EventInteract>();

                    newObj.SetActive(eventObj.isActive);
                    eI.isActivated = eventObj.activated;
                    eI.InteractableState = eventObj.state;
                    eI.SetActivated(eI.isActivated);
                    if (eI.isActivated)
                        eI.OnActivationEvent.Invoke();
                }

                for (int i = 0; i < rData.rSwitches.Count; i++) // set the room's switches...
                {
                    SaveSwitches switchObj = rData.rSwitches[i];
                    GameObject newObj = parent.Find(switchObj.name).gameObject;
                    SwitchInteract sI = newObj.GetComponentInChildren<SwitchInteract>();

                    newObj.SetActive(switchObj.isActive);
                    sI.isActivated = switchObj.activated;
                    sI.InteractableState = switchObj.state;
                    sI.SetActivated(sI.isActivated);
                }

                for (int i = 0; i < rData.rPuzzles.Count; i++) // set the room's puzzles...
                {
                    SavePuzzles puzzleObj = rData.rPuzzles[i];
                    GameObject newObj = parent.Find(puzzleObj.name).gameObject;
                    Puzzle puzzle = newObj.GetComponent<Puzzle>();

                    newObj.SetActive(puzzleObj.isActive);
                    puzzle.SetState(puzzleObj.pState);
                    puzzle.SetCorrect(puzzleObj.correct);
                    if (puzzleObj.pState == Puzzle.PuzzleStates.Solved)
                    {
                        puzzle.unlockableObject.InteractableState = State.Open;
                    }
                }

                UnitInfo[] unitInfos = parent.GetComponentsInChildren<UnitInfo>(true);
                if (unitInfos.Length > 0) // Set room's enemies...
                {
                    int uIIndex = 0;
                    for (int i = 0; i < rData.rEnemies.Count; i++, uIIndex++) 
                    {
                        SaveEnemies enemyObj = rData.rEnemies[i];
                        while (!unitInfos[uIIndex].CompareTag("Enemy"))
                        {
                            uIIndex += 1;
                            if (uIIndex >= unitInfos.Length)
                                break;
                        }
                        if (uIIndex >= unitInfos.Length)
                            break;

                        // Set enemy position and active status
                        GameObject newObj = unitInfos[uIIndex].gameObject;
                        newObj.transform.position = enemyObj.oPos;
                        newObj.transform.rotation = enemyObj.oRot;
                        newObj.SetActive(enemyObj.isActive);
                    }
                }

                for (int i = 0; i < rData.rEnvironments.Count; i++) // spawn the environmental objects...
                {
                    SaveEnvironmentals envObj = rData.rEnvironments[i];
                    if (envPrefabs.Exists(x => x.GetComponent<UnitInfo>().unitData.name == envObj.name))
                    {
                        int envInd = envPrefabs.FindIndex(x => x.GetComponent<UnitInfo>().unitData.name == envObj.name);

                        GameObject newObj = Instantiate(envPrefabs[envInd], envObj.oPos, envObj.oRot);
                        newObj.transform.localScale = envObj.oScale;
                        newObj.transform.SetParent(parent);
                        newObj.SetActive(envObj.isActive);
                        envList.Add(newObj);
                    }
                }

                for (int i = 0; i < rData.rGatedSpawns.Count; i++) // spawn gated spawns
                {
                    SaveGatedSpawns gSObj = rData.rGatedSpawns[i];
                    GameObject newObj = parent.Find(gSObj.name).gameObject;
                    GatedSpawn gSComp = newObj.GetComponent<GatedSpawn>();

                    gSComp.isActivated = gSObj.isActivated;
                    newObj.SetActive(gSObj.isActive);
                    gSComp.SetParticles();
                    gSComp.deactivateOnCompletion = gSObj.deactivate;
                    gSComp.Deactivate();

                    int gSIndex = 0;
                    for (int j = 0; j < gSComp.spawners.Length; j++)
                    {
                        GameObject nullObject = gSComp.spawners[j];
                        if (nullObject == null && gSIndex < gSObj.objectTypes.Count)
                        {
                            SaveObjectType type = gSObj.objectTypes[gSIndex++];
                            // respawn instance based on position
                            switch (type)
                            {
                                case SaveObjectType.pickup:
                                    for (int k = 0; k < gSObj.pickUpPosition.Count; k++)
                                    {
                                        GameObject gO = pickUpList.Find(x => Vector3.Distance(x.transform.position, gSObj.pickUpPosition[k]) < 0.2f);
                                        if (gO)
                                        {
                                            gSComp.spawners[j] = gO;
                                            pickUpList.Remove(gO);
                                            break;
                                        }
                                    }
                                    break;
                                case SaveObjectType.environmental:
                                    for (int k = 0; k < gSObj.envPosition.Count; k++)
                                    {
                                        GameObject gO = envList.Find(x => Vector3.Distance(x.transform.position, gSObj.envPosition[k]) < 0.2f);
                                        if (gO)
                                        {
                                            gSComp.spawners[j] = gO;
                                            envList.Remove(gO);
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }

                for (int i = 0; i < rData.rTriggers.Count; i++) // set the room's triggers...
                {
                    SaveTriggers triggerObj = rData.rTriggers[i];
                    GameObject newObj = parent.Find(triggerObj.name).gameObject;
                    TriggerManager trigComp = newObj.GetComponent<TriggerManager>();

                    int triggerIndex = 0;
                    if (triggerObj.tType == TriggerManager.TriggerType.spawn)
                    {
                        for (int j = 0; j < trigComp.spawners.Length; j++)
                        {
                            GameObject nullObject = trigComp.spawners[j];
                            if (nullObject == null && triggerIndex < triggerObj.objectTypes.Count)
                            {
                                SaveObjectType type = triggerObj.objectTypes[triggerIndex++];
                                // respawn instance based on position
                                switch (type)
                                {
                                    case SaveObjectType.pickup:
                                        for (int k = 0; k < triggerObj.pickUpPosition.Count; k++)
                                        {
                                            GameObject gO = pickUpList.Find(x => Vector3.Distance(x.transform.position, triggerObj.pickUpPosition[k]) < 0.2f);
                                            if (gO)
                                            {
                                                trigComp.spawners[j] = gO;
                                                pickUpList.Remove(gO);
                                                break;
                                            }
                                        }
                                        break;
                                    case SaveObjectType.environmental:
                                        for (int k = 0; k < triggerObj.envPosition.Count; k++)
                                        {
                                            GameObject gO = envList.Find(x => Vector3.Distance(x.transform.position, triggerObj.envPosition[k]) < 0.2f);
                                            if (gO)
                                            {
                                                trigComp.spawners[j] = gO;
                                                envList.Remove(gO);
                                                break;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    trigComp.isActivated = triggerObj.activated;
                    newObj.SetActive(triggerObj.isActive);
                    trigComp.Retrigger();
                }
            }

            // Set door states
            foreach (RoomData rData in data.rData)
            {
                Transform parent = GameObject.Find("Level").transform.Find(rData.rName).transform.Find("brush");
                if (parent) // edge case for rooms without 'brush'
                {
                    DoorInteract[] doors = parent.GetComponentsInChildren<DoorInteract>();
                    if (doors.Length > 0)
                    {
                        for (int i = 0; i < rData.rDoors.Count; i++)
                        {
                            SaveDoors doorObj = rData.rDoors[i];
                            doors[i].InteractableState = doorObj.state;
                        }
                    }
                }
            }

            // load player data
            if (pl)
            {
                PlayerInfo playerData = pl.GetComponent<PlayerInfo>();
                GameObject loadRoom = GameObject.Find("Level").transform.Find(data.rName).gameObject;
                roomMan.SetRoomsInactive();

                pl.GetComponent<Rigidbody>().useGravity = false; //freeze player during loading

                // Move to the room to load
                roomMan.SetRoomActive(loadRoom, true);
                roomMan.SetRoomCurrent(loadRoom);

                // Update player position         
                pl.transform.position = data.pLocation;
                pl.transform.rotation = data.pRotation;

                // Move camera to player position
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraTarget>().InstantMove();

                // Set player data
                playerData.CurrentHealth = data.pCurrentHealth;
                playerData.maxHealth = data.pMaxHealth;
                playerData.currentEnergy = data.pCurrentEnergy;
                playerData.maxEnergy = data.pMaxEnergy;
                
                // Clear and set ammo clips
                playerData.AmmoClips = new List<AmmoClip>();
                for (int i = 0; i < data.pClips.Count; i++)
                {
                    InventoryItem gunType = gunSO.Find(x => x.itemID == data.pClips[i].gunID);
                    playerData.AddAmmoClip(new AmmoClip(gunType, data.pClips[i].GetAmount()));
                }
                if (playerData.AmmoClips.Count != 0)
                    playerData.ChangeGun(gunSO.Find(x => x.itemID == data.pGun));

                pl.GetComponent<Rigidbody>().useGravity = true; //unfreeze player
            }
            else
            {
                Debug.LogWarning("Player data wasn't found during loading. Player will not be affected.");
            }

            // Notify successful load.
            Debug.Log("Game Data loaded");
            hudMan.GetComponent<GameUI>().ChangeTextTimed("Game Loaded.");
            loadingData = false;
        }
    }


    /// <summary>
    /// Player data with JSON data
    /// Used for transfering player data between scenes.
    /// </summary>
    /// 
    /// 2021-07-12  JH  Initial Implementation
    /// 2021-07-13  JH  Added loading bool to prevent loading when 
    /// 2021-07-14  JH  Add check to file.
    /// 
    public void LoadPlayerData()
    {
        if (!File.Exists(savePlayerFilePath))
        {
            Debug.Log("No player data found!");
            return;
        }
        if (!loadingData)
        {
            loadingData = true;
            // Fetch text file, then generate Class values from JSON.
            string jsonStr = File.ReadAllText(savePlayerFilePath);
            GameData data = JsonUtility.FromJson<GameData>(jsonStr);

            // Get Component References
            GameObject pl = GameObject.FindGameObjectWithTag("Player");
            Inventory invData = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Inventory>();

            // Clear and update Inventory
            invData.inventory.Clear();
            foreach (InventorySlot itm in data.iData)
            {

                InventoryItem itmDetails = pickupPrefabs.Find(x => x.GetComponent<ItemInteract>().item.itemID == itm.itemID).GetComponent<ItemInteract>().item;
                invData.AddItem(itm.GetAmount(), itmDetails);
            }

            // load player data
            if (pl)
            {
                PlayerInfo playerData = pl.GetComponent<PlayerInfo>();

                // set player data
                playerData.CurrentHealth = data.pCurrentHealth;
                playerData.maxHealth = data.pMaxHealth;
                playerData.currentEnergy = data.pCurrentEnergy;
                playerData.maxEnergy = data.pMaxEnergy;

                // Clear and set ammo clips
                playerData.AmmoClips = new List<AmmoClip>();
                for (int i = 0; i < data.pClips.Count; i++)
                {
                    InventoryItem gunType = gunSO.Find(x => x.itemID == data.pClips[i].gunID);
                    playerData.AddAmmoClip(new AmmoClip(gunType, data.pClips[i].GetAmount()));
                }
                if (playerData.AmmoClips.Count != 0)
                    playerData.ChangeGun(gunSO.Find(x => x.itemID == data.pGun));


            }
            else
            {
                Debug.LogWarning("Player data wasn't found during loading. Player will not be affected.");
            }

            // Notify successful load.
            Debug.Log("Player Data Loaded");
            loadingData = false;
        }
    }

    /// <summary>
    /// Holds all the GameObject data for a specific room
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    /// 
    /// public var      desc
    /// rName           name of the room
    /// rPickups        list of the pickups in the room
    /// rTriggers       list of the triggers in the room
    /// rEnemies        list of the enemies in the room
    /// rEnvironments   list of the environmental units in the room
    /// rEvents         list of the events in the room
    /// rSwitches       list of the switches in the room
    /// rPuzzles        list of the puzzles in the room
    /// rDoor           list of the door states in a room
    /// rGatedSpawns    list of the gated spawns in a room
    /// 
    [System.Serializable]
    private class RoomData
    {
        public string rName;
        public List<SavePickups> rPickups = new List<SavePickups>();
        public List<SaveTriggers> rTriggers = new List<SaveTriggers>();
        public List<SaveEnemies> rEnemies = new List<SaveEnemies>();
        public List<SaveEnvironmentals> rEnvironments = new List<SaveEnvironmentals>();
        public List<SaveEvents> rEvents = new List<SaveEvents>();
        public List<SaveSwitches> rSwitches = new List<SaveSwitches>();
        public List<SavePuzzles> rPuzzles = new List<SavePuzzles>();
        public List<SaveDoors> rDoors = new List<SaveDoors>();
        public List<SaveGatedSpawns> rGatedSpawns = new List<SaveGatedSpawns>();

        /// <summary>
        /// Go through all the GameObjects in the room and record important data
        /// </summary>
        /// <returns>successful save</returns>
        /// 
        /// 2021-06-11  TH  Initial Implementation
        /// 2021-07-12  JH  Add environmental object case
        /// 2021-07-13  JH  Add event object case
        /// 2021-07-14  JH  Add case for switch and puzzles
        /// 2021-07-22  JH  Add case for gated spawn
        /// 
        public bool SaveGameObjects()
        {
            Transform roomObj = GameObject.Find("Level").transform.Find(rName).transform.Find("GameObjects");

            foreach(Transform o in roomObj)
            {
                switch (o.tag)
                {
                    case "Pickup":
                        SavePickups sO = new SavePickups();
                        sO.Save(o.gameObject);
                        sO.type = SaveObjectType.pickup;
                        rPickups.Add(sO);
                        break;
                    case "Trigger":
                        SaveTriggers sT = new SaveTriggers();
                        sT.Save(o.gameObject);
                        sT.type = SaveObjectType.trigger;
                        rTriggers.Add(sT);
                        break;
                    case "Enemy":
                        SaveEnemies sE = new SaveEnemies();
                        sE.Save(o.gameObject);
                        sE.type = SaveObjectType.enemy;
                        rEnemies.Add(sE);
                        break;
                    case "Environmental":
                        SaveEnvironmentals sEnv = new SaveEnvironmentals();
                        sEnv.Save(o.gameObject);
                        sEnv.type = SaveObjectType.environmental;
                        rEnvironments.Add(sEnv);
                        break;
                    case "Event":
                        SaveEvents sEvt = new SaveEvents();
                        sEvt.Save(o.gameObject);
                        sEvt.type = SaveObjectType.events;
                        rEvents.Add(sEvt);
                        break;
                    case "Switch":
                        SaveSwitches sS = new SaveSwitches();
                        sS.Save(o.gameObject);
                        sS.type = SaveObjectType.switches;
                        rSwitches.Add(sS);
                        break;
                    case "Puzzle":
                        SavePuzzles sP = new SavePuzzles();
                        sP.Save(o.gameObject);
                        sP.type = SaveObjectType.puzzle;
                        rPuzzles.Add(sP);
                        break;
                    case "Gated Spawn":
                        SaveGatedSpawns sGS = new SaveGatedSpawns();
                        sGS.Save(o.gameObject);
                        sGS.type = SaveObjectType.gatedspawn;
                        rGatedSpawns.Add(sGS);
                        break;
                    case "Indestructible":
                        //Debug.Log("Skip: " + o.name);
                        break;
                    default:
                        Debug.LogWarning(o.name + " has no applicable tag." +
                            "It may not have an appropriate tag or is incorrectly in the GameObjects parent.");
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Go through doors and save their state
        /// </summary>
        /// 
        /// 2021-07-13  JH  Initial Implementation
        /// 
        public void SaveRoomDoors()
        {
            Transform roomObj = GameObject.Find("Level").transform.Find(rName).transform.Find("brush");
            if (roomObj)
            {
                foreach (Transform o in roomObj)
                {
                    switch (o.tag)
                    {
                        case "Door":
                            SaveDoors sDoor = new SaveDoors();
                            sDoor.Save(o.gameObject);
                            sDoor.type = SaveObjectType.door;
                            rDoors.Add(sDoor);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Holds data specific to the Pickup GameObjects
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// oPos            position of the pickup
    /// oRot            rotation of the pickup
    /// isActive        if the GameObject is active in the scene
    /// itmName         item name of the pickup
    /// amount          quantity of item given when picked up
    /// 
    [System.Serializable]
    private class SavePickups
    {
        public SaveObjectType type;
        public Vector3 oPos;
        public Quaternion oRot;
        public bool isActive;
        public string itmName;
        public int amount;

        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-06-11  TH  Initial Implementation
        /// 2021-07-20  JH  Now save item name rather than the item itself
        /// 
        public void Save(GameObject gO)
        {
            ItemInteract invComp = gO.GetComponent<ItemInteract>();

            oPos = gO.transform.position;
            oRot = gO.transform.rotation;
            isActive = gO.gameObject.activeInHierarchy;
            itmName = invComp.item.itemName;
            amount = invComp.amount;
        }      
    }

    /// <summary>
    /// Holds data specific to the Trigger GameObjects
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// oPos            position of the trigger
    /// oRot            rotation of the trigger
    /// isActive        if the GameObject is active in the scene
    /// activated       if this trigger has already been activated
    /// name            name of this trigger
    /// persistance     persistance of trigger, whether to reactivate across loads
    /// tType           trigger type of the trigger
    /// objectTypes     type of object to spawn (if trigger is a spawn type)
    /// pickUpPosition  list of positions for pickups in the spawner
    /// envPosition     list of positions for environmentals in the spawner
    /// 
    [System.Serializable]
    private class SaveTriggers
    {
        public SaveObjectType type;
        public Vector3 oPos;
        public Quaternion oRot;
        public bool isActive;
        public bool activated;
        public string name;
        public TriggerManager.TriggerPersist persistance;
        public TriggerManager.TriggerType tType;
        public List<SaveObjectType> objectTypes = new List<SaveObjectType>();
        public List<Vector3> pickUpPosition = new List<Vector3>();
        public List<Vector3> envPosition = new List<Vector3>();

        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-06-11  TH  Initial Implementation
        /// 2021-07-21  JH  Add new data to save
        /// 
        public void Save(GameObject gO)
        {
            TriggerManager triComp = gO.GetComponent<TriggerManager>();

            oPos = gO.transform.position;
            oRot = gO.transform.rotation;
            isActive = gO.gameObject.activeInHierarchy;
            activated = triComp.isActivated;
            name = triComp.name;
            persistance = triComp.persistance;
            tType = triComp.type;

            if (tType == TriggerManager.TriggerType.spawn)
            {
                objectTypes.Clear();
                pickUpPosition.Clear();
                envPosition.Clear();
                foreach (GameObject gameObject in triComp.spawners)
                {
                    if (gameObject != null && gameObject.CompareTag("Pickup"))
                    {
                        pickUpPosition.Add(gameObject.transform.position);
                        objectTypes.Add(SaveObjectType.pickup);
                    }
                    else if (gameObject != null && gameObject.CompareTag("Environmental"))
                    {
                        envPosition.Add(gameObject.transform.position);
                        objectTypes.Add(SaveObjectType.environmental);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Holds data specific to the Event GameObjects
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// oPos            position of the event
    /// oRot            rotation of the event
    /// isActive        if the GameObject is active in the scene
    /// state           state of the event
    /// activated       if event is activated
    /// name            name of the event
    /// 
    [System.Serializable]
    private class SaveEvents
    {
        public SaveObjectType type;
        public Vector3 oPos;
        public Quaternion oRot;
        public bool isActive;
        public State state;
        public bool activated;
        public string name;

        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-07-13  JH  Initial Implementation
        /// 
        public void Save(GameObject gO)
        {
            EventInteract eI = gO.transform.GetComponentInChildren<EventInteract>();

            oPos = gO.transform.position;
            oRot = gO.transform.rotation;
            isActive = gO.gameObject.activeInHierarchy;
            state = eI.InteractableState;
            activated = eI.isActivated;
            name = gO.name;
        }
    }

    /// <summary>
    /// Holds data specific to the Switch GameObjects
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// oPos            position of the switch
    /// oRot            rotation of the switch
    /// isActive        if the GameObject is active in the scene
    /// state           state of the switch
    /// activated       if switch is activated
    /// name            name of the switch
    /// 
    [System.Serializable]
    private class SaveSwitches
    {
        public SaveObjectType type;
        public Vector3 oPos;
        public Quaternion oRot;
        public bool isActive;
        public State state;
        public bool activated;
        public string name;

        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-07-14  JH  Initial Implementation
        /// 
        public void Save(GameObject gO)
        {
            SwitchInteract sI = gO.transform.GetComponentInChildren<SwitchInteract>();

            oPos = gO.transform.position;
            oRot = gO.transform.rotation;
            isActive = gO.gameObject.activeInHierarchy;
            state = sI.InteractableState;
            activated = sI.isActivated;
            name = gO.name;
        }
    }

    /// <summary>
    /// Holds data specific to the Puzzle GameObjects
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// oPos            position of the puzzle
    /// oRot            rotation of the puzzle
    /// isActive        if the GameObject is active in the scene
    /// pState          puzzle state of the event
    /// correct         determines if player solved the puzzle
    /// name            name of the puzzle
    /// 
    [System.Serializable]
    private class SavePuzzles
    {
        public SaveObjectType type;
        public Vector3 oPos;
        public Quaternion oRot;
        public bool isActive;
        public Puzzle.PuzzleStates pState;
        public bool correct;
        public string name;

        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-07-14  JH  Initial Implementation
        /// 
        public void Save(GameObject gO)
        {
            Puzzle puzzle = gO.transform.GetComponent<Puzzle>();

            oPos = gO.transform.position;
            oRot = gO.transform.rotation;
            isActive = gO.gameObject.activeInHierarchy;
            pState = puzzle.GetState();
            correct = puzzle.GetCorrect();
            name = gO.name;
        }
    }

    /// <summary>
    /// Holds data specific to the GatedSpawn GameObjects
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// isActive        if the GameObject is active in the scene
    /// isActivated     if the GatedSpawn was activated
    /// deactivate      if GatedSpawn is to be deactivated on completion
    /// name            name of the event
    /// objectTypes     type of object to spawn (if trigger is a spawn type)
    /// pickUpPosition  list of positions for pickups in the spawner
    /// envPosition     list of positions for environmentals in the spawner
    /// 
    [System.Serializable]
    private class SaveGatedSpawns
    {
        public SaveObjectType type;
        public bool isActive;
        public bool isActivated;
        public bool deactivate;
        public string name;
        public List<SaveObjectType> objectTypes = new List<SaveObjectType>();
        public List<Vector3> pickUpPosition = new List<Vector3>();
        public List<Vector3> envPosition = new List<Vector3>();

        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-07-21  JH  Initial Implementation
        /// 
        public void Save(GameObject gO)
        {
            GatedSpawn gSComp = gO.GetComponent<GatedSpawn>();

            name = gO.name;
            isActive = gO.gameObject.activeInHierarchy;
            isActivated = gSComp.isActivated;
            deactivate = gSComp.deactivateOnCompletion;

            objectTypes.Clear();
            pickUpPosition.Clear();
            envPosition.Clear();

            foreach (GameObject gameObject in gSComp.spawners)
            {
                if (gameObject != null && gameObject.CompareTag("Pickup"))
                {
                    pickUpPosition.Add(gameObject.transform.position);
                    objectTypes.Add(SaveObjectType.pickup);
                }
                else if (gameObject != null && gameObject.CompareTag("Environmental"))
                {
                    envPosition.Add(gameObject.transform.position);
                    objectTypes.Add(SaveObjectType.environmental);
                }
            }

        }
    }


    /// <summary>
    /// Holds data specific to the Enemy GameObjects
    /// </summary>
    /// 
    /// Author: Tyson Hoang (TH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// enemyData       enemy information
    /// oPos            position of the enemy
    /// oRot            rotation of the enemy
    /// isActive        if the GameObject is active in the scene
    /// cPoints         GameObject's patrol points in the room
    /// 
    [System.Serializable]
    private class SaveEnemies
    {
        public SaveObjectType type;
        public UnitData enemyData;
        public Vector3 oPos;        // oPos will store the enemy's START POSITION
        public Quaternion oRot;     // oRot will store the enemy's START ROTATION
        public bool isActive;
        public List<Transform> cPoints = new List<Transform>();

        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-06-11  TH  Initial Implementation
        /// 
        public void Save(GameObject gO)
        {
            UnitInfo nmeComp = gO.GetComponent<UnitInfo>();
            EnemyController controllerComp = gO.GetComponent<EnemyController>();
            Transform[] cPointsArray = controllerComp.cPointsAll;

            oPos = gO.transform.position;
            oRot = gO.transform.rotation;
            isActive = gO.gameObject.activeInHierarchy;
            enemyData = nmeComp.unitData;

            foreach (Transform c in cPointsArray)
            {
                cPoints.Add(c);
            }
        }
    }

    /// <summary>
    /// Holds data specific to the Environmental GameObjects
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// envData         environmental object information
    /// oPos            position of the environmental
    /// oRot            rotation of the environmental
    /// oScale          scale of the environmental
    /// isActive        if the GameObject is active in the scene
    /// name            name of the environmental
    /// activePart      if an active particle is on the object
    /// partTimer       time for timed particle allowed on the object
    /// 
    [System.Serializable]
    private class SaveEnvironmentals
    {
        public SaveObjectType type;
        public Vector3 oPos;        // oPos will store the environmental's START POSITION
        public Quaternion oRot;     // oRot will store the environmental's START ROTATION
        public Vector3 oScale;
        public bool isActive;
        public string name;
        public bool activePart;
        public float partTimer;


        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-07-12  JH  Initial Implementation
        /// 2021-07-20  JH  Add saving of particle related information
        /// 
        public void Save(GameObject gO)
        {
            EnvironmentInfo envComp = gO.GetComponent<EnvironmentInfo>();

            oPos = gO.transform.position;
            oRot = gO.transform.rotation;
            oScale = gO.transform.localScale;
            isActive = gO.gameObject.activeInHierarchy;
            name = envComp.unitData.name;
            activePart = envComp.activeParticle;
            partTimer = envComp.particleTimer;
        }
    }

    /// <summary>
    /// Holds data specific to doors
    /// </summary>
    /// 
    /// Author: Jacky Huynh (JH)
    /// 
    /// public var      desc
    /// type            GameObject type
    /// state           interactable state
    /// 
    [System.Serializable]
    private class SaveDoors
    {
        public SaveObjectType type;
        public State state;

        /// <summary>
        /// Save data to this class by accessing the appropriate Component
        /// </summary>
        /// <param name="gO">GameObject to save</param>
        /// 
        /// 2021-07-13  JH  Initial Implementation
        /// 
        public void Save(GameObject gO)
        {
            DoorInteract dGO = gO.GetComponentInChildren<DoorInteract>();

            state = dGO.InteractableState;
        }
    }

    /// <summary>
    /// The type of Object being saved
    /// </summary>
    /// 
    /// 2021-06-11  TH  Initial Implementation
    /// 2021-07-12  JH  Add environmental and door type
    /// 2021-07-14  JH  Add switch and puzzle types
    /// 2021-07-22  JH  Add gatedspawn puzzle type
    /// 
    public enum SaveObjectType { trigger, pickup, events, switches, puzzle, gatedspawn, enemy, environmental, savepoint, door}
}
