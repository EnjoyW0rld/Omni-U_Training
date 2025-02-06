using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSceneManager :  ISerializable
{
    public string _sceneName;
    public void DeSerialize(NetworkPacket pPacket)
    {
        _sceneName = pPacket.ReadString();
    }

    public void Serialize(NetworkPacket pPacket)
    {
       pPacket.WriteString(_sceneName);
    }
    public void Use()
    {
        SimpleSceneManager.ChangeScene(_sceneName);
    }
}