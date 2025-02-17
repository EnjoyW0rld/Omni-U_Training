using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmailingHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _InputField;
    public void SendData()
    {

    }
    
}
public class EmailingContainer : RCPBase, ISerializable
{
    public string Email;
    public void DeSerialize(NetworkPacket pPacket)
    {
        Email = pPacket.ReadString();
    }

    public void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteString(Email);
    }

    public void Use()
    {
        GameObject.FindObjectOfType<EmailingHandler>();
    }
}
