using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class ClientBehaviour : MonoBehaviour
{
    NetworkDriver _networkDriver;
    NetworkConnection _connection;
    // Start is called before the first frame update
    void Start()
    {
        _networkDriver = NetworkDriver.Create(new WebSocketNetworkInterface());
        
        //var endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(7777);
        //_connection = _networkDriver.Connect(endpoint);
    }
    public void MakeConnection(string adress)
    {
        if (!_connection.IsCreated)
        {
            NetworkEndpoint endpoint = NetworkEndpoint.Parse(adress, (ushort)9001);
            endpoint.Family = NetworkFamily.Ipv4;
            _connection = _networkDriver.Connect(endpoint);
        }
    }
    private void OnDestroy()
    {
        _networkDriver.Dispose();
    }
    // Update is called once per frame
    void Update()
    {
        _networkDriver.ScheduleUpdate().Complete();
        if (!_connection.IsCreated)
        {
            return;
        }

        DataStreamReader streamReader;
        NetworkEvent.Type cmd;
        while ((cmd = _connection.PopEvent(_networkDriver, out streamReader)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to server!");
                uint value = 1;
                _networkDriver.BeginSend(_connection, out var writer);
                writer.WriteUInt(value);
                _networkDriver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = streamReader.ReadUInt();
                Debug.Log($"Got the value {value} back");

                _connection.Disconnect(_networkDriver);
                _connection = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                _connection = default;
            }
        }
    }
}
