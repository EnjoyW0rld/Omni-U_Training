using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Networking.Transport;
using UnityEngine;

public class UserData : NetworkObject
{
    private uint _teamNum;
    private List<TextData> _emails;

    public uint TeamNum { get { return _teamNum; } }
    public UserData() { }
    public TextData[] GetEmails()
    {
        return _emails.ToArray();
    }
    public TextData GetEmail(int pIndex)
    {
        if (pIndex < 0 || _emails.Count < pIndex)
        {
            Debug.LogAssertion($"Tried to get email with {pIndex} passed as number");
            return null;
        }
        return _emails[pIndex];
    }
    public int GetEmailsCount() => _emails.Count;

    public UserData(uint pTeamNum)
    {
        //_connection = pConnection;
        _teamNum = pTeamNum;
        _emails = new List<TextData>();
    }
    public void AddEmail(TextData pTextData)
    {
        _emails.Add(pTextData);
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteInt(_emails.Count);
        for (int i = 0; i < _emails.Count; i++)
        {
            WriteTextData(pPacket, _emails[i]);
        }
    }
    public override void DeSerialize(NetworkPacket pPacket)
    {
        _emails = new List<TextData>();
        int capacity = pPacket.ReadInt();
        _emails.Capacity = capacity;
        for (int i = 0; i < capacity; i++)
        {
            _emails.Add(ReadTextData(pPacket));
        }
    }
    public override void Use()
    {
        Debug.Log("Got user data");
        if (!ServerBehaviour.IsThisUserServer)
        {
            ClientBehaviour.Instance.UpdateUserData(this);
        }
    }


    public static void WriteTextData(NetworkPacket pPacket, TextData pTextData)
    {
        pPacket.WriteString(pTextData.Text);
        pPacket.WriteString(pTextData.Recipient);
        pPacket.WriteString(pTextData.Sender);
        pPacket.WriteString(pTextData.Reply);
        pPacket.WriteString(pTextData.Title);
    }
    public static TextData ReadTextData(NetworkPacket pPacket)
    {
        TextData textData = new TextData();
        textData.Text = pPacket.ReadString();
        textData.Recipient = pPacket.ReadString();
        textData.Sender = pPacket.ReadString();
        textData.Reply = pPacket.ReadString();
        textData.Title = pPacket.ReadString();
        return textData;
    }

    // ------------
    // Data structs
    // ------------
    public class TextData
    {
        public string Text;
        public string Recipient;
        public string Sender;
        public string Reply;
        public string Title;

        public TextData()
        {
            Text = "";
            Recipient = "";
            Reply = "";
            Title = "";
            Sender = "";
        }
        public TextData(string pText, string pRecipient, string pTitle)
        {
            Text = pText;
            Recipient = pRecipient;
            Title = pTitle;
            Reply = "";
            Sender = "";
        }
        public TextData(EmailingContainer pCont)
        {
            Text = pCont.Email;
            Recipient = pCont.Recipient;
            Reply = pCont.Reply;
            Title = pCont.Title;
            Sender = pCont.Sender;
        }

    }
}
public class GameData
{
    public int CurrentTime;
}