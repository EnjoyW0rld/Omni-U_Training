using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamScoreHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _teams;
    private GameObject _parentObject;
    private int[] _score;

    private void Start()
    {
        _parentObject = gameObject;
        _teams[ClientBehaviour.Instance.TeamNubmer - 1].color = Color.red;
        _score = new int[_teams.Length];
        UpdateScores();
    }

    private void UpdateScores()
    {
        for (int i = 0; i < _teams.Length; i++)
        {
            _teams[i].text = "Team " + (i + 1) + ": " + _score[i];
        }
    }
    /// <summary>
    /// Provide actual team number, not the array team number
    /// </summary>
    /// <param name="pScore"></param>
    /// <param name="pTeam"></param>
    public void AddScore(int pScore, int pTeam)
    {
        _score[pTeam - 1] += pScore;
        UpdateScores();
    }
}
public class ScoreContainer : NetworkObject
{
    public enum Instructions { AssignScore }
    public Instructions Instruction;

    public int TargetTeam;
    public int ScoreToAdd;

    public ScoreContainer() { }
    public ScoreContainer(Instructions pInstructions) { Instruction = pInstructions; }
    public override void DeSerialize(NetworkPacket pPacket)
    {
        Instruction = (Instructions)pPacket.ReadInt();
        switch (Instruction)
        {
            case Instructions.AssignScore:
                TargetTeam = pPacket.ReadInt();
                ScoreToAdd = pPacket.ReadInt();
                break;
        }
    }

    public override void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteInt((int)Instruction);
        switch (Instruction)
        {
            case Instructions.AssignScore:
                pPacket.WriteInt(TargetTeam);
                pPacket.WriteInt(ScoreToAdd);
                break;
        }
    }

    public override void Use()
    {
        switch (Instruction)
        {
            case Instructions.AssignScore:
                ReferenceHandler.GetObject<TeamScoreHandler>().AddScore(ScoreToAdd, TargetTeam);
                break;
        }
    }
}