using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine.Events;
using static Unity.Burst.Intrinsics.X86.Avx;

/// <summary>
/// Script for client side that holds connection with the host as well as sends/recieves messages
/// Is singleton
/// </summary>
public class ClientBehaviour : MonoBehaviour
{
    private static ClientBehaviour _instance;
    public static ClientBehaviour Instance { get { return _instance; } }

    private NetworkDriver _networkDriver;
    private NetworkConnection _connection;


    private List<NetworkPacket> _scheduledPackets;

    private Fragmentation _fragmenter;
    private UserData _thisData;

    private int _teamNubmer;
    public bool IsConnected { get; private set; }
    public int TeamNubmer { get { return _teamNubmer; } }

    public UnityEvent OnConnected;
    public UnityEvent OnGameStarted;

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
        _scheduledPackets = new List<NetworkPacket>();
        Application.runInBackground = true;
        if (OnGameStarted == null) OnGameStarted = new UnityEvent();
    }
    /// <summary>
    /// Connect to the server passing adress, uses 9001 port by default
    /// </summary>
    /// <param name="adress"></param>
    public void MakeConnection(string adress)
    {
        print("Make connection called");
        if (!IsConnected)
        {
            NetworkEndpoint endpoint = NetworkEndpoint.Parse(adress, (ushort)9001);
            endpoint.Family = NetworkFamily.Ipv4;
            _connection = _networkDriver.Connect(endpoint);
        }
    }
    public void TryMakeConnection(string adress)
    {
        NetworkEndpoint endpoint = NetworkEndpoint.Parse(adress, (ushort)9001);
        endpoint.Family = NetworkFamily.Ipv4;
        _networkDriver.Connect(endpoint);
    }
    private void OnDestroy()
    {
        _networkDriver.Dispose();
    }
    // Update is called once per frame
    void Update()
    {
        _networkDriver.ScheduleUpdate().Complete();
        ReadConnectRequests();
        if (!IsConnected)
        {
            return;
        }
        ReadData();
        SendData();
    }
    public void SchedulePackage(NetworkPacket pPacket)
    {
        if (!_connection.IsCreated) return;
        print("Package successfully scheduled");
        _scheduledPackets.Add(pPacket);
    }
    private void SendData()
    {
        if (_scheduledPackets == null || _scheduledPackets.Count == 0) return;
        for (int i = 0; i < _scheduledPackets.Count; i++)
        {
            _networkDriver.BeginSend(NetworkPipeline.Null, _connection, out DataStreamWriter writer);
            writer.WriteBytes(_scheduledPackets[i].GetNativeArrayBytes());
            _networkDriver.EndSend(writer);
        }
        _scheduledPackets.Clear();
    }
    private void ReadConnectRequests()
    {
        if (!IsConnected)
        {
            NetworkEvent.Type cmd;
            while ((cmd = _networkDriver.PopEvent(out NetworkConnection connection, out DataStreamReader dataReader)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    _connection = connection;
                    IsConnected = true;
                    OnConnected?.Invoke();
                }

            }
            return;
        }
    }
    private void HandleDisconnect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        _instance = null;
        Destroy(gameObject);
    }
    private void ReadData()
    {
        DataStreamReader streamReader;
        NetworkEvent.Type cmd;

        while ((cmd = _connection.PopEvent(_networkDriver, out streamReader)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to server!");
                OnConnected?.Invoke();
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                print("recieved !");
                NetworkPacket pPacket = new NetworkPacket(streamReader);
                ISerializable data = pPacket.Read();
                //Debug.Log($"Managed to read package {data.GetType().FullName} and we red {fragBytesRecieved}");
                if (data is FragSetting)
                {
                    DebugTextWriter.WriteOnScreen($"Recieved fragSettings");
                    _fragmenter = new Fragmentation();
                    _fragmenter.Initialize(data as FragSetting);
                    Debug.Log($"Initialized fragmenter");
                }
                else if (data is FragBytes)
                {
                    DebugTextWriter.WriteOnScreen($"Recieved frag bytes");
                    if (_fragmenter.TryDefragment(data as FragBytes))
                    {
                        _fragmenter = null;
                    }

                }
                else
                {
                    data.Use();
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                _connection = default;
                HandleDisconnect();
            }
        }
    }

    [MyRCP]
    public void StartGameForClient()
    {
        OnGameStarted?.Invoke();
        SimpleSceneManager.ChangeScene("PlayerScene");
    }

    public void UpdateUserData(UserData pData)
    {
        _thisData = pData;
        PC_UI pcui = ReferenceHandler.GetObject<PC_UI>(true);
        var emails = pData.GetEmails();
        Debug.Log("Updating user data");
        for (int i = 0; i < emails.Length; i++)
        {
            pcui.AddToArchive(emails[i], false);
        }
    }
    public void AssignTeam(int pTeamID)
    {
        _teamNubmer = pTeamID;
        ReferenceHandler.GetObject<TeamSelection>(true).ShowStandBy();
    }
}
