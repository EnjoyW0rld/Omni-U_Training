using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class ClientBehaviour : MonoBehaviour
{
    private static ClientBehaviour _instance;
    public static ClientBehaviour Instance { get { return _instance; } }

    private NetworkDriver _networkDriver;
    private NetworkConnection _connection;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        _networkDriver = NetworkDriver.Create(new WebSocketNetworkInterface());
    }
    /// <summary>
    /// Connect to the server passing adress
    /// </summary>
    /// <param name="adress"></param>
    public void MakeConnection(string adress)
    {
        print("Make connection called");
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
                NativeArray<byte> data = new NativeArray<byte>();
                NetworkPacket packet = new NetworkPacket(data);
                print(data.Length);
                ISerializable obj = packet.Read();
                print(obj.GetType().FullName);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                _connection = default;
            }
        }
    }
}
