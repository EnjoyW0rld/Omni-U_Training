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
}
public class PhasesContainer : ISerializable
{
    public string PhaseName;
    public PhasesContainer() { }
    public PhasesContainer(string pPhaseName)
    {
        PhaseName = pPhaseName;
    }

    public void DeSerialize(NetworkPacket pPacket)
    {
        PhaseName = pPacket.ReadString();
    }

    public void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteString(PhaseName);
    }

    public void Use()
    {
        ReferenceHandler.GetObject<PhasesHandler>().CallPhaseInteraction(PhaseName);
    }
}