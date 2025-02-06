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

    private ClientBehaviour _createdClientPrefab;
    private ServerBehaviour _createdServerPrefab;


    public void CreateAndConnectClient()
    {
        if (ClientBehaviour.Instance == null)
        {
            _createdClientPrefab = Instantiate(_clientPrefab);
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
}
