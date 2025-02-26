using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmailingHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailText;
    [SerializeField] private TMP_InputField _recipientInput;
    [SerializeField] private GameObject _notification;
    private PC_UI _pcUi;
    private PC_UI PC_UI { get { if (_pcUi == null) _pcUi = FindObjectOfType<PC_UI>(); return _pcUi; } }

    public void SendWrittenAction()
    {
        EmailingContainer email = new EmailingContainer(EmailingContainer.Instructions.Email);
        NetworkPacket packet = new NetworkPacket();
        email.Email = _emailText.text;
        email.Sender = _recipientInput.text;
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
    public string Sender = "";
    public string Reply = "";

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
                Sender = pPacket.ReadString();
                Reply = pPacket.ReadString();
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
                pPacket.WriteString(Sender);
                pPacket.WriteString(Reply);
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
            handler.Use(this);
        }
    }

    #region RCPFunctions

    #endregion
}