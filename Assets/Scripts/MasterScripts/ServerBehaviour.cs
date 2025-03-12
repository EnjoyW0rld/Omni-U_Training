using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;
using System.Linq;
using Unity.Mathematics;

public class ServerBehaviour : MonoBehaviour
{
    public const int TEAMS_COUNT = 3;

    public static bool IsThisUserServer { get { return _instance != null; } }
    public static ServerBehaviour Instance { get { return _instance; } }
    private static ServerBehaviour _instance;

    private NetworkDriver _networkDriver;
    private NativeList<NetworkConnection> _connections;
    private Dictionary<int, NetworkConnection> _teamConnectionDict;
    private UserData[] _userDatas;
    private NetworkConnection _currentReadConnetion;

    private List<ScheduledMessage> _packetsToSend;

    public int ConnectionsCount { get { return _connections.Length; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Initializing arrays
        _packetsToSend = new List<ScheduledMessage>();
        _userDatas = new UserData[TEAMS_COUNT];
        for (int i = 0; i < TEAMS_COUNT; i++)
        {
            _userDatas[i] = new UserData((uint)i + 1);
        }
        _teamConnectionDict = new Dictionary<int, NetworkConnection> {  {1, (NetworkConnection)default},
                                                                        {2, (NetworkConnection)default},
                                                                        {3, (NetworkConnection)default}};
    }
    private void Start()
    {
        Application.runInBackground = true;
    }
    /// <summary>
    /// Starts server and automatically changes scene to MasterScreen
    /// </summary>
    public void StartServer()
    {
        if (_networkDriver.IsCreated)
        {
            Debug.Log($"Server is already created, is running - {_networkDriver.Listening}");
            return;
        }
        _networkDriver = NetworkDriver.Create(new WebSocketNetworkInterface());
        _connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(9001);
        endpoint.Family = NetworkFamily.Ipv4;

        print(endpoint);
        if (_networkDriver.Bind(endpoint) != 0)
        {
            Debug.LogError("Failed to bind to port 7777");
            return;
        }
        _networkDriver.Listen();
        StartCoroutine(MenuHandler.DoNextTick(() => SimpleSceneManager.ChangeScene("MasterScreen")));
    }


    private void RemoveFaultyConnections()
    {
        // Removing old connections
        for (int i = 0; i < _connections.Length; i++)
        {
            if (!_connections[i].IsCreated)
            {
                _connections.RemoveAtSwapBack(i);
                i--;
                Debug.Log("Removed connection");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!_networkDriver.IsCreated) return;
        _networkDriver.ScheduleUpdate().Complete();
        RemoveFaultyConnections();
        AcceptIncomingConnections();
        ReadIncomingData();
        SendData();
    }

    public void ScheduleMessage(NetworkPacket pPacket, NetworkConnection pConn)
    {
        _packetsToSend.Add(new ScheduledMessage(pPacket, pConn));
    }
    /// <summary>
    /// If you not specify team to whom you send, it will automatically send it to everyone
    /// </summary>
    /// <param name="pPacket"></param>
    /// <param name="pTeamID"></param>
    public void ScheduleMessage(NetworkPacket pPacket, int pTeamID = -1)
    {
        _packetsToSend.Add(new ScheduledMessage(pPacket, pTeamID));
    }

    private void AcceptIncomingConnections()
    {
        // Accepting new connections
        NetworkConnection connection;
        while ((connection = _networkDriver.Accept()) != default)
        {
            //UserData newUser = new UserData(connection, (uint)_connections.Length + 1);
            _connections.Add(connection);
            Debug.Log("Added new connection");
        }
    }
    private void SendData()
    {
        if (_packetsToSend.Count == 0) return;

        int packetsSent = 0;
        //Sends data to every connection
        for (int i = 0; i < _packetsToSend.Count; i++)
        {
            if (_packetsToSend[i].IsSendToAll)
            {
                for (int t = 0; t < _connections.Length; t++)
                {

                    Debug.Log($"{_networkDriver.GetEventQueueSizeForConnection(_connections[t])} - Queeu for connection");
                    if (!_connections[t].IsCreated || _connections[t] == default) continue;
                    _networkDriver.BeginSend(NetworkPipeline.Null, _connections[t], out DataStreamWriter dataWriter);
                    dataWriter.WriteBytes(_packetsToSend[i].Packet.GetNativeArrayBytes());
                    _networkDriver.EndSend(dataWriter);

                }
                packetsSent++;
                if (packetsSent > 20) break;
                continue;
            }

            // Setting NetworkConnection based on the values in the current packet
            NetworkConnection conn;
            if (_packetsToSend[i].TeamID != -1) conn = _teamConnectionDict[_packetsToSend[i].TeamID];
            else conn = _packetsToSend[i].Connection;

            _networkDriver.BeginSend(NetworkPipeline.Null, conn, out DataStreamWriter writer);
            writer.WriteBytes(_packetsToSend[i].Packet.GetNativeArrayBytes());
            _networkDriver.EndSend(writer);
        }
        if (packetsSent != 0)
        {
            _packetsToSend.RemoveRange(0, packetsSent);
            Debug.Log($"Sent {packetsSent} packets, cleaning part of it");
        }
        else
        {
            _packetsToSend.Clear();
        }




    }
    /// <summary>
    /// Dispatches and handles all the incoming messages
    /// </summary>
    private void ReadIncomingData()
    {
        // Reading incoming data
        for (int i = 0; i < _connections.Length; i++)
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = _networkDriver.PopEventForConnection(_connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    _currentReadConnetion = _connections[i];
                    NetworkPacket packet = new NetworkPacket(stream);
                    ISerializable data = packet.Read();
                    data.Use();

                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from the server");
                    for (int k = 1; k < _teamConnectionDict.Count + 1; k++)
                    {
                        if (_teamConnectionDict[k] == _connections[i])
                        {
                            _teamConnectionDict[k] = default;
                        }
                    }
                    _connections[i] = default;
                    break;

                }
            }
        }
        _currentReadConnetion = default;
    }
    public void AssignConnection(NetworkConnection pConn, int pTeamId)
    {
        _teamConnectionDict[pTeamId] = pConn;
        Debug.Log("Assigned connection for team " + pTeamId);

        TeamSelectionContainer cont = new TeamSelectionContainer(TeamSelectionContainer.Instruction.RCP);
        cont.RCPName = "AssignTeamNumber";
        NetworkPacket packet = new NetworkPacket();
        packet.Write(cont);
        packet.WriteInt(pTeamId);
        ScheduleMessage(packet, pTeamId);
    }

