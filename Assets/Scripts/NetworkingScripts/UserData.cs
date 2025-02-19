using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Networking.Transport;
using UnityEngine;

public struct UserData
{
    //private NetworkConnection _connection;
    private uint _teamNum;
    private List<TextData> _emails;

    public uint TeamNum { get { return _teamNum; } }
    public TextData[] GetEmails()
    {
        return _emails.ToArray();
    }
    public UserData(uint pTeamNum)
    {
        //_connection = pConnection;
        _teamNum = pTeamNum;
        _emails = new List<TextData>();
    }
    public void AddEmail(string pText, string pSender)
    {
        _emails.Add(new TextData(pText, pSender));
    }

    // ------------
    // Data structs
    // ------------
    public struct TextData
    {
        public string Text;
        public string Sender;
        public TextData(string pText, string pSender)
        {
            Text = pText;
            Sender = pSender;
        }
    }
}