using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IPWriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private void Start()
    {
        //_text.text = ServerBehaviour.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211);
    }
}
