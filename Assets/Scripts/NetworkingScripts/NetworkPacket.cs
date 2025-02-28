using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Unity.Collections;
using Unity.VisualScripting;
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
    public NetworkPacket(DataStreamReader pStreamReader)
    {
        NativeArray<byte> data = new NativeArray<byte>(pStreamReader.Length, Allocator.Temp);
        pStreamReader.ReadBytes(data);

        _reader = new BinaryReader(new MemoryStream(data.ToArray()));

    }
    ~NetworkPacket()
    {
        _writer.Dispose();
        _reader.Dispose();
    }

    public void WriteString(string pString) => _writer.Write(pString);
    public void WriteInt(int pInt) => _writer.Write(pInt);
    public void WriteUInt(uint pInt) => _writer.Write(pInt);
    public void WriteBool(bool pBool) => _writer.Write(pBool);
    public void WriteFloat(float pFloat) => _writer.Write(pFloat);
    public void WriteVector2(Vector2 pVec2)
    {
        WriteFloat(pVec2.x);
        WriteFloat(pVec2.y);
    }
    public void WriteVector3(Vector3 pVec3)
    {
        WriteFloat(pVec3.x);
        WriteFloat(pVec3.y);
        WriteFloat(pVec3.z);
    }
    public void WriteRect(Rect pRect)
    {
        WriteFloat(pRect.x);
        WriteFloat(pRect.y);
        WriteFloat(pRect.width);
        WriteFloat(pRect.height);
    }
    public void WriteUIntArray(uint[] pIntArr)
    {
        if (pIntArr == null || pIntArr.Length == 0)
        {
            Debug.LogError("Array you passed is null or 0!");
            return;
        }
        WriteInt(pIntArr.Length);
        for (int i = 0; i < pIntArr.Length; i++)
        {
            WriteUInt(pIntArr[i]);
        }
    }
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
    public void WriteBoolArray(bool[] pBoolArr)
    {
        if (pBoolArr == null || pBoolArr.Length == 0)
        {
            Debug.LogError("Array you passed is null or 0!");
            return;
        }

        WriteInt(pBoolArr.Length);
        for (int i = 0; i < pBoolArr.Length; i++)
        {
            WriteBool(pBoolArr[i]);
        }
    }
    public void WriteSprite(Sprite pSprite)
    {
        byte[] img = pSprite.texture.EncodeToPNG();
        Debug.Log($"img is null - {img == null} and its lenght is - {img.Length}");
        WriteInt(pSprite.texture.width);
        WriteInt(pSprite.texture.height);
        WriteInt(img.Length);
        _writer.Write(img);
        WriteRect(pSprite.rect);
        WriteVector2(pSprite.pivot);
        Debug.Log(_writer.BaseStream.Length);
    }
    public Sprite ReadSprite()
    {
        Texture2D tex = new Texture2D(ReadInt(), ReadInt());
        int length = ReadInt();
        Debug.Log($"Creaing texture with size {tex.width} and {tex.height}");
        Debug.Log($"array lenght is {length}");

        byte[] data = new byte[length];
        _reader.Read(data, 0, length);
        tex.LoadImage(data);

        return Sprite.Create(tex, ReadRect(), ReadVec2());
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
    public uint[] ReadUIntArr()
    {
        uint[] arr = new uint[ReadInt()];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = ReadUInt();
        }
        return arr;
    }
    public bool[] ReadBoolArr()
    {
        bool[] arr = new bool[ReadInt()];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = ReadBool();
        }
        return arr;
    }

    public bool ReadBool() => _reader.ReadBoolean();
    public uint ReadUInt() => _reader.ReadUInt32();
    public string ReadString() => _reader.ReadString();
    public int ReadInt() => _reader.ReadInt32();
    public float ReadFloat() => _reader.ReadSingle();
    public Vector2 ReadVec2() => new Vector2(ReadFloat(), ReadFloat());
    public Vector3 ReadVec3() => new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    public Rect ReadRect() => new Rect(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
    public object ReadByType(Type type)
    {
        object obj = null;
        if (type == typeof(Sprite))
        {
            obj = ReadSprite();
        }
        else
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                    obj = _reader.ReadInt32();
                    break;
                case TypeCode.Boolean:
                    obj = _reader.ReadBoolean();
                    break;
                case TypeCode.String:
                    obj = _reader.ReadString();
                    break;
            }
        }
        Debug.Log("obj is " + obj);
        return obj;
    }

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
        NativeArray<byte> arr = new NativeArray<byte>(((MemoryStream)_writer.BaseStream).ToArray(), Allocator.Temp);
        return arr;
    }
}
