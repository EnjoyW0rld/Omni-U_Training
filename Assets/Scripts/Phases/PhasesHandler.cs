using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhasesHandler : MonoBehaviour
{
    private PhaseInteractionExecutor[] _phaseInteractions;
    private void Start()
    {
        _phaseInteractions = FindObjectsOfType<PhaseInteractionExecutor>(true);
    }

    public void CallPhaseInteraction(string pPhaseName)
    {
        for (int i = 0; i < _phaseInteractions.Length; i++)
        {
            _phaseInteractions[i].Execute(pPhaseName);
            ReferenceHandler.GetObject<NotificationHandler>().CallNotification();
        }
    }
    public void AddEmail(UserData.TextData pEmail)
    {
        ReferenceHandler.GetObject<PC_UI>(true).AddToArchive(pEmail);
    }
}
public class PhasesContainer : NetworkObject
{
    public enum Instructions { Browser, Email }
    public Instructions Instruction;

    public string PhaseName;
    public UserData.TextData Email;

    public PhasesContainer() { }
    public PhasesContainer(string pPhaseName)
    {
        PhaseName = pPhaseName;
    }
    public PhasesContainer(Instructions pInstruction)
    {
        Instruction = pInstruction;
    }

    public override void DeSerialize(NetworkPacket pPacket)
    {
        Instruction = (Instructions)pPacket.ReadInt();
        switch (Instruction)
        {
            case Instructions.Browser:
                PhaseName = pPacket.ReadString();
                break;
            case Instructions.Email:
                Email = new UserData.TextData(pPacket.ReadString(), pPacket.ReadString(), pPacket.ReadString());
                Email.Sender = pPacket.ReadString();
                Debug.Log(Email.Sender + " is email sender");
                break;
        }
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteInt((int)Instruction);
        switch (Instruction)
        {
            case Instructions.Browser:
                pPacket.WriteString(PhaseName);
                break;
            case Instructions.Email:
                pPacket.WriteString(Email.Text);
                pPacket.WriteString(Email.Recipient);
                pPacket.WriteString(Email.Title);
                pPacket.WriteString(Email.Sender);
                break;
        }
    }

    public override void Use()
    {
        switch (Instruction)
        {
            case Instructions.Browser:
                ReferenceHandler.GetObject<PhasesHandler>().CallPhaseInteraction(PhaseName);
                break;
            case Instructions.Email:
                ReferenceHandler.GetObject<PhasesHandler>().AddEmail(Email);
                break;
        }
    }
}