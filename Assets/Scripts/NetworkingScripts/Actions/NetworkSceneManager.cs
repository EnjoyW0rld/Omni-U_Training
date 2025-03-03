using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSceneManager : NetworkObject
{
    public string _sceneName;
    public override void DeSerialize(NetworkPacket pPacket)
    {
        _sceneName = pPacket.ReadString();
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteString(_sceneName);
    }
    public override void Use()
    {
        SimpleSceneManager.ChangeScene(_sceneName);
    }
}