using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TeamSelectionContainer : ISerializable
{
    public bool[] TeamsInUse;

    public enum Instruction { TeamAccessibility }
    private Instruction _instruction;

    public TeamSelectionContainer(Instruction pInstruction)
    {
        _instruction = pInstruction;
    }
    public TeamSelectionContainer() { }

    public void DeSerialize(NetworkPacket pPacket)
    {
        if (_instruction == Instruction.TeamAccessibility && !ServerBehaviour.IsThisUserServer)
        {
            TeamsInUse = pPacket.ReadBoolArr();
        }
    }

    public void Serialize(NetworkPacket pPacket)
    {
        if (_instruction == Instruction.TeamAccessibility && ServerBehaviour.IsThisUserServer)
        {
            bool[] teams = ServerBehaviour.Instance.GetTeamNumbers();
            pPacket.WriteBoolArray(teams);
        }
    }

    public void Use()
    {
        if (ServerBehaviour.IsThisUserServer)
        {
            Debug.Log("Sending available teams");
            TeamSelectionContainer cont = new TeamSelectionContainer(Instruction.TeamAccessibility);
            NetworkPacket packet = new NetworkPacket();
            packet.Write(cont);
            ServerBehaviour.Instance.ScheduleMessage(packet);
        }
        else
        {
            GameObject.FindObjectOfType<TeamSelection>().Use(this);
        }
    }
}
