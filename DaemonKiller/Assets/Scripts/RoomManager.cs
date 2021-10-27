using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Overlooks each Game Room in the scene, and their GameObjects.
/// Game Rooms are individual areas in the scene that contain enemies, items, models, and brushes. They're grouped under "Level" in the game scene.
/// </summary>
/// 
/// Author: Tyson Hoang (TH), Jacky Huynh (JH)
/// private var     desc
/// allRooms        Holds all the Game Rooms in the Scene
/// currentRoom     The Game Room that should be active because the player is in it
/// saveMan         save manager used to save rooms
/// 
public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> allRooms;
    [SerializeField] private GameObject currentRoom;

    private SaveDataManager saveMan;

    /// <summary>
    /// Get/Set functions for currentRoom variable
    /// </summary>
    /// 
    /// 2021-05-21  TH  Initial Implementation
    /// 
    public GameObject CurrentRoom {
        get { return currentRoom; } set { currentRoom = value; } 
    }

    /// <summary>
    /// Fetch the Game Rooms and get components
    /// </summary>
    /// 
    /// 2021-05-21  TH  Initial Implementation
    /// 2021-07-12  JH  Add referencing of save manager
    /// 
    void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Scene Manager"))
            saveMan = GameObject.FindGameObjectWithTag("Scene Manager").GetComponent<SaveDataManager>();

        allRooms = new List<GameObject>();

        GameObject lvlObj = GameObject.Find("Level");

        if (lvlObj != null)
        {           
            foreach (Transform child in lvlObj.transform)
            {
                allRooms.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }

            CurrentRoom = allRooms[0];
            CurrentRoom.SetActive(true);
        }
        else
            Debug.LogWarning("Room Manager failed to find the Level GameObject.");
    }

    /// <summary>
    /// Sets a room in allRooms[] to active or inactive
    /// </summary>
    /// <param name="newRoom">The room to modify</param>
    /// <param name="newActive">Set active/inactive</param>
    /// 
    /// 2021-05-21  TH  Initial Implementation
    /// 2021-07-12  JH  Add saving current room 
    /// 
    public void SetRoomActive(GameObject newRoom, bool newActive)
    {
        if (newActive && saveMan)
        {
            // save the room state before swapping to the next room
            saveMan.SaveCurrentRoomData(); 
        }
        int index = allRooms.FindIndex(x => x.GetInstanceID() == newRoom.GetInstanceID());
        allRooms[index].SetActive(newActive);
    }

    /// <summary>
    /// Sets a room as the currentRoom
    /// </summary>
    /// <param name="newRoom">The room to set</param>
    /// 
    /// 2021-05-21  TH  Initial Implementation
    /// 
    public void SetRoomCurrent(GameObject newRoom)
    {
        if (allRooms.Exists(x => x.GetInstanceID() == newRoom.GetInstanceID()))
            CurrentRoom = newRoom;
        else
            Debug.LogError(newRoom.name + " is not in the Room List.");
    }

    /// <summary>
    /// Sets all rooms active
    /// </summary>
    /// 
    /// 2021-07-12  JH  Initial Implementation
    /// 
    public void SetRoomsActive()
    {
        foreach (GameObject room in allRooms)
        {
            room.SetActive(true);
        }
    }

    /// <summary>
    /// Sets all rooms inactive
    /// </summary>
    /// 
    /// 2021-07-12  JH  Initial Implementation
    /// 
    public void SetRoomsInactive()
    {
        foreach (GameObject room in allRooms)
        {
            room.SetActive(false);
        }
    }
}
