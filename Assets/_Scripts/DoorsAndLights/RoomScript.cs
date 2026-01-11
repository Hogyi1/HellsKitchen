using System.Collections.Generic;
using UnityEngine;

public class RoomScript:MonoBehaviour
{
    public List<DoorManager> doors;

    void OnTriggerEnter(Collider other)
    {
        foreach(DoorManager door in doors)
        {
            door.OnRoomEnter(other);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        foreach (DoorManager door in doors) { door.OnRoomExit(other); }
    }
}
