using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{
    [SerializeField] private GameObject _PCScreen;
    [SerializeField] private GameObject _officeRoom1;
    [SerializeField] private GameObject _officeRoom2;
    private Dictionary<Room, GameObject> _allRooms;
    public enum Room { PCRoom = 0, OfficeRoom1 = 1, OfficeRoom2 = 2 }
    [SerializeField] private int _currentRoom = 0;

    private void Start()
    {
        _allRooms = new Dictionary<Room, GameObject> {
            { Room.PCRoom, _PCScreen },
            { Room.OfficeRoom1,_officeRoom1},
            { Room.OfficeRoom2,_officeRoom2}
            };
        SwitchRoom(_currentRoom);
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
    public void GoToPrevRoom()
    {
        _currentRoom--;
        if (_currentRoom < 1)
        {
            _currentRoom = _allRooms.Count - 1;
        }
        SwitchRoom(_currentRoom);
    }
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

