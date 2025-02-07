using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Wrapper for the messages to be sent through the network
/// </summary>
public class NetworkPacket
{
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
    public void WriteInt(int pInt) => _writer.Write(pInt);
    public void WriteIntArray(int[] pIntArr)
    {
        if (pIntArr == null || pIntArr.Length == 0)
        {
            Debug.LogError("Array you passed is null or 0!");
            return;
        }
        WriteInt(pIntArr.Length);
        for (int i = 0; i < pIntArr.Length; i++)
        {
            WriteInt(pIntArr[i]);
        }
    }

    public int[] ReadIntArr()
    {
        int[] arr = new int[ReadInt()];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = ReadInt();
        }
        return arr;
    }
    public string ReadString() => _reader.ReadString();
    public int ReadInt() => _reader.ReadInt32();



    /// <summary>
    /// Serializes passed class into BinaryWriter stream
    /// </summary>
    /// <param name="pMessage"></param>
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
    /// <summary>
    /// Returns NativeArray of all the written data for this package
    /// </summary>
    /// <returns></returns>
    public NativeArray<byte> GetBytes()
    {
        NativeArray<byte> arr = new NativeArray<byte>(((MemoryStream)_writer.BaseStream).ToArray(), Allocator.Persistent);
        return arr;
    }
}
