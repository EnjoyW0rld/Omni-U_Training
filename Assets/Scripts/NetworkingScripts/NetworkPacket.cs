using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Unity.Collections;
using UnityEngine;

public class NetworkPacket
{
    //private NativeList<byte> _packetList;
    private BinaryWriter _writer;
    private BinaryReader _reader;

    public NetworkPacket()
    {
        _writer = new BinaryWriter(new MemoryStream());
    }
    public NetworkPacket(NativeArray<byte> packetList)
    {
        _reader = new BinaryReader(new MemoryStream(packetList.ToArray()));
    }
    public NetworkPacket(byte[] pObject)
    {
        _reader = new BinaryReader(new MemoryStream(pObject));
    }
    ~NetworkPacket()
    {
        _writer.Dispose();
        _reader.Dispose();
    }

    public void WriteString(string pString) => _writer.Write(pString);
    public string ReadString() => _reader.ReadString();

    public void Write(ISerializable pMessage)
    {
        _writer.Write(pMessage.GetType().FullName);
        pMessage.Serialize(this);
    }
    public ISerializable Read()
    {
        Type type = Type.GetType(ReadString());
        ISerializable obj = (ISerializable)Activator.CreateInstance(type);
        obj.DeSerialize(this);
        return obj;
    }

    public NativeArray<byte> GetBytes()
    {
        NativeArray<byte> arr = new NativeArray<byte>(((MemoryStream)_writer.BaseStream).ToArray(), Allocator.Persistent);
        return arr;
    }
}
