using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [Header("Network prefabs")]
    [SerializeField] private ClientBehaviour _clientPrefab;
    [SerializeField] private ServerBehaviour _serverPrefab;

    [SerializeField] private TMP_InputField _ipInputField;
    [Header("Client prefabs")]
    [SerializeField] private GameObject _teamChooseScreen;
    [SerializeField] private GameObject _connectionScreen;

    private ClientBehaviour _createdClientPrefab;
    private ServerBehaviour _createdServerPrefab;


    public void CreateAndConnectClient()
    {
        if (ClientBehaviour.Instance == null)
        {
            _createdClientPrefab = Instantiate(_clientPrefab);
            ClientBehaviour.Instance.OnConnected.AddListener(ChangeToTeamScreen);
        }
        StartCoroutine(DoNextTick(() => ClientBehaviour.Instance.MakeConnection(_ipInputField.text)));
    }
    public void CreateServer()
    {
        if (ServerBehaviour.Instance == null) _createdServerPrefab = Instantiate(_serverPrefab);
        ServerBehaviour.Instance.StartServer();
    }
    public static IEnumerator DoNextTick(Action action)
    {
        yield return null;
        print("Invoking action");
        action?.Invoke();
    }
    private void OnDestroy()
    {
        if (ClientBehaviour.Instance != null)
            ClientBehaviour.Instance.OnConnected.RemoveListener(ChangeToTeamScreen);
    }
    private void ChangeToTeamScreen()
    {
        _teamChooseScreen.SetActive(true);
        _connectionScreen.SetActive(false);
    }
}
