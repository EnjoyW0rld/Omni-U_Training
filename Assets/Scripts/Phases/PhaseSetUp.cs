using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseSetUp : MonoBehaviour
{
    [SerializeField] private Queue<GeneralPhase> _phaseQueue;
    [SerializeField] private Transform _nextPhasesParent;
    public UnityEngine.Events.UnityEvent OnGameStart;

    private bool _isStarted;
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
        GameObject phaseObj = new GameObject();
        var text = phaseObj.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = pPhase.GetType().FullName;
        phaseObj.transform.parent = _nextPhasesParent;
    }
    public void IssueNextPhase()
    {
        GeneralPhase phase = _phaseQueue.Dequeue();
        _timeToNextPhase = phase.TimeToIssue * 60f;
        ServerBehaviour.Instance.ScheduleMessage(phase.PackObject());
    }
    private void Update()
    {
        if (!_isStarted) return;
        _timePassed += Time.deltaTime;
        if (_timePassed > _timeToNextPhase)
        {
            IssueNextPhase();
        }
    }
    public void StartGame()
    {
        _isStarted = true;
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
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        UserData.WriteTextData(pPacket, EmailText);
    }

    public override void Use()
    {
        Debug.Log("Recieved email phase, calling use on it");
        ReferenceHandler.GetObject<PhasesHandler>().AddEmail(EmailText);
    }
}
public abstract class GeneralPhase : NetworkObject
{
    //In minutes
    public int TimeToIssue;
}