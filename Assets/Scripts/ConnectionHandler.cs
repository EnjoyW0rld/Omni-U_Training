using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectionHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    private ClientBehaviour _clientBehaviour;
    private void Start()
    {
        if (_inputField == null) Debug.LogError("Input field is null!");
        _clientBehaviour = FindObjectOfType<ClientBehaviour>();
    }

    public void TryConnect()
    {
        _clientBehaviour.MakeConnection(_inputField.text);
    }
}
