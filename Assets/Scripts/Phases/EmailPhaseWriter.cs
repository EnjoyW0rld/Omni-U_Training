using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmailPhaseWriter : MonoBehaviour
{
    [SerializeField] private TMP_InputField _title;
    [SerializeField] private TMP_InputField _sender;
    [SerializeField] private TMP_InputField _mailText;
    [SerializeField] protected TMP_InputField _recipient;

    public void IssuePhase()
    {
        PhasesContainer phasesContainer = new PhasesContainer(PhasesContainer.Instructions.Email);
        phasesContainer.Email = new UserData.TextData();
        phasesContainer.Email.Text = _mailText.text;
        phasesContainer.Email.Sender = _sender.text;
        phasesContainer.Email.Recipient = _recipient.text;
        phasesContainer.Email.Title = _title.text;

        NetworkPacket packet = new NetworkPacket();
        packet.Write(phasesContainer);
        ServerBehaviour.Instance.ScheduleMessage(packet);
    }
}
