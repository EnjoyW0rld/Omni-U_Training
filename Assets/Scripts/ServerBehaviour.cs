using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;

public class ServerBehaviour : MonoBehaviour
{
    NetworkDriver _networkDriver;
    NativeList<NetworkConnection> _connections;
    // Start is called before the first frame update
    void Start()
    {

        
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
        _networkDriver = NetworkDriver.Create(new WebSocketNetworkInterface());
        _connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        //var endpoint = NetworkEndpoint.Parse(NetworkEndpoint.AnyIpv4, 9000);
        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(9001);
        endpoint.Family = NetworkFamily.Ipv4;
        print(endpoint);
        if (_networkDriver.Bind(endpoint) != 0)
        {
            Debug.LogError("Failed to bind to port 7777");
            return;
        }
        _networkDriver.Listen();
    }

    private void OnDestroy()
    {
        if (_networkDriver.IsCreated)
        {
            _networkDriver.Dispose();
            _connections.Dispose();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!_networkDriver.IsCreated) return;
        _networkDriver.ScheduleUpdate().Complete();
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
        // Accepting new connections
        NetworkConnection connection;
        while ((connection = _networkDriver.Accept()) != default)
        {
            _connections.Add(connection);
            Debug.Log("Added new connection");
        }
        // Reading incoming data
        for (int i = 0; i < _connections.Length; i++)
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = _networkDriver.PopEventForConnection(_connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadUInt();
                    Debug.Log($"Got {number} from client, adding 2 to it");
                    number += 2;

                    _networkDriver.BeginSend(NetworkPipeline.Null, _connections[i], out DataStreamWriter writer);
                    writer.WriteUInt(number);
                    _networkDriver.EndSend(writer);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from the server");
                    _connections[i] = default;
                    break;
                }
            }
        }
    }
}
