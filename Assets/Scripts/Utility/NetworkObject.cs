using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkObject : ISerializable
{
    public abstract void DeSerialize(NetworkPacket pPacket);
    public NetworkPacket PackObject()
    {
        NetworkPacket pPacket = new NetworkPacket();
        pPacket.Write(this);
        return pPacket;
    }


    public abstract void Serialize(NetworkPacket pPacket);


    public abstract void Use();

}
public abstract class RCPNetworkObject : RCPBase, ISerializable
{
    public abstract void DeSerialize(NetworkPacket pPacket);


    public NetworkPacket PackObject()
    {
        NetworkPacket pPacket = new NetworkPacket();
        pPacket.Write(this);
        return pPacket;
    }


    public abstract void Serialize(NetworkPacket pPacket);


    public abstract void Use();
}