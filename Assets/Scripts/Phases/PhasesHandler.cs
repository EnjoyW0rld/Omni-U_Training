using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhasesHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentTime;
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
        }
    }
    public void AddEmail(UserData.TextData pEmail)
    {
        ReferenceHandler.GetObject<PC_UI>(true).AddToArchive(pEmail);
        ReferenceHandler.GetObject<NotificationHandler>().AddEmailNotification();
    }
    /// <summary>
    /// Updates current time UI
    /// </summary>
    /// <param name="time">Time needs to be in minutes</param>
    public void UpdateCurrentTime(int time)
    {
        _currentTime.text = $"{time / 60}:{time % 60}";
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