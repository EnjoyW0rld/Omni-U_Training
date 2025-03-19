using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PhaseSetUp : MonoBehaviour
{
    [SerializeField] private Queue<GeneralPhase> _phaseQueue;
    [SerializeField] private Transform _nextPhasesParent;
    [SerializeField] private TextMeshProUGUI _nextPhaseTimerUI;
    [SerializeField] private PhaseQueueUI _phaseQueuePrefab;
    public UnityEngine.Events.UnityEvent OnGameStart;

    //private bool _isStarted;
    private float _timePassed;
    private float _timeToNextPhase;

    private void Start()
    {
        if (_phaseQueue == null) _phaseQueue = new Queue<GeneralPhase>();
        if (OnGameStart == null) OnGameStart = new UnityEngine.Events.UnityEvent();
    }
    public void AddPhase(GeneralPhase pPhase)
    {
        _phaseQueue.Enqueue(pPhase);
        //Adding info about next phase, need to change later to propper way
        PhaseQueueUI newPhase = Instantiate(_phaseQueuePrefab, _nextPhasesParent);
        newPhase.Initialize(pPhase);
    }
    public void IssueNextPhase()
    {
        GeneralPhase phase = _phaseQueue.Dequeue();
        _timeToNextPhase = phase.TimeToIssue * 60f;
        ServerBehaviour.Instance.ScheduleMessage(phase.PackObject());
    }
    private void Update()
    {
        if (!ServerBehaviour.Instance.IsStarted) return;
        _timeToNextPhase -= Time.deltaTime;
        _nextPhaseTimerUI.text = $"{(int)_timeToNextPhase / 60}:{(int)(_timeToNextPhase % 60)}";
        if (_timeToNextPhase < 0)
        {
            IssueNextPhase();
        }
    }
    public void StartGame()
    {
        ServerBehaviour.Instance.StartGame();
        OnGameStart?.Invoke();
        RCPInvokeContainer rcp = new RCPInvokeContainer("StartGameForClient");
        ServerBehaviour.Instance.ScheduleMessage(rcp.PackObject());
    }
}
public class EmailPhase : GeneralPhase
{
    public UserData.TextData EmailText;
    public EmailPhase() { }
    public EmailPhase(UserData.TextData pEmailText)
    {
        EmailText = pEmailText;
    }

    public override void DeSerialize(NetworkPacket pPacket)
    {
        EmailText = UserData.ReadTextData(pPacket);
        base.DeSerialize(pPacket);
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        UserData.WriteTextData(pPacket, EmailText);
        base.Serialize(pPacket);
    }

    public override void Use()
    {
        Debug.Log("Recieved email phase, calling use on it");
        ReferenceHandler.GetObject<PhasesHandler>().AddEmail(EmailText);
        base.Use();
    }
}
public abstract class GeneralPhase : NetworkObject
{
    //In minutes
    public int TimeToIssue;
    //Is translated to minutes
    public int InGameTime;
    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteInt(InGameTime);
    }
    public override void DeSerialize(NetworkPacket pPacket)
    {
        InGameTime = pPacket.ReadInt();
    }
    public override void Use()
    {
        ReferenceHandler.GetObject<PhasesHandler>(true).UpdateCurrentTime(InGameTime);
    }
}