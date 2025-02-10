using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;
using UnityEngine.SocialPlatforms.Impl;
using UnityEditor;

public class ServerBehaviour : MonoBehaviour
{
    public const int TEAM_NUMBER = 3;

    public static bool IsThisUserServer { get { return _instance != null; } }
    public static ServerBehaviour Instance { get { return _instance; } }
    private static ServerBehaviour _instance;

    private NetworkDriver _networkDriver;
    private NativeList<NetworkConnection> _connections;
    private Dictionary<int, NetworkConnection> _teamConnectionDict;
    private UserData[] _userDatas;
    private NetworkConnection _currentReadConnetion;

    private List<NetworkPacket> _packetsToSend;

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
        _packetsToSend = new List<NetworkPacket>();
        _userDatas = new UserData[TEAM_NUMBER];
        for (int i = 0; i < TEAM_NUMBER; i++)
        {
            _userDatas[i] = new UserData((uint)i + 1);
        }
        _teamConnectionDict = new Dictionary<int, NetworkConnection> {  {1, (NetworkConnection)default},
                                                                        {2, (NetworkConnection)default},
                                                                        {3, (NetworkConnection)default}};
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

    public void ScheduleMessage(NetworkPacket pPacket)
    {
        _packetsToSend.Add(pPacket);
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
        if (_packetsToSend.Count > 0)
        {
            for (int i = 0; i < _connections.Length; i++)
            {
                _networkDriver.BeginSend(NetworkPipeline.Null, _connections[i], out DataStreamWriter writer);
                writer.WriteBytes(_packetsToSend[0].GetBytes());
                _networkDriver.EndSend(writer);
            }
            _packetsToSend.RemoveAt(0);
        }
    }
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
                    print($"Is stream null - {stream.IsCreated} and has data - {stream.Length}");
                    NetworkPacket packet = new NetworkPacket(stream);
                    ISerializable data = packet.Read();
                    data.Use();
                    //uint number = stream.ReadUInt();
                    //Debug.Log($"Got {number} from client, adding 2 to it");

                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from the server");
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
    }

    private void OnDestroy()
    {
        if (_networkDriver.IsCreated)
        {
            _networkDriver.Dispose();
            _connections.Dispose();
        }
    }
    // ---------------------
    // GET FUNCTIONS
    // ---------------------

    public bool[] GetTeamNumbers()
    {
        bool[] arr = new bool[TEAM_NUMBER];
        for (int i = 0; i < TEAM_NUMBER; i++)
        {
            arr[i] = _teamConnectionDict[i + 1] == default;
            //teams[i] = _connections[i].TeamNum;
        }
        /*arr[0] = true;
        arr[1] = false;
        arr[2] = true;*/
        return arr;
    }
    public NetworkConnection GetCurrentConnection()
    {
        return _currentReadConnetion;
    }
}