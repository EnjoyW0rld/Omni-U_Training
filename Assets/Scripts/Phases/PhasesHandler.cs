using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhasesHandler : MonoBehaviour
{
    private PhaseInteractionExecutor[] _phaseInteractions;
    private void Start()
    {
        _phaseInteractions = FindObjectsOfType<PhaseInteractionExecutor>(true);

        ClientBehaviour.Instance.SchedulePackage((new RCPInvokeContainer("SendUserData")).PackObject());
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
    public enum Instructions { Browser }
    //public Instructions Instruction;

    public string PhaseName;
    public UserData.TextData Email;

    public PhasesContainer() { }
    public PhasesContainer(string pPhaseName)
    {
        PhaseName = pPhaseName;
    }
    
    public override void DeSerialize(NetworkPacket pPacket)
    {
        PhaseName = pPacket.ReadString();
    }
    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteString(PhaseName);
    }
    public override void Use()
    {
        ReferenceHandler.GetObject<PhasesHandler>().CallPhaseInteraction(PhaseName);
    }
}