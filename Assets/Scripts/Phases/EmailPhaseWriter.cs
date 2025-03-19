using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EmailPhaseWriter : MonoBehaviour
{
    [SerializeField] private TMP_InputField _title;
    [SerializeField] private TMP_InputField _sender;
    [SerializeField] private TMP_InputField _mailText;
    [SerializeField] private TMP_InputField _recipient;
    [SerializeField] private TMP_InputField _timeToNextPhase;
    [Header("In game time")]
    [SerializeField] private TMP_InputField _InGameMinutes;
    [SerializeField] private TMP_InputField _inGameHours;

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

        emailPhase.InGameTime = int.Parse(_inGameHours.text) * 60 + int.Parse(_InGameMinutes.text);
        CleanAllInputs();
        phaseSetUp.AddPhase(emailPhase);
    }
    private void CleanAllInputs()
    {
        _timeToNextPhase.text = "";
    }
}
