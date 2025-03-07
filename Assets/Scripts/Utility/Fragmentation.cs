using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class Fragmentation
{
    const int MAXBYTESIZE = 1100;//1360;
    private int id;
    private List<byte> bytes;
    private int packagesCount;

    /// <summary>
    /// Take bytesFragment and reads it
    /// </summary>
    /// <param name="pFragByte"></param>
    /// <returns>true - it is the last fragment, false - it is not</returns>
    public bool TryDefragment(FragBytes pFragByte)
    {
        //Debug.Log($"packages left to read {packagesCount}");
        if (pFragByte.ID == id)
        {
            packagesCount--;
            bytes.AddRange(pFragByte.ByteData);
        }
        if (packagesCount == 0)
        {
            NetworkPacket pPacket = new NetworkPacket(bytes.ToArray());
            ISerializable a = pPacket.Read();
            Debug.Log($"WE are at last package");
            a.Use();
            return true;
        }
        return false;
    }
    public void Initialize(FragSetting pFragSettings)
    {
        id = pFragSettings.TargetID;
        bytes = new List<byte>();
        bytes.Capacity = pFragSettings.byteSize;
        packagesCount = pFragSettings.PackagesCount;
        Debug.Log($"In fragmentataion we will read {packagesCount}");
    }
    /*
        public NetworkPacket[] DoFragmentation(byte[] pMessage, out FragSetting fragSetting)
        {
            DebugTextWriter.WriteOnScreen($"message is null - {pMessage != null} and its length is {pMessage.Length}");
            int id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            Type t = typeof(FragBytes);
            int nameSize = t.FullName.Length * sizeof(char);
            int messageSize = MAXBYTESIZE - sizeof(int) - nameSize;
            int packagesCount = (int)math.ceil(pMessage.Length / (float)messageSize);
            Debug.Log($"floatPackageCount {pMessage.Length / (float)messageSize} vs int one {packagesCount}");
            fragSetting = new FragSetting(id, packagesCount, pMessage.Length);

            NetworkPacket[] packets = new NetworkPacket[packagesCount];

            int copyIndex = 0;
            for (int i = 0; i < packagesCount; i++)
            {
                FragBytes oneFragment = new FragBytes();
                oneFragment.ID = id;
                //Debug.Log(pMessage.Length - copyIndex);
                if (pMessage.Length - copyIndex < messageSize)
                {
                    Debug.Log($"For package {i} packing {pMessage.Length - copyIndex} bytes, index we start from is {copyIndex}");
                    oneFragment.ByteData = new byte[pMessage.Length - copyIndex];
                    Array.Copy(pMessage, copyIndex, oneFragment.ByteData, 0, pMessage.Length - copyIndex - 1);
                    packets[i] = oneFragment.PackObject();
                    break;
                }
                else
                {
                    oneFragment.ByteData = new byte[messageSize];
                    Array.Copy(pMessage, copyIndex, oneFragment.ByteData, 0, messageSize);
                    copyIndex += messageSize;
                }
                packets[i] = oneFragment.PackObject();
            }
            return packets;
        }
    */

    public static NetworkPacket[] DoFragmentation(byte[] pMessage, out FragSetting fragSetting)
    {
        //DebugTextWriter.WriteOnScreen($"message is null - {pMessage != null} and its length is {pMessage.Length}");
        int id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        Type t = typeof(FragBytes);
        int nameSize = t.FullName.Length * sizeof(char);
        int messageSize = 1100 - sizeof(int) - nameSize;
        int packagesCount = (int)math.ceil(pMessage.Length / (float)messageSize);
        Debug.Log($"floatPackageCount {pMessage.Length / (float)messageSize} vs int one {packagesCount}");
        //DebugTextWriter.WriteOnScreen($"We will send {packagesCount}. Message length {pMessage.Length} and message size {messageSize}");
        fragSetting = new FragSetting(id, packagesCount, pMessage.Length);

        NetworkPacket[] packets = new NetworkPacket[packagesCount];

        int copyIndex = 0;
        for (int i = 0; i < packagesCount; i++)
        {
            FragBytes oneFragment = new FragBytes();
            oneFragment.ID = id;
            //Debug.Log(pMessage.Length - copyIndex);
            if (pMessage.Length - copyIndex < messageSize)
            {
                Debug.Log($"For package {i} packing {pMessage.Length - copyIndex} bytes, index we start from is {copyIndex}");
                oneFragment.ByteData = new byte[pMessage.Length - copyIndex];
                Array.Copy(pMessage, copyIndex, oneFragment.ByteData, 0, pMessage.Length - copyIndex - 1);
                packets[i] = oneFragment.PackObject();
                break;
            }
            else
            {
                oneFragment.ByteData = new byte[messageSize];
                Array.Copy(pMessage, copyIndex, oneFragment.ByteData, 0, messageSize);
                copyIndex += messageSize;
            }
            packets[i] = oneFragment.PackObject();
        }
        return packets;
    }

    public static byte[] GetBytesFromTexture(Sprite pTargetSprite)
    {
        Texture2D tex = new Texture2D(pTargetSprite.texture.width, pTargetSprite.texture.height, pTargetSprite.texture.format, false);
        Graphics.CopyTexture(pTargetSprite.texture, 0, tex, 0);
        return tex.GetRawTextureData<byte>().ToArray();
    }
}
public class FragSetting : NetworkObject
{
    public int TargetID;
    public int PackagesCount;
    public int byteSize;

    public FragSetting() { }
    public FragSetting(int pTargetID, int pPackagesCount, int pByteSize)
    {
        TargetID = pTargetID;
        PackagesCount = pPackagesCount;
        byteSize = pByteSize;
    }

    public override void DeSerialize(NetworkPacket pPacket)
    {
        TargetID = pPacket.ReadInt();
        PackagesCount = pPacket.ReadInt();
        byteSize = pPacket.ReadInt();
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteInt(TargetID);
        pPacket.WriteInt(PackagesCount);
        pPacket.WriteInt(byteSize);
    }

    public override void Use()
    {

    }
}
public class FragBytes : NetworkObject
{
    public byte[] ByteData;
    public int ID;

    public FragBytes() { }
    public override void DeSerialize(NetworkPacket pPacket)
    {
        ID = pPacket.ReadInt();
        ByteData = pPacket.ReadRemainingBytes();
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteInt(ID);
        pPacket.WriteBytes(ByteData);
    }

    public override void Use()
    {
        //throw new NotImplementedException();
    }
}
public class DEBUGIMAGE : NetworkObject
{
    public Sprite Spr;
    public DEBUGIMAGE() { }
    public override void DeSerialize(NetworkPacket pPacket)
    {
        Spr = pPacket.ReadSprite();
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteSprite(Spr);
    }

    public override void Use()
    {
        GameObject obj = new GameObject();
        //obj.transform.position = Vector3.zero;
        Spr.name = "SpriteSent";
        GameObject.FindObjectOfType<SpriteRenderer>().sprite = Spr;
        SpriteRenderer rend = obj.AddComponent<SpriteRenderer>();
        rend.sprite = Spr;
        Debug.Log($"Sprite on renderer {rend.sprite.name}");
    }
}
