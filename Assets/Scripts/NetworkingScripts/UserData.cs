using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Networking.Transport;
using UnityEngine;

public struct UserData
{
    private uint _teamNum;
    private List<TextData> _emails;

    public uint TeamNum { get { return _teamNum; } }

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