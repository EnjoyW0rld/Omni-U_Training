using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelection : MonoBehaviour, ISerializable
{
    [SerializeField] private GameObject[] _teamButtons;

    public void DeSerialize(NetworkPacket pPacket)
    {
        throw new System.NotImplementedException();
    }

    public void Serialize(NetworkPacket pPacket)
    {

    }

    public void Use()
    {
        if (ServerBehaviour.Instance != null)
        {
            MasterUse();
        }
        else
        {

        }
    }
    private void MasterUse()
    {
        NetworkPacket teamsInUsePacket = new NetworkPacket();
        teamsInUsePacket.Write(this);

        ServerBehaviour.Instance.ScheduleMessage(teamsInUsePacket);
    }
}
