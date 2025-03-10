using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{
    [SerializeField] private GameObject _PCScreen;
    [SerializeField] private GameObject _mainRoom;
    private Dictionary<Room, GameObject> _allRooms;
    public enum Room { PCRoom = 0, MainRoom = 1 }
    private int _currentRoom = 0;

    private void Start()
    {
        _allRooms = new Dictionary<Room, GameObject> {
            { Room.PCRoom, _PCScreen },
            { Room.MainRoom,_mainRoom} };
    }
    public void SwitchRoom(Room pNextRoom)
    {
        foreach (var item in Enum.GetValues(typeof(Room)))
        {
            _allRooms[(Room)item].SetActive((Room)item == pNextRoom);
        }
        _currentRoom = (int)pNextRoom;
    }
    public void SwitchRoom(int pNextRoom) => SwitchRoom((Room)pNextRoom);
    public void GoToNextRoom()
    {
        _currentRoom++;
        if (_currentRoom == _allRooms.Count)
        {
            _currentRoom = 1;
        }
        SwitchRoom(_currentRoom);
    }
}

