using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmailingHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailText;
    [SerializeField] private TMP_InputField _recipientInput;
    [SerializeField] private TMP_InputField _TitleInput;
    [SerializeField] private GameObject _notification;

    private PC_UI PC_UI { get { return ReferenceHandler.GetObject<PC_UI>(true); } }

    public void SendWrittenAction()
    {
        EmailingContainer email = new EmailingContainer(EmailingContainer.Instructions.Email);
        NetworkPacket packet = new NetworkPacket();
        email.Email = _emailText.text;
        email.Recipient = _recipientInput.text;
        email.Title = _TitleInput.text;

        packet.Write(email);
        ClientBehaviour.Instance.SchedulePackage(packet);
        _emailText.text = "";
        _recipientInput.text = "";
    }
    public void Use(EmailingContainer pCont)
    {
        if (_notification != null) _notification.SetActive(true);
        else Debug.LogAssertion("Notification object is null");

        PC_UI.AddToArchive(new UserData.TextData(pCont));
    }
}
public class EmailingContainer : RCPBase, ISerializable
{
    public enum Instructions { RCP, Email }
    public string Email = "";
    public string Recipient = "";
    public string Sender = "";
    public string Reply = "";
    public string Title = "";

    private Instructions _instruction;
    public EmailingContainer(Instructions pInstruction)
    {
        _instruction = pInstruction;
    }
    public EmailingContainer() { }
    public void DeSerialize(NetworkPacket pPacket)
    {
        _instruction = (Instructions)pPacket.ReadInt();
        switch (_instruction)
        {
            case Instructions.RCP:
                UseRCP(pPacket);
                break;
            case Instructions.Email:
                Email = pPacket.ReadString();
                Recipient = pPacket.ReadString();
                Reply = pPacket.ReadString();
                Title = pPacket.ReadString();
                Sender = pPacket.ReadString();
                break;
        }
    }

    public void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteInt((int)_instruction);
        switch (_instruction)
        {
            case Instructions.RCP:
                pPacket.WriteString(RCPName);
                break;
            case Instructions.Email:
                pPacket.WriteString(Email);
                pPacket.WriteString(Recipient);
                pPacket.WriteString(Reply);
                pPacket.WriteString(Title);
                pPacket.WriteString(Sender);
                break;
        }
    }

    public void Use()
    {
        if (ServerBehaviour.IsThisUserServer)
        {
            ReferenceHandler.GetObject<MasterActions>().Use(this);
        }
        else
        {
            EmailingHandler handler = ReferenceHandler.GetObject<EmailingHandler>(true);
            if (handler == null) Debug.LogError("NO EMAILING HANDER");
            Debug.Log("Recieved " + Reply);
            handler.Use(this);
        }
    }

    #region RCPFunctions

    #endregion
}