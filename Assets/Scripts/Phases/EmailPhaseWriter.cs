using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmailPhaseWriter : MonoBehaviour
{
    [SerializeField] private TMP_InputField _title;
    [SerializeField] private TMP_InputField _sender;
    [SerializeField] private TMP_InputField _mailText;
    [SerializeField] private TMP_InputField _recipient;
    [SerializeField] private TMP_InputField _timeToNextPhase;


    /*public void IssuePhase()
    {
        PhasesContainer phasesContainer = new PhasesContainer(PhasesContainer.Instructions.Email);
        phasesContainer.Email = GetTextData();

        NetworkPacket packet = new NetworkPacket();
        packet.Write(phasesContainer);
        ServerBehaviour.Instance.ScheduleMessage(packet);
    }*/
    private UserData.TextData GetTextData()
    {
        UserData.TextData textData = new UserData.TextData();

        textData.Text = _mailText.text;
        textData.Sender = _sender.text;
        textData.Recipient = _recipient.text;
        textData.Title = _title.text;
        return textData;
    }
    public void AddEmailPhase()
    {
        PhaseSetUp phaseSetUp = ReferenceHandler.GetObject<PhaseSetUp>();
        EmailPhase emailPhase = new EmailPhase(GetTextData());
        emailPhase.TimeToIssue = int.Parse(_timeToNextPhase.text);
        _timeToNextPhase.text = "";
        phaseSetUp.AddPhase(emailPhase);
    }
}
