using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public struct UserData
{
    private NetworkConnection _connection;
    private uint _teamNum;

    public bool IsCreated { get { return _connection.IsCreated; } }
    public NetworkConnection Connection { get { return _connection; } }
    public uint TeamNum { get { return _teamNum; } }

    public UserData(NetworkConnection pConnection, uint pTeamNum)
    {
        _connection = pConnection;
        _teamNum = pTeamNum;
    }
}
