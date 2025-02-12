using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Networking.Transport;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class TeamSelectionContainer : RCPBase, ISerializable
{
    public bool[] TeamsInUse;
    public int TeamId;

    public enum Instruction { TeamAccessibility, ConnectRequest, RCP }
    private Instruction _instruction;

    public TeamSelectionContainer(Instruction pInstruction)
    {
        _instruction = pInstruction;
    }
    public TeamSelectionContainer() { }

    public void DeSerialize(NetworkPacket pPacket)
    {
        _instruction = (Instruction)pPacket.ReadInt();
        if (_instruction == Instruction.TeamAccessibility && !ServerBehaviour.IsThisUserServer)
        {
            TeamsInUse = pPacket.ReadBoolArr();
        }
        if (_instruction == Instruction.ConnectRequest)
        {
            TeamId = pPacket.ReadInt();
        }
        if (_instruction == Instruction.RCP)
        {
            //RCPName = pPacket.ReadString();
            UseRCP(pPacket);
        }
    }

    public void Serialize(NetworkPacket pPacket)
    {
        pPacket.WriteInt((int)_instruction);
        if (_instruction == Instruction.TeamAccessibility && ServerBehaviour.IsThisUserServer)
        {
            bool[] teams = ServerBehaviour.Instance.GetTeamNumbers();
            pPacket.WriteBoolArray(teams);
        }
        if (_instruction == Instruction.ConnectRequest)
        {
            pPacket.WriteInt(TeamId);
        }
        if (_instruction == Instruction.RCP)
        {
            pPacket.WriteString(RCPName);
            //UseRCP(pPacket);
        }
    }

    public void Use()
    {
        if (ServerBehaviour.IsThisUserServer)
        {
            switch (_instruction)
            {
                case Instruction.TeamAccessibility:
                    SendTeamsAvailability();
                    break;
                case Instruction.ConnectRequest:
                    NetworkConnection conn = ServerBehaviour.Instance.GetCurrentConnection();
                    if (conn == default) Debug.LogError("conn turned to be default!!");
                    ServerBehaviour.Instance.AssignConnection(conn, TeamId);
                    break;
            }
        }
        else
        {
            GameObject.FindObjectOfType<TeamSelection>().Use(this);
        }
    }
    private void SendTeamsAvailability()
    {
        Debug.Log("Sending available teams");
        TeamSelectionContainer cont = new TeamSelectionContainer(Instruction.TeamAccessibility);
        NetworkPacket packet = new NetworkPacket();
        packet.Write(cont);
        ServerBehaviour.Instance.ScheduleMessage(packet);
    }
    [MyRCP]
    public void SendTeamNumber(int pTeam)
    {

    }
    [MyRCP]
    public void TestCase()
    {
        Debug.Log("Called using RCP");
    }
}