    private void OnDestroy()
    {
        if (_networkDriver.IsCreated)
        {
            _networkDriver.Dispose();
            _connections.Dispose();
        }
    }
    public static string GetLocalIPv4(NetworkInterfaceType _type)
    {
        string output = "";
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        output = ip.Address.ToString();
                    }
                }
            }
        }
        return output;
    }
    public static IPAddress GetDefaultGateway()
    {
        var gateway_address = NetworkInterface.GetAllNetworkInterfaces()
            .Where(e => e.OperationalStatus == OperationalStatus.Up)
            .SelectMany(e => e.GetIPProperties().GatewayAddresses)
            .FirstOrDefault();
        if (gateway_address == null) return null;
        return gateway_address.Address;
    }

    public void AddEmailToTeamArchive(UserData.TextData pTextData, int pTeamID)
    {
        print(pTeamID + " team id");
        _userDatas[pTeamID - 1].AddEmail(pTextData);
    }
    public void SendFragmentedData(ISerializable pObj)
    {
        //DebugTextWriter.WriteOnScreen($"ref object exists - {pObj.PackObject() != null}");
        NetworkPacket[] array;
        byte[] bytes = pObj.PackObject().GetBytes();
        //DebugTextWriter.WriteOnScreen($"{bytes.Length} - is bytes we plan to write");
        array = Fragmentation.DoFragmentation(bytes, out FragSetting fragSettings);
        ScheduleMessage(fragSettings.PackObject());

        for (int i = 0; i < array.Length; i++)
        {
            ScheduleMessage(array[i]);
        }
    }


    // ---------------------
    // GET FUNCTIONS
    // ---------------------

    public bool[] GetTeamNumbers()
    {
        bool[] arr = new bool[TEAMS_COUNT];
        for (int i = 0; i < TEAMS_COUNT; i++)
        {
            arr[i] = _teamConnectionDict[i + 1] == default;
        }
        return arr;
    }
    public int GetTeamByConnection(NetworkConnection pConn)
    {
        if (pConn == default) return -1;
        foreach (var pair in _teamConnectionDict)
        {
            if (pair.Value == pConn) return pair.Key;
        }
        return -1;
    }

    public int GetCurrentConnectionTeam() => GetTeamByConnection(GetCurrentConnection());
    public NetworkConnection GetCurrentConnection()
    {
        return _currentReadConnetion;
    }

    public UserData GetCurrentUserData() => GetUserDataByID(GetCurrentConnectionTeam() - 1);
    /// <summary>
    /// ID should be your team number "-1" for this function to work
    /// </summary>
    /// <param name="pId"></param>
    /// <returns></returns>
    public UserData GetUserDataByID(int pId)
    {
        return _userDatas[pId];
    }
    [MyRCP]
    public void SendUserData()
    {
        Debug.Log("Sending user data");
        int teamNum = GetTeamByConnection(GetCurrentConnection());
        Debug.Log($"For team number {GetUserDataByID(teamNum -1).TeamNum} amount texts is {GetUserDataByID(teamNum -1).GetEmailsCount()}");
        ScheduleMessage(GetUserDataByID(teamNum - 1).PackObject(), teamNum);
    }

    private struct ScheduledMessage
    {
        public NetworkPacket Packet;
        public NetworkConnection Connection;
        public int TeamID;
        public bool IsSendToAll { get { return TeamID == -1 && Connection == default; } }

        public ScheduledMessage(NetworkPacket pPacket, NetworkConnection pConn)
        {
            Packet = pPacket;
            Connection = pConn;
            TeamID = -1;
        }
        public ScheduledMessage(NetworkPacket pPacket, int pTeamID = -1)
        {
            Packet = pPacket;
            Connection = default;
            TeamID = pTeamID;
        }

    }
}